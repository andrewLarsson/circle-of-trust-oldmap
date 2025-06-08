using AndrewLarsson.CircleOfTrust.Domain;
using developersBliss.OLDMAP.Application;
using developersBliss.OLDMAP.Hosting;
using developersBliss.OLDMAP.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace AndrewLarsson.CircleOfTrust.View;
public static class HostApplicationBuilderExtensions {
	public static IHostApplicationBuilder AddCircleOfTrustView(this IHostApplicationBuilder applicationBuilder) {
		applicationBuilder
			.AddCircleOfTrustViewDb()
			.AddCircleOfTrustViewDefaultEventHandler()
			.AddCircleOfTrustViewCircleStats()
			.AddCircleOfTrustViewUserStats()
			.AddCircleOfTrustViewDapperSynchronizationObserver()
		;
		return applicationBuilder;
	}

	public static IHostApplicationBuilder AddCircleOfTrustViewDefaultEventHandler(this IHostApplicationBuilder applicationBuilder) {
		applicationBuilder.Services.AddScoped<IDefaultDomainEventHandler, CircleOfTrustViewDefaultHandler>();
		return applicationBuilder;
	}

	public static IHostApplicationBuilder AddCircleOfTrustViewCircleStats(this IHostApplicationBuilder applicationBuilder) {
		applicationBuilder
			.AddKafkaDomainEventApplication(Applications.CircleStatsView)
			.AddDomainEventHandler<CircleClaimed, CircleStatsViewHandler>(Applications.CircleStatsView)
			.AddDomainEventHandler<CircleJoined, CircleStatsViewHandler>(Applications.CircleStatsView)
			.AddDomainEventHandler<CircleBetrayed, CircleStatsViewHandler>(Applications.CircleStatsView)
		;
		return applicationBuilder;
	}

	public static IHostApplicationBuilder AddCircleOfTrustViewUserStats(this IHostApplicationBuilder applicationBuilder) {
		applicationBuilder
			.AddKafkaDomainEventApplication(Applications.UserStatsView)
			.AddDomainEventHandler<CircleClaimed, UserStatsViewHandler>(Applications.UserStatsView)
			.AddDomainEventHandler<CircleJoined, UserStatsViewHandler>(Applications.UserStatsView)
			.AddDomainEventHandler<CircleBetrayed, UserStatsViewHandler>(Applications.UserStatsView)
		;
		return applicationBuilder;
	}

	public static IHostApplicationBuilder AddCircleOfTrustViewDb(this IHostApplicationBuilder applicationBuilder) {
		applicationBuilder.Services.AddScoped(serviceProvider => {
			var configuration = serviceProvider.GetRequiredService<IConfiguration>();
			var connectionString = configuration.GetConnectionString(ViewDbConnection.ConnectionStringName);
			NpgsqlConnection postgreSqlConnection = new(connectionString);
			if (postgreSqlConnection.State != System.Data.ConnectionState.Open) {
				postgreSqlConnection.Open();
			}
			ViewDbConnection viewDbConnection = new(postgreSqlConnection);
			return viewDbConnection;
		});
		return applicationBuilder;
	}

	public static IHostApplicationBuilder AddCircleOfTrustViewDapperSynchronizationObserver(this IHostApplicationBuilder applicationBuilder) {
		applicationBuilder.Services.AddSingleton<ISynchronizationObserver>(serviceProvider => {
			var configuration = serviceProvider.GetRequiredService<IConfiguration>();
			var connectionString = configuration.GetConnectionString(ViewDbConnection.ConnectionStringName);
			NpgsqlConnection postgreSqlConnection = new(connectionString);
			if (postgreSqlConnection.State != System.Data.ConnectionState.Open) {
				postgreSqlConnection.Open();
			}
			DapperSynchronizationObserver dapperSynchronizationObserver = new(postgreSqlConnection);
			return dapperSynchronizationObserver;
		});
		return applicationBuilder;
	}
}
