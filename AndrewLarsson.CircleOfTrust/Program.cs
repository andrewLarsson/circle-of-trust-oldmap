using AndrewLarsson.CircleOfTrust;
using developersBliss.OLDMAP.Hosting;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder applicationBuilder = Host.CreateApplicationBuilder();
applicationBuilder.Services
	.AddOLDMAP()
	.AddKafkaDomainMessageApplication()
	.AddKafkaDomainEventApplication()
	.AddPostgreSqlMartenAggregateRootStorage()
	.TryAddPostgreSqlMartenAggregateRootStore<Circle>()
	.AddApplicationServiceWithPureStyle<CircleService, Circle>()
;
IHost host = applicationBuilder.Build();
await host.RunAsync();
