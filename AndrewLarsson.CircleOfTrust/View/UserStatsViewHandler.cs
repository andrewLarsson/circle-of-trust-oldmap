using AndrewLarsson.CircleOfTrust.Domain;
using AndrewLarsson.CircleOfTrust.Infrastructure;
using Dapper;
using developersBliss.OLDMAP.Messaging;

namespace AndrewLarsson.CircleOfTrust.View;
public class UserStatsViewHandler(ViewDbConnection viewDb) :
	IDomainEventHandler<CircleClaimed>,
	IDomainEventHandler<CircleJoined>,
	IDomainEventHandler<CircleBetrayed> {
	static readonly string TransactionContext = "UserStats";
	static readonly string InsertUserStatsFromCircleClaimedEvent = @"
		INSERT INTO UserStats (UserId, CircleId, MemberOfCircles, MemberOfNonbetrayedCircles, MemberOfBetrayedCircles)
		VALUES (@Owner, @AggregateRootId, 1, 1, 0);
	";
	static readonly string InsertUserStatsCircleMembersFromCircleClaimedEvent = @"
		INSERT INTO UserStatsCircleMembers (UserIdCircleId, UserId, CircleId)
		VALUES (CONCAT(@Owner, '|', @AggregateRootId), @Owner, @AggregateRootId);
	";
	static readonly string UpdateUserStatsFromCircleJoinedEvent = @"
		UPDATE UserStats SET
			MemberOfCircles = MemberOfCircles + 1,
			MemberOfNonbetrayedCircles = MemberOfNonbetrayedCircles + 1
		WHERE UserId = @Member;
	";
	static readonly string InsertUserStatsCircleMembersFromCircleJoinedEvent = @"
		INSERT INTO UserStatsCircleMembers (UserIdCircleId, UserId, CircleId)
		VALUES (CONCAT(@Member, '|', @AggregateRootId), @Member, @AggregateRootId);
	";
	static readonly string UpdateUserStatsFromCircleBetrayedEvent = @"
		UPDATE UserStats SET
			MemberOfNonbetrayedCircles = MemberOfNonbetrayedCircles - 1,
			MemberOfBetrayedCircles = MemberOfBetrayedCircles + 1
		FROM UserStatsCircleMembers
		WHERE UserStats.UserId = UserStatsCircleMembers.UserId
		AND UserStatsCircleMembers.CircleId = @AggregateRootId;
	";

	public Task Handle(DomainEvent<CircleClaimed> domainEvent) {
		var param = domainEvent.ToParameters();
		return viewDb.ExecuteIdempotentTransaction(
			transactionContext: TransactionContext,
			transactionId: domainEvent.DomainMessageId,
			async (transaction) => {
				await viewDb.ExecuteAsync(InsertUserStatsFromCircleClaimedEvent, param, transaction);
				await viewDb.ExecuteAsync(InsertUserStatsCircleMembersFromCircleClaimedEvent, param, transaction);
			}
		);
	}

	public Task Handle(DomainEvent<CircleJoined> domainEvent) {
		var param = domainEvent.ToParameters();
		return viewDb.ExecuteIdempotentTransaction(
			transactionContext: TransactionContext,
			transactionId: domainEvent.DomainMessageId,
			async (transaction) => {
				await viewDb.ExecuteAsync(UpdateUserStatsFromCircleJoinedEvent, param, transaction);
				await viewDb.ExecuteAsync(InsertUserStatsCircleMembersFromCircleJoinedEvent, param, transaction);
			}
		);
	}

	public Task Handle(DomainEvent<CircleBetrayed> domainEvent) {
		return viewDb.ExecuteIdempotentTransaction(
			transactionContext: TransactionContext,
			transactionId: domainEvent.DomainMessageId,
			sql: UpdateUserStatsFromCircleBetrayedEvent,
			param: domainEvent.ToParameters()
		);
	}
}
