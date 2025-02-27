using AndrewLarsson.CircleOfTrust.Model;
using developersBliss.OLDMAP.Messaging;
using Microsoft.Extensions.Hosting;

namespace AndrewLarsson.CircleOfTrust;
public class TestCircleOfTrust(IDomainMessageSender domainMessageSender, DomainMessagePacker domainMessagePacker) : BackgroundService {
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
					"User1's Circle",
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
			await domainMessageSender.Send(domainMessage);
		}
	}
}
