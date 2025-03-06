using AndrewLarsson.CircleOfTrust.Infrastructure;
using AndrewLarsson.CircleOfTrust.Model;
using developersBliss.OLDMAP.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

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

	public static IServiceCollection AddViewDb(this IServiceCollection services) {
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
