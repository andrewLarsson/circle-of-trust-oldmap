using developersBliss.OLDMAP.Hosting;

namespace AndrewLarsson.CircleOfTrust.Host;
public static class CircleOfTrustHostServiceCollectionExtensions {
	public static IServiceCollection AddCircleOfTrustHost(this IServiceCollection services) {
		services
			.AddKafkaDomainRequester()
		;
		return services;
	}
}
