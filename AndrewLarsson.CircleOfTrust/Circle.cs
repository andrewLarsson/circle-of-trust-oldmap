namespace AndrewLarsson.CircleOfTrust;
public class Circle(string title, string owner, string secretKey) {
	public string Title { get; } = title;
	public string Owner { get; } = owner;
	public string SecretKey { get; } = secretKey;

	public static (Circle, ClaimCircleEvent) Claim(ClaimCircle message) {
		//await _circlesMustHaveAUniqueNameRule.Verify(name);
		//await _playersMayOnlyInitiateOneCircleRule.Verify(playerId);
		var circle = new Circle(
			title: message.Title,
			owner: message.Owner,
			secretKey: message.SecretKey
		);
		return (circle, new CircleClaimed(
			Title: circle.Title,
			Owner: circle.Owner
		));
	}

	public JoinCircleEvent Join(JoinCircle message) {
		//await _circleKeyMustBeValidInOrderToJoinOrBetrayCircleRule.Verify(circleId, key);
		//await _playersMayNotJoinOrBetrayCircleThatHasBeenBetrayedRule.Verify(circleId);
		//await _playersMayNotJoinOrBetrayTheirOwnCircleRule.Verify(circleId, playerId);
		//await _playersMayOnlyJoinACircleOnceRule.Verify(circleId, playerId);
		return new CircleJoined();
	}

	public BetrayCircleEvent Betray(BetrayCircle message) {
		//await _circleKeyMustBeValidInOrderToJoinOrBetrayCircleRule.Verify(circleId, key);
		//await _playersMayNotJoinOrBetrayCircleThatHasBeenBetrayedRule.Verify(circleId);
		//await _playersMayNotJoinOrBetrayTheirOwnCircleRule.Verify(circleId, playerId);
		//await _playersMayNotBetrayCircleTheyAreAMemberOfRule.Verify(circleId, playerId
		return new CircleBetrayed();
	}
}

public record ClaimCircle(string Title, string Owner, string SecretKey);
public interface ClaimCircleEvent;
public record CircleClaimed(string Title, string Owner) : ClaimCircleEvent;

public record JoinCircle();
public interface JoinCircleEvent;
public record CircleJoined() : JoinCircleEvent;

public record BetrayCircle();
public interface BetrayCircleEvent;
public record CircleBetrayed() : BetrayCircleEvent;
