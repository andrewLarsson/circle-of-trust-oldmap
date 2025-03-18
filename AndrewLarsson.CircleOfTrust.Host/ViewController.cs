using AndrewLarsson.CircleOfTrust.View;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AndrewLarsson.CircleOfTrust.Host;

[ApiController]
[Route("api/view")]
public class ViewController(
	ViewDbConnection viewDbConnection
) : ControllerBase {

	[HttpPost("leaderboard")]
	public async Task<IEnumerable<CircleStats>> Leaderboard() {
		IEnumerable<CircleStats> leaderboard = await viewDbConnection.QueryAsync<CircleStats>(
			"SELECT * FROM CircleLeaderboard;"
		);
		return leaderboard;
	}

	[HttpPost("circle-stats/{circleId}")]
	public async Task<CircleStats?> CircleStats(string circleId) {
		CircleStats? circleStats = await viewDbConnection.QueryFirstOrDefaultAsync<CircleStats>(
			"SELECT * FROM CircleStats WHERE CircleId = @circleId LIMIT 1;",
			new { circleId }
		);
		return circleStats;
	}

	[Authorize]
	[HttpPost("my-circle-stats")]
	public async Task<CircleStats?> MyCircleStats() {
		string userId = User.UserId();
		CircleStats? circleStats = await viewDbConnection.QueryFirstOrDefaultAsync<CircleStats>(
			"SELECT * FROM CircleStats WHERE Owner = @userId LIMIT 1;",
			new { userId }
		);
		return circleStats;
	}

	[Authorize]
	[HttpPost("my-user-stats")]
	public async Task<UserStats> MyUserStats() {
		string userId = User.UserId();
		UserStats userStats = await viewDbConnection.QueryFirstOrDefaultAsync<UserStats>(
			"SELECT * FROM UserStats WHERE UserId = @userId LIMIT 1;",
			new { userId }
		) ?? new(UserId: userId, CircleId: null, 0, 0, 0);
		return userStats;
	}

	[HttpPost("user-stats/{userId}")]
	public async Task<UserStats> UserStats(string userId) {
		UserStats userStats = await viewDbConnection.QueryFirstOrDefaultAsync<UserStats>(
			"SELECT * FROM UserStats WHERE UserId = @userId LIMIT 1;",
			new { userId }
		) ?? new(UserId: userId, CircleId: null, 0, 0, 0);
		return userStats;
	}
}
