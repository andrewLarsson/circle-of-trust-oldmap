using developersBliss.OLDMAP.Application;
using developersBliss.OLDMAP.Messaging;

namespace AndrewLarsson.CircleOfTrust.View;
public class CircleOfTrustViewDefaultHandler(
	ApplicationContext applicationContext,
	Context<Synchronization> synchronizationContext,
	ViewDbConnection viewDb
) : IDefaultDomainEventHandler {
	public Task Handle(PackedDomainEvent packedDomainEvent) {
		return viewDb.ExecuteIdempotentTransactionWithSynchronization(
			application: applicationContext._.Name,
			transactionId: packedDomainEvent.DomainMessageId,
			synchronization: synchronizationContext,
			async (transaction) => { }
		);
	}
}
