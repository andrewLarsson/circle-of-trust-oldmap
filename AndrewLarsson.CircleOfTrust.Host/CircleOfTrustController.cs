using AndrewLarsson.CircleOfTrust.Domain;
using developersBliss.OLDMAP.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AndrewLarsson.CircleOfTrust.Host;

[ApiController]
[Route("api/circle-of-trust")]
public class CircleOfTrustController(
	IDomainRequester domainRequester,
	DomainMessagePacker domainMessagePacker
) : ControllerBase {

	[Authorize]
	[HttpPost("claim-circle")]
	public async Task<PackedDomainEvent> ClaimCircle(string requestId, string title, string secretKey) {
		string userId = User.UserId();
		PackedDomainEvent response = await domainRequester.Request(domainMessagePacker.PackMessage(
			domainMessageId: requestId,
			address: CircleAddress.For(userId),
			domainMessage: new ClaimCircle(title, userId, secretKey)
		));
		return response;
	}

	[Authorize]
	[HttpPost("join-circle")]
	public async Task<PackedDomainEvent> JoinCircle(string requestId, string circleId, string secretKey) {
		string userId = User.UserId();
		PackedDomainEvent response = await domainRequester.Request(domainMessagePacker.PackMessage(
			domainMessageId: requestId,
			address: CircleAddress.For(circleId),
			domainMessage: new JoinCircle(userId, secretKey)
		));
		return response;
	}

	[Authorize]
	[HttpPost("betray-circle")]
	public async Task<PackedDomainEvent> BetrayCircle(string requestId, string circleId, string secretKey) {
		string userId = User.UserId();
		PackedDomainEvent response = await domainRequester.Request(domainMessagePacker.PackMessage(
			domainMessageId: requestId,
			address: CircleAddress.For(circleId),
			domainMessage: new BetrayCircle(userId, secretKey)
		));
		return response;
	}
}
