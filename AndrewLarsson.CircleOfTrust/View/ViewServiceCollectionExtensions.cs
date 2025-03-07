using AndrewLarsson.CircleOfTrust.Domain;
using AndrewLarsson.CircleOfTrust.Infrastructure;
using developersBliss.OLDMAP.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace AndrewLarsson.CircleOfTrust.View;
public static class ViewServiceCollectionExtensions {
	public static IServiceCollection AddCircleOfTrustView(this IServiceCollection services) {
		services
			.AddCircleOfTrustViewDb()
			.AddKafkaDomainEventApplication(Applications.View)
			.AddDomainEventHandler<CircleClaimed, CircleStatsViewHandler>(Applications.View)
			.AddDomainEventHandler<CircleJoined, CircleStatsViewHandler>(Applications.View)
			.AddDomainEventHandler<CircleBetrayed, CircleStatsViewHandler>(Applications.View)
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
