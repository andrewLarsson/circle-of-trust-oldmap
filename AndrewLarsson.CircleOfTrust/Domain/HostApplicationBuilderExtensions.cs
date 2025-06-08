using developersBliss.OLDMAP.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;

namespace AndrewLarsson.CircleOfTrust.Domain;
public static class HostApplicationBuilderExtensions {
	public static IHostApplicationBuilder AddCircleOfTrust(this IHostApplicationBuilder applicationBuilder) {
		applicationBuilder.AddKafkaDomainMessageApplication(Applications.Domain);
		applicationBuilder.Services
			.AddApplicationServiceWithPureStyle<CircleService, Circle>()
			.AddPostgreSqlMartenAggregateRootStorage()
			.TryAddPostgreSqlMartenAggregateRootStore<Circle>()
			.AddTransient<JsonConverter, CircleJsonConverter>()
		;
		return applicationBuilder;
	}
}
