namespace AndrewLarsson.CircleOfTrust;
public class CircleService {
	public (Circle, ClaimCircleEvent) ClaimCircle(
		ClaimCircle message
	) {
		var (circle, @event) = Circle.Claim(message);
		return (circle, @event);
	}

	public (Circle, JoinCircleEvent) JoinCircle(
		Circle circle,
		JoinCircle message
	) {
		var @event = circle.Join(message);
		return (circle, @event);
	}

	public (Circle, BetrayCircleEvent) BetrayCircle(
		Circle circle,
		BetrayCircle message
	) {
		var @event = circle.Betray(message);
		return (circle, @event);
	}
}
