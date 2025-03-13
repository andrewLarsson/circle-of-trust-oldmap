using AndrewLarsson.CircleOfTrust.Domain;
using developersBliss.OLDMAP.Messaging;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace AndrewLarsson.CircleOfTrust.Host;

[ApiController]
[Route("api/circle-of-trust")]
public class CircleOfTrustController(
	IConfiguration configuration,
	IDomainRequester domainRequester,
	DomainMessagePacker domainMessagePacker
) : ControllerBase {
	readonly byte[] userTokenHmacKey = Encoding.UTF8.GetBytes(
		configuration.GetValue<string>("UserTokenHmacKey")
		?? throw new Exception("UserTokenHmacKey missing from configuration.")
	);

	[HttpPost("sign-in")]
	public IActionResult SignIn(string userId) { // TODO Use real authentication.
		if (userId.Contains('.')) {
			return BadRequest("Invalid User ID. Must be alphanumeric.");
		}
		using var hmacSha256 = new HMACSHA256(userTokenHmacKey);
		byte[] signatureBytes = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(userId));
		string signature = Convert.ToHexString(signatureBytes).ToLower();
		string userToken = $"{userId}.{signature}";
		return Ok(userToken);
	}

	string AuthenticateUser(string userToken) { // TODO Move this to an authorization filter and use claims.
		var pieces = userToken.Split('.');
		var userId = pieces[0];
		var signature = pieces[1];
		using var hmacSha256 = new HMACSHA256(userTokenHmacKey);
		byte[] checkSignatureBytes = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(userId));
		string checkSignature = Convert.ToHexString(checkSignatureBytes).ToLower();
		if (signature != checkSignature) {
			throw new Exception("Invalid user token.");
		}
		return userId;
	}

	[HttpPost("claim-circle")]
	public async Task<PackedDomainEvent> ClaimCircle(string requestId, string userToken, string title, string secretKey) {
		string userId = AuthenticateUser(userToken);
		PackedDomainEvent response = await domainRequester.Request(domainMessagePacker.PackMessage(
			domainMessageId: requestId,
			address: CircleAddress.For(userId),
			domainMessage: new ClaimCircle(title, userId, secretKey)
		));
		return response;
	}

	[HttpPost("join-circle")]
	public async Task<PackedDomainEvent> JoinCircle(string requestId, string userToken, string circleId, string secretKey) {
		string userId = AuthenticateUser(userToken);
		PackedDomainEvent response = await domainRequester.Request(domainMessagePacker.PackMessage(
			domainMessageId: requestId,
			address: CircleAddress.For(circleId),
			domainMessage: new JoinCircle(userId, secretKey)
		));
		return response;
	}

	[HttpPost("betray-circle")]
	public async Task<PackedDomainEvent> BetrayCircle(string requestId, string userToken, string circleId, string secretKey) {
		string userId = AuthenticateUser(userToken);
		PackedDomainEvent response = await domainRequester.Request(domainMessagePacker.PackMessage(
			domainMessageId: requestId,
			address: CircleAddress.For(circleId),
			domainMessage: new BetrayCircle(userId, secretKey)
		));
		return response;
	}
}
