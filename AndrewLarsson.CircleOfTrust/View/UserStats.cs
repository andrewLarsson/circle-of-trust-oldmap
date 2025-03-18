namespace AndrewLarsson.CircleOfTrust.View;
public record UserStats(string UserId, string? CircleId, int MemberOfCircles, int MemberOfNonbetrayedCircles, int MemberOfBetrayedCircles);
