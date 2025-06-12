using developersBliss.OLDMAP.Hosting;

namespace AndrewLarsson.CircleOfTrust.Host;
public static class HostApplicationBuilderExtensions {
	public static IHostApplicationBuilder AddCircleOfTrustHost(this IHostApplicationBuilder applicationBuilder) {
		applicationBuilder.AddKafkaDomainRequester();
		applicationBuilder.Services.AddSynchronizer();
		return applicationBuilder;
	}
}
