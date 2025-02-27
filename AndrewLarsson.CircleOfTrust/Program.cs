using AndrewLarsson.CircleOfTrust;
using AndrewLarsson.CircleOfTrust.Hosting;
using developersBliss.OLDMAP.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder applicationBuilder = Host.CreateApplicationBuilder();
applicationBuilder.Services
	.AddOLDMAP()
	.AddKafkaDomainMessageApplication()
	.AddKafkaDomainEventApplication()
	.AddPostgreSqlMartenAggregateRootStorage()
	.AddCircleOfTrust()
	.AddHostedService<TestCircleOfTrust>()
;
IHost host = applicationBuilder.Build();
await host.RunAsync();
