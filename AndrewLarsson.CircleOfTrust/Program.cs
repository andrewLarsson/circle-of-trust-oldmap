using AndrewLarsson.CircleOfTrust.Hosting;
using AndrewLarsson.CircleOfTrust.Simulations;
using developersBliss.OLDMAP.Hosting;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder applicationBuilder = Host.CreateApplicationBuilder();
applicationBuilder.Services
	.AddOLDMAP()
	.AddKafkaDomainMessageApplication()
	.AddKafkaDomainEventApplication()
	.AddPostgreSqlMartenAggregateRootStorage()
	.AddCircleOfTrust()
	.AddLargestCircleSimulation()
;
IHost host = applicationBuilder.Build();
await host.RunAsync();
