using AndrewLarsson.CircleOfTrust.Domain;
using developersBliss.OLDMAP.Messaging;

namespace AndrewLarsson.CircleOfTrust.View;
public class CircleStatsViewHandler(ViewDbConnection viewDb) :
	IDomainEventHandler<CircleClaimed>,
	IDomainEventHandler<CircleJoined>,
	IDomainEventHandler<CircleBetrayed> {
	static readonly string TransactionContext = "CircleStats";
	static readonly string InsertCircleStatsFromCircleClaimedEvent = @"
		INSERT INTO CircleStats (CircleId, Title, Owner, IsBetrayed, Members)
		VALUES (@AggregateRootId, @Title, @Owner, FALSE, 1);
	";
	static readonly string UpdateCircleStatsFromCircleJoinedEvent = @"
		UPDATE CircleStats SET
			Members = Members + 1
		WHERE CircleId = @AggregateRootId;
	";
	static readonly string UpdateCircleStatsFromCircleBetrayedEvent = @"
		UPDATE CircleStats SET
			IsBetrayed = TRUE
		WHERE CircleId = @AggregateRootId;
	";

	public Task Handle(DomainEvent<CircleClaimed> domainEvent) {
		return viewDb.ExecuteIdempotentTransaction(
			transactionContext: TransactionContext,
			transactionId: domainEvent.DomainMessageId,
			sql: InsertCircleStatsFromCircleClaimedEvent,
			param: domainEvent.ToParameters()
		);
	}

	public Task Handle(DomainEvent<CircleJoined> domainEvent) {
		return viewDb.ExecuteIdempotentTransaction(
			transactionContext: TransactionContext,
			transactionId: domainEvent.DomainMessageId,
			sql: UpdateCircleStatsFromCircleJoinedEvent,
			param: domainEvent.ToParameters()
		);
	}

	public Task Handle(DomainEvent<CircleBetrayed> domainEvent) {
		return viewDb.ExecuteIdempotentTransaction(
			transactionContext: TransactionContext,
			transactionId: domainEvent.DomainMessageId,
			sql: UpdateCircleStatsFromCircleBetrayedEvent,
			param: domainEvent.ToParameters()
		);
	}
}
