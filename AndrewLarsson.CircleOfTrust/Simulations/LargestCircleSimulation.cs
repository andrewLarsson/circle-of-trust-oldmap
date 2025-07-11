﻿using AndrewLarsson.CircleOfTrust.Domain;
using developersBliss.OLDMAP.Hosting;
using developersBliss.OLDMAP.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace AndrewLarsson.CircleOfTrust.Simulations;
public class LargestCircleSimulation(
	IHostApplicationLifetime applicationLifetime,
	IDomainMessageSender domainMessageSender,
	DomainMessagePacker domainMessagePacker,
	ILogger<LargestCircleSimulation> logger
) : BackgroundService {
	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		await Task.Yield();

		const int totalUsers = 100;
		const int actionsPerUser = 5;
		var random = new Random();
		var secretKey = "April Fools!";

		// Generate 100 unique users
		var users = Enumerable.Range(1, totalUsers)
			.Select(_ => Guid.NewGuid().ToString("N")[..5])
			.ToList()
		;

		// Create a mapping of each user's circle address
		var userCircles = users.ToDictionary(
			user => user,
			user => new AggregateRootAddress {
				Domain = "AndrewLarsson.CircleOfTrust",
				AggregateRoot = "Circle",
				AggregateRootId = user
			}
		);

		List<PackedDomainMessage> domainMessages = [];

		// Step 1: Each user claims their own circle
		foreach (var user in users) {
			domainMessages.Add(domainMessagePacker.PackMessage(
				domainMessageId: Guid.NewGuid().ToString("N"),
				address: userCircles[user],
				domainMessage: new ClaimCircle($"{user} Circle", user, secretKey)
			));
		}

		// Step 2: Each user takes X turns performing random actions
		for (int turn = 0; turn < actionsPerUser; turn++) {
			foreach (var user in users) {
				var targetUser = users.Where(u => u != user).OrderBy(_ => random.Next()).First(); // Pick a random user, excluding self
				var targetCircle = userCircles[targetUser];

				// Randomly decide if the user will join or betray
				var actionType = random.NextDouble() < 0.1 ? 1 : 0; // 10% for Betray, 90% for Join
				var useCorrectKey = random.NextDouble() > 0.2; // 80% chance of using the correct key
				var key = useCorrectKey ? secretKey : "wrong key";

				object domainMessage = actionType switch {
					0 => new JoinCircle(user, key),
					1 => new BetrayCircle(user, key),
					_ => throw new InvalidOperationException()
				};

				domainMessages.Add(domainMessagePacker.PackMessage(
					domainMessageId: Guid.NewGuid().ToString("N"),
					address: targetCircle,
					domainMessage: domainMessage
				));
			}
		}

		// Setup shutdown triggers.
		applicationLifetime.ApplicationStopping.Register(() => {
			(string largestCircleId, uint largestCircleMemberCount) = LargestCircleSimulationEventHandler.circleMemberCounts.MaxBy(kvp => kvp.Value);
			logger.LogInformation("Largest Circle: {largestCircleId} - {largestCircleMemberCount}", largestCircleId, largestCircleMemberCount);
		});
		var lastDomainMessageId = domainMessages.Last().Id;
		LargestCircleSimulationEventHandler.lastDomainMessageId = lastDomainMessageId;
		LargestCircleSimulationEventHandler.stopwatch.Start();

		// Send all messages
		logger.LogInformation("Sending {X} Domain Messages", domainMessages.Count);
		Stopwatch stopwatch = new();
		stopwatch.Start();
		foreach (var domainMessage in domainMessages) {
			await domainMessageSender.Send(domainMessage);
		}
		stopwatch.Stop();
		var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
		logger.LogInformation("Last message sent. Seconds to send all messages: {elapsedSeconds}", elapsedSeconds);
	}
}

public class LargestCircleSimulationEventHandler(
	IHostApplicationLifetime applicationLifetime,
	ILogger<LargestCircleSimulationEventHandler> logger
) : IDomainEventHandler<CircleClaimed>,
	IDomainEventHandler<CircleJoined>,
	IDomainEventHandler<CircleBetrayed>,
	IDomainEventHandler<CircleAlreadyBetrayed>,
	IDomainEventHandler<KeyDoesNotUnlockCircle>,
	IDomainEventHandler<UserAlreadyMemberOfCircle> {
	public static string lastDomainMessageId = "";
	public static Stopwatch stopwatch = new();
	public static bool stopApplicationWhenFinished = false;
	public static readonly ConcurrentDictionary<string, uint> circleMemberCounts = [];

	public Task Check<T>(DomainEvent<T> domainEvent) where T: notnull {
		if (domainEvent.DomainMessageId == lastDomainMessageId) {
			stopwatch.Stop();
			var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
			logger.LogInformation("Last messaged received. Total seconds to process all messages: {elapsedSeconds}", elapsedSeconds);
			if (stopApplicationWhenFinished) {
				applicationLifetime.StopApplication();
			}
		}
		return Task.CompletedTask;
	}

	public Task Handle(DomainEvent<CircleClaimed> domainEvent) {
		circleMemberCounts[domainEvent.Address.AggregateRootId] = 1;
		return Check(domainEvent);
	}

	public Task Handle(DomainEvent<CircleJoined> domainEvent) {
		if (circleMemberCounts.ContainsKey(domainEvent.Address.AggregateRootId)) {
			circleMemberCounts[domainEvent.Address.AggregateRootId] += 1;
		}
		return Check(domainEvent);
	}

	public Task Handle(DomainEvent<CircleBetrayed> domainEvent) => Check(domainEvent);
	public Task Handle(DomainEvent<CircleAlreadyBetrayed> domainEvent) => Check(domainEvent);
	public Task Handle(DomainEvent<KeyDoesNotUnlockCircle> domainEvent) => Check(domainEvent);
	public Task Handle(DomainEvent<UserAlreadyMemberOfCircle> domainEvent) => Check(domainEvent);
}

public static class LargestCircleSimulationHostApplicationBuilderExtensions {
	public static IHostApplicationBuilder AddLargestCircleSimulation(this IHostApplicationBuilder applicationBuilder) {
		applicationBuilder.Services.AddHostedService<LargestCircleSimulation>();
		applicationBuilder
			.TryAddKafkaDomainMessageSender()
			.AddKafkaDomainEventApplication(Applications.LargestCircleSimulation)
			.AddDomainEventHandler<CircleClaimed, LargestCircleSimulationEventHandler>(Applications.LargestCircleSimulation)
			.AddDomainEventHandler<CircleJoined, LargestCircleSimulationEventHandler>(Applications.LargestCircleSimulation)
			.AddDomainEventHandler<CircleBetrayed, LargestCircleSimulationEventHandler>(Applications.LargestCircleSimulation)
			.AddDomainEventHandler<CircleAlreadyBetrayed, LargestCircleSimulationEventHandler>(Applications.LargestCircleSimulation)
			.AddDomainEventHandler<KeyDoesNotUnlockCircle, LargestCircleSimulationEventHandler>(Applications.LargestCircleSimulation)
			.AddDomainEventHandler<UserAlreadyMemberOfCircle, LargestCircleSimulationEventHandler>(Applications.LargestCircleSimulation)
		;
		return applicationBuilder;
	}
}
