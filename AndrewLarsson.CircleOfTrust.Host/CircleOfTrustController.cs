using AndrewLarsson.CircleOfTrust.Domain;
using developersBliss.OLDMAP.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace AndrewLarsson.CircleOfTrust.Host;

[ApiController]
[Route("api/circle-of-trust")]
public class CircleOfTrustController(IDomainRequester domainRequester, DomainMessagePacker domainMessagePacker) : ControllerBase {
	[HttpPost("test-with-response")]
	public async Task<object> TestWithResponse(string requestId) {
		var user = Guid.NewGuid().ToString("N")[..5];
		object response = await domainRequester.Request(domainMessagePacker.PackMessage(
			domainMessageId: requestId,
			address: CircleAddress.For(user),
			domainMessage: new ClaimCircle($"{user} Test Circle", user, "April Fools!")
		));
		return response;
	}
}
