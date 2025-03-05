using AndrewLarsson.CircleOfTrust.Model;
using developersBliss.OLDMAP.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AndrewLarsson.CircleOfTrust.View;
public static class ViewDbServiceCollectionExtensions {
	public static IServiceCollection AddView(this IServiceCollection services) {
		services
			.AddViewDb()
			.AddDomainEventHandler<CircleClaimed, CircleStatsViewHandler>()
			.AddDomainEventHandler<CircleJoined, CircleStatsViewHandler>()
			.AddDomainEventHandler<CircleBetrayed, CircleStatsViewHandler>()
		;
		return services;
	}

	public static IServiceCollection AddViewDb(this IServiceCollection serviceCollection) {
		throw new NotImplementedException();
	}
}
