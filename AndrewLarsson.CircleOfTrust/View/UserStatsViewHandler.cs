using AndrewLarsson.CircleOfTrust.Domain;
using Dapper;
using developersBliss.OLDMAP.Application;
using developersBliss.OLDMAP.Messaging;

namespace AndrewLarsson.CircleOfTrust.View;
public class UserStatsViewHandler(
	ApplicationContext applicationContext,
	Context<Synchronization> synchronizationContext,
	ViewDbConnection viewDb
) : IDomainEventHandler<CircleClaimed>,
	IDomainEventHandler<CircleJoined>,
	IDomainEventHandler<CircleBetrayed> {
	static readonly string TryInsertUserStats = @"
		INSERT INTO UserStats (UserId, CircleId, MemberOfCircles, MemberOfNonbetrayedCircles, MemberOfBetrayedCircles)
		VALUES (@UserId, NULL, 0, 0, 0)
		ON CONFLICT (UserId) DO NOTHING;
	";
	static readonly string UpdateUserStatsFromCircleClaimedEvent = @"
		UPDATE UserStats SET
			CircleId = @AggregateRootId,
			MemberOfCircles = MemberOfCircles + 1,
			MemberOfNonbetrayedCircles = MemberOfNonbetrayedCircles + 1
		WHERE UserId = @Owner;
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
		return viewDb.ExecuteIdempotentTransactionWithSynchronization(
			transactionId: domainEvent.DomainMessageId,
			application: applicationContext,
			synchronization: synchronizationContext,
			perform: async (transaction) => {
				await viewDb.ExecuteAsync(TryInsertUserStats, new { UserId = domainEvent.Body.Owner }, transaction);
				await viewDb.ExecuteAsync(UpdateUserStatsFromCircleClaimedEvent, param, transaction);
				await viewDb.ExecuteAsync(InsertUserStatsCircleMembersFromCircleClaimedEvent, param, transaction);
			}
		);
	}

	public Task Handle(DomainEvent<CircleJoined> domainEvent) {
		var param = domainEvent.ToParameters();
		return viewDb.ExecuteIdempotentTransactionWithSynchronization(
			transactionId: domainEvent.DomainMessageId,
			application: applicationContext,
			synchronization: synchronizationContext,
			perform: async (transaction) => {
				await viewDb.ExecuteAsync(TryInsertUserStats, new { UserId = domainEvent.Body.Member }, transaction);
				await viewDb.ExecuteAsync(UpdateUserStatsFromCircleJoinedEvent, param, transaction);
				await viewDb.ExecuteAsync(InsertUserStatsCircleMembersFromCircleJoinedEvent, param, transaction);
			}
		);
	}

	public Task Handle(DomainEvent<CircleBetrayed> domainEvent) {
		return viewDb.ExecuteIdempotentTransactionWithSynchronization(
			transactionId: domainEvent.DomainMessageId,
			application: applicationContext,
			synchronization: synchronizationContext,
			sql: UpdateUserStatsFromCircleBetrayedEvent,
			param: domainEvent.ToParameters()
		);
	}
}
