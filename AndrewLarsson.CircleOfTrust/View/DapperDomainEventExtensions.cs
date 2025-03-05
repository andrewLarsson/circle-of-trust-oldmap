using Dapper;
using developersBliss.OLDMAP.Messaging;

namespace AndrewLarsson.CircleOfTrust.View;
public static class DapperDomainEventExtensions {
	public static DynamicParameters ToParameters<TDomainEvent>(this DomainEvent<TDomainEvent> domainEvent)
		where TDomainEvent : notnull {
		DynamicParameters parameters = new(domainEvent);
		parameters.AddDynamicParams(domainEvent.Address);
		parameters.AddDynamicParams(domainEvent.Body);
		return parameters;
	}
}
