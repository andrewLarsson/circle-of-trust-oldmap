using Microsoft.AspNetCore.Mvc;

namespace AndrewLarsson.CircleOfTrust.Host;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase {
	[HttpGet]
	public string GetHealth() {
		return "Healthy";
	}
}
