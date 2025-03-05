using AndrewLarsson.CircleOfTrust.Model;
using developersBliss.OLDMAP.Messaging;
using System.Data;

namespace AndrewLarsson.CircleOfTrust.View;
public class CircleStatsViewHandler(IDbConnection dbConnection) :
	IDomainEventHandler<CircleClaimed>,
	IDomainEventHandler<CircleJoined>,
	IDomainEventHandler<CircleBetrayed> {
	static readonly string TransactionContext = "CircleStats";
	// These are SQL Server queries. Change them to PostgreSQL and actually create the schema.
	static readonly string InsertCircleStatsFromCircleClaimedEvent = @"INSERT INTO CircleStats ([CircleId], [Title], [Owner], [IsBetrayed], [Members]) VALUES (@AggregateRootId, @Title, @Owner, 0, 0);";
	static readonly string UpdateCircleStatsFromCircleJoinedEvent = @"UPDATE CircleStats SET [Members] = [Members] + 1 WHERE [CircleId] = @AggregateRootId;";
	static readonly string UpdateCircleStatsFromCircleBetrayedEvent = @"UPDATE CircleStats SET [IsBetrayed] = 1 WHERE [CircleId] = @AggregateRootId;";

	public Task Handle(DomainEvent<CircleClaimed> domainEvent) {
		return dbConnection.ExecuteIdempotentTransaction(
			transactionContext: TransactionContext,
			transactionId: domainEvent.DomainMessageId,
			sql: InsertCircleStatsFromCircleClaimedEvent,
			param: domainEvent.ToParameters()
		);
	}

	public Task Handle(DomainEvent<CircleJoined> domainEvent) {
		return dbConnection.ExecuteIdempotentTransaction(
			transactionContext: TransactionContext,
			transactionId: domainEvent.DomainMessageId,
			sql: UpdateCircleStatsFromCircleJoinedEvent,
			param: domainEvent.ToParameters()
		);
	}

	public Task Handle(DomainEvent<CircleBetrayed> domainEvent) {
		return dbConnection.ExecuteIdempotentTransaction(
			transactionContext: TransactionContext,
			transactionId: domainEvent.DomainMessageId,
			sql: UpdateCircleStatsFromCircleBetrayedEvent,
			param: domainEvent.ToParameters()
		);
	}
}
