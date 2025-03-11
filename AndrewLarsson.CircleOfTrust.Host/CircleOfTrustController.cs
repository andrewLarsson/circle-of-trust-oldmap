using AndrewLarsson.CircleOfTrust.Domain;
using developersBliss.OLDMAP.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace AndrewLarsson.CircleOfTrust.Host;

[ApiController]
[Route("api/circle-of-trust")]
public class CircleOfTrustController(IDomainMessageSender domainMessageSender, DomainMessagePacker domainMessagePacker) : ControllerBase {
	[HttpPost("test")]
	public async Task Test(string requestId) {
		var user = Guid.NewGuid().ToString("N")[..5];
		await domainMessageSender.Send(domainMessagePacker.PackMessage(
			domainMessageId: requestId,
			address: CircleAddress.For(user),
			domainMessage: new ClaimCircle($"{user} Test Circle", user, "April Fools!")
		));
	}
}
