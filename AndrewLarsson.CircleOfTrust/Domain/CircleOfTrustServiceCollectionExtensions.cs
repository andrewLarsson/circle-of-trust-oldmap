using developersBliss.OLDMAP.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace AndrewLarsson.CircleOfTrust.Domain;
public static class CircleOfTrustServiceCollectionExtensions {
	public static IServiceCollection AddCircleOfTrust(this IServiceCollection services) {
		services
			.AddKafkaDomainMessageApplication(Applications.Domain)
			.AddApplicationServiceWithPureStyle<CircleService, Circle>()
			.AddPostgreSqlMartenAggregateRootStorage()
			.TryAddPostgreSqlMartenAggregateRootStore<Circle>()
			.AddTransient<JsonConverter, CircleJsonConverter>()
		;
		return services;
	}
}
