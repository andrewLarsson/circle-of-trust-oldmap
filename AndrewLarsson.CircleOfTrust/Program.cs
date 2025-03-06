using AndrewLarsson.CircleOfTrust.Hosting;
using AndrewLarsson.CircleOfTrust.Simulations;
using AndrewLarsson.CircleOfTrust.View;
using developersBliss.OLDMAP.Hosting;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder applicationBuilder = Host.CreateApplicationBuilder();
applicationBuilder.Services
	.AddOLDMAP()
	.AddKafkaDomainMessageApplication()
	.AddKafkaDomainEventApplication()
	.AddPostgreSqlMartenAggregateRootStorage()
	.AddCircleOfTrust()
	//.AddSampleCircleOfTrustSimulation()
	.AddView()
	.AddLargestCircleSimulation(noHandlers: true)
;
IHost host = applicationBuilder.Build();
await host.RunAsync();
