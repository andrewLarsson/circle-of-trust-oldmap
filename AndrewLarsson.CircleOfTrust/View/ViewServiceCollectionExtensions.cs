using AndrewLarsson.CircleOfTrust.Domain;
using developersBliss.OLDMAP.Application;
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
			.AddCircleOfTrustViewDapperSynchronizationObserver()
		;
		return services;
	}

	public static IServiceCollection AddCircleOfTrustViewCircleStats(this IServiceCollection services) {
		services
			.AddKafkaDomainEventApplication(Applications.CircleStatsView)
			.AddDomainEventHandler<CircleClaimed, CircleStatsViewHandler>(Applications.CircleStatsView)
			.AddDomainEventHandler<CircleJoined, CircleStatsViewHandler>(Applications.CircleStatsView)
			.AddDomainEventHandler<CircleBetrayed, CircleStatsViewHandler>(Applications.CircleStatsView)
		;
		return services;
	}

	public static IServiceCollection AddCircleOfTrustViewUserStats(this IServiceCollection services) {
		services
			.AddKafkaDomainEventApplication(Applications.UserStatsView)
			.AddDomainEventHandler<CircleClaimed, UserStatsViewHandler>(Applications.UserStatsView)
			.AddDomainEventHandler<CircleJoined, UserStatsViewHandler>(Applications.UserStatsView)
			.AddDomainEventHandler<CircleBetrayed, UserStatsViewHandler>(Applications.UserStatsView)
		;
		return services;
	}

	public static IServiceCollection AddCircleOfTrustViewDb(this IServiceCollection services) {
		services.AddScoped(serviceProvider => {
			var configuration = serviceProvider.GetRequiredService<IConfiguration>();
			var connectionString = configuration.GetConnectionString(ViewDbConnection.ConnectionStringName);
			NpgsqlConnection postgreSqlConnection = new(connectionString);
			if (postgreSqlConnection.State != System.Data.ConnectionState.Open) {
				postgreSqlConnection.Open();
			}
			ViewDbConnection viewDbConnection = new(postgreSqlConnection);
			return viewDbConnection;
		});
		return services;
	}

	public static IServiceCollection AddCircleOfTrustViewDapperSynchronizationObserver(this IServiceCollection services) {
		services.AddSingleton<ISynchronizationObserver>(serviceProvider => {
			var configuration = serviceProvider.GetRequiredService<IConfiguration>();
			var connectionString = configuration.GetConnectionString(ViewDbConnection.ConnectionStringName);
			NpgsqlConnection postgreSqlConnection = new(connectionString);
			if (postgreSqlConnection.State != System.Data.ConnectionState.Open) {
				postgreSqlConnection.Open();
			}
			DapperSynchronizationObserver dapperSynchronizationObserver = new(postgreSqlConnection);
			return dapperSynchronizationObserver;
		});
		return services;
	}
}
