using AndrewLarsson.CircleOfTrust.Application;
using AndrewLarsson.CircleOfTrust.Infrastructure;
using AndrewLarsson.CircleOfTrust.Model;
using developersBliss.OLDMAP.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace AndrewLarsson.CircleOfTrust.Hosting;
public static class CircleOfTrustServiceCollectionExtensions {
	public static IServiceCollection AddCircleOfTrust(this IServiceCollection services) {
		services
			.AddApplicationServiceWithPureStyle<CircleService, Circle>()
			.TryAddPostgreSqlMartenAggregateRootStore<Circle>()
			.AddTransient<JsonConverter, CircleJsonConverter>()
		;
		return services;
	}
}
