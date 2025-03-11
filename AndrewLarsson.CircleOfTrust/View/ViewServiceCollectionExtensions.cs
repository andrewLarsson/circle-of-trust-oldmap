using AndrewLarsson.CircleOfTrust.Domain;
using developersBliss.OLDMAP.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace AndrewLarsson.CircleOfTrust.View;
public static class ViewServiceCollectionExtensions {
	public static IServiceCollection AddCircleOfTrustView(this IServiceCollection services) {
		services
			.AddCircleOfTrustViewDb()
			.AddCircleOfTrustViewCircleStats()
			.AddCircleOfTrustViewUserStats()
		;
		return services;
	}

	public static IServiceCollection AddCircleOfTrustViewCircleStats(this IServiceCollection services) {
		services
			.AddKafkaDomainEventApplication(Applications.ViewCircleStats)
			.AddDomainEventHandler<CircleClaimed, CircleStatsViewHandler>(Applications.ViewCircleStats)
			.AddDomainEventHandler<CircleJoined, CircleStatsViewHandler>(Applications.ViewCircleStats)
			.AddDomainEventHandler<CircleBetrayed, CircleStatsViewHandler>(Applications.ViewCircleStats)
		;
		return services;
	}

	public static IServiceCollection AddCircleOfTrustViewUserStats(this IServiceCollection services) {
		services
			.AddKafkaDomainEventApplication(Applications.ViewUserStats)
			.AddDomainEventHandler<CircleClaimed, UserStatsViewHandler>(Applications.ViewUserStats)
			.AddDomainEventHandler<CircleJoined, UserStatsViewHandler>(Applications.ViewUserStats)
			.AddDomainEventHandler<CircleBetrayed, UserStatsViewHandler>(Applications.ViewUserStats)
		;
		return services;
	}

	public static IServiceCollection AddCircleOfTrustViewDb(this IServiceCollection services) {
		services.AddScoped(serviceProvider => {
			var configuration = serviceProvider.GetRequiredService<IConfiguration>();
			var connectionString = configuration.GetConnectionString("ViewDbPostgreSql");
			NpgsqlConnection postgreSqlConnection = new(connectionString);
			if (postgreSqlConnection.State != System.Data.ConnectionState.Open) {
				postgreSqlConnection.Open();
			}
			ViewDbConnection viewDbConnection = new(postgreSqlConnection);
			return viewDbConnection;
		});
		return services;
	}
}
