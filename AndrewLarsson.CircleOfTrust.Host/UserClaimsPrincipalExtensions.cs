using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AndrewLarsson.CircleOfTrust.Host;
public static class UserClaimsPrincipalExtensions {
	public static string UserId(this ClaimsPrincipal user) {
		var email = user.Claims.First(x => x.Type == ClaimTypes.Email).Value;
		var userId = email.HashWithSHA256()[..16];
		return userId;
	}

	public static string HashWithSHA256(this string value) {
		var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
		return Convert.ToHexString(hash).ToLower();
	}
}
