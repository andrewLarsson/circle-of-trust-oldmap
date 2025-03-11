using developersBliss.OLDMAP.Messaging;

namespace AndrewLarsson.CircleOfTrust.Domain;
public static class CircleAddress {
	public static AggregateRootAddress For(string CircleId) => new() {
		Domain = "AndrewLarsson.CircleOfTrust",
		AggregateRoot = "Circle",
		AggregateRootId = CircleId
	};
}
