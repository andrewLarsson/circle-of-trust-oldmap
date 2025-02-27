namespace AndrewLarsson.CircleOfTrust.Model;
public class Circle {
	public string Title { get; }
	public string Owner { get; }
	public Lock Lock { get; }
	public Members Members { get; }
	public bool Betrayed { get; private set; }

	private Circle(string title, string owner, string secretKey, IEnumerable<string> members, bool betrayed) {
		Title = title;
		Owner = owner;
		Lock = new Lock(secretKey);
		Members = new Members(members);
		Betrayed = betrayed;
	}

	public static (Circle, ICircleEvent) Claim(ClaimCircle message) {
		Circle circle = new(
			title: message.Title,
			owner: message.User,
			secretKey: message.SecretKey,
			members: [message.User],
			betrayed: false
		);
		return (circle, new CircleClaimed(
			Title: circle.Title,
			Owner: circle.Owner
		));
	}

	public ICircleEvent Join(JoinCircle message) {
		if (Betrayed) {
			return new CircleAlreadyBetrayed();
		}
		if (!Lock.Unlock(message.Key)) {
			return new KeyDoesNotUnlockCircle();
		}
		if (Members.IsMember(message.User)) {
			return new UserAlreadyMemberOfCircle(message.User);
		}
		Members.Join(message.User);
		return new CircleJoined(Member: message.User);
	}

	public ICircleEvent Betray(BetrayCircle message) {
		if (Betrayed) {
			return new CircleAlreadyBetrayed();
		}
		if (!Lock.Unlock(message.Key)) {
			return new KeyDoesNotUnlockCircle();
		}
		if (Members.IsMember(message.User)) {
			return new UserAlreadyMemberOfCircle(message.User);
		}
		Betrayed = true;
		return new CircleBetrayed(Betrayed);
	}
}

public class Lock(string secretKey) {
	readonly string secretKey = secretKey;

	public bool Unlock(string key) {
		return key == secretKey;
	}
}

public class Members(IEnumerable<string> members) {
	readonly HashSet<string> members = [.. members];

	public bool IsMember(string user) {
		return members.Contains(user);
	}

	public void Join(string user) {
		members.Add(user);
	}
}

public interface ICircleEvent;

public record ClaimCircle(string Title, string User, string SecretKey);
public record CircleClaimed(string Title, string Owner) : ICircleEvent;

public record JoinCircle(string User, string Key);
public record CircleJoined(string Member) : ICircleEvent;

public record BetrayCircle(string User, string Key);
public record CircleBetrayed(bool Betrayed) : ICircleEvent;

public record KeyDoesNotUnlockCircle() : ICircleEvent;
public record CircleAlreadyBetrayed() : ICircleEvent;
public record UserAlreadyMemberOfCircle(string User) : ICircleEvent;
