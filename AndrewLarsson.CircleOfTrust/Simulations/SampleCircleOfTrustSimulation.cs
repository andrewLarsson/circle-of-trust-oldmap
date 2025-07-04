﻿using AndrewLarsson.CircleOfTrust.Domain;
using developersBliss.OLDMAP.Hosting;
using developersBliss.OLDMAP.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndrewLarsson.CircleOfTrust.Simulations;
public class SampleCircleOfTrustSimulation(IDomainMessageSender domainMessageSender, DomainMessagePacker domainMessagePacker) : BackgroundService {
	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		await Task.Yield();
		var user1 = Guid.NewGuid().ToString("N")[..5];
		var user2 = Guid.NewGuid().ToString("N")[..5];
		var user3 = Guid.NewGuid().ToString("N")[..5];
		var user4 = Guid.NewGuid().ToString("N")[..5];
		var user5 = Guid.NewGuid().ToString("N")[..5];
		var secretKey = "April Fools!";
		AggregateRootAddress user1Circle = new() {
			Domain = "AndrewLarsson.CircleOfTrust",
			AggregateRoot = "Circle",
			AggregateRootId = user1
		};
		List<PackedDomainMessage> domainMessages = [
			domainMessagePacker.PackMessage(
				domainMessageId: Guid.NewGuid().ToString("N"),
				address: user1Circle,
				domainMessage: new ClaimCircle(
					$"{user1} Circle",
					user1,
					secretKey
				)
			),
			domainMessagePacker.PackMessage(
				domainMessageId: Guid.NewGuid().ToString("N"),
				address: user1Circle,
				domainMessage: new JoinCircle(
					user1,
					secretKey
				)
			),
			domainMessagePacker.PackMessage(
				domainMessageId: Guid.NewGuid().ToString("N"),
				address: user1Circle,
				domainMessage: new JoinCircle(
					user2,
					"wrong key"
				)
			),
			domainMessagePacker.PackMessage(
				domainMessageId: Guid.NewGuid().ToString("N"),
				address: user1Circle,
				domainMessage: new JoinCircle(
					user2,
					secretKey
				)
			),
			domainMessagePacker.PackMessage(
				domainMessageId: Guid.NewGuid().ToString("N"),
				address: user1Circle,
				domainMessage: new JoinCircle(
					user2,
					secretKey
				)
			),
			domainMessagePacker.PackMessage(
				domainMessageId: Guid.NewGuid().ToString("N"),
				address: user1Circle,
				domainMessage: new BetrayCircle(
					user3,
					secretKey
				)
			),
			domainMessagePacker.PackMessage(
				domainMessageId: Guid.NewGuid().ToString("N"),
				address: user1Circle,
				domainMessage: new JoinCircle(
					user4,
					secretKey
				)
			),
			domainMessagePacker.PackMessage(
				domainMessageId: Guid.NewGuid().ToString("N"),
				address: user1Circle,
				domainMessage: new BetrayCircle(
					user5,
					secretKey
				)
			)
		];
		foreach (var domainMessage in domainMessages) {
			await domainMessageSender.Send(domainMessage);
		}
	}
}

public static class SampleCircleOfTrustSimulationServiceCollectionExtensions {
	public static IHostApplicationBuilder AddSampleCircleOfTrustSimulation(this IHostApplicationBuilder applicationBuilder) {
		applicationBuilder.Services.AddHostedService<SampleCircleOfTrustSimulation>();
		applicationBuilder.TryAddKafkaDomainMessageSender();
		return applicationBuilder;
	}
}
