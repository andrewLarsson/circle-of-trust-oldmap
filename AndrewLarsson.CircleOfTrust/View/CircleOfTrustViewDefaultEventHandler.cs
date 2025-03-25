using developersBliss.OLDMAP.Application;
using developersBliss.OLDMAP.Messaging;

namespace AndrewLarsson.CircleOfTrust.View;
public class CircleOfTrustViewDefaultHandler(
	ApplicationContext applicationContext,
	Context<Synchronization> synchronizationContext,
	ViewDbConnection viewDb
) : IDefaultDomainEventHandler {
	public Task Handle(PackedDomainEvent packedDomainEvent) {
		return viewDb.ExecuteIdempotentTransaction(
			transactionId: packedDomainEvent.DomainMessageId,
			application: applicationContext,
			synchronization: synchronizationContext,
			perform: async (transaction) => { }
		);
	}
}
