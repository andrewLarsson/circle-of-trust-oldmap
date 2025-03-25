using developersBliss.OLDMAP.Messaging;

namespace AndrewLarsson.CircleOfTrust.Host.Infrastructure;

public static class DomainRequesterHttpExtensions {
	public static async Task<PackedDomainEvent> Request(this IDomainRequester domainRequester, PackedDomainMessage packedDomainMessage, HttpResponse httpResponse, CancellationToken cancellationToken = default) {
		(PackedDomainEvent response, string synchronizationToken) = await domainRequester.RequestWithSynchronization(packedDomainMessage, cancellationToken);
		httpResponse.Headers.Append(Headers.SynchronizationToken, synchronizationToken);
		return response;
	}
}
