using AndrewLarsson.CircleOfTrust.Model;

namespace AndrewLarsson.CircleOfTrust.Application;
public class CircleService {
	public (Circle, ICircleEvent) ClaimCircle(
		ClaimCircle message
	) {
		var (circle, @event) = Circle.Claim(message);
		return (circle, @event);
	}

	public (Circle, ICircleEvent) JoinCircle(
		Circle circle,
		JoinCircle message
	) {
		var @event = circle.Join(message);
		return (circle, @event);
	}

	public (Circle, ICircleEvent) BetrayCircle(
		Circle circle,
		BetrayCircle message
	) {
		var @event = circle.Betray(message);
		return (circle, @event);
	}
}
