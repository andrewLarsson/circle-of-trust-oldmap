using AndrewLarsson.CircleOfTrust.Domain;
using AndrewLarsson.CircleOfTrust.Simulations;
using AndrewLarsson.CircleOfTrust.View;
using developersBliss.OLDMAP.Hosting;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder applicationBuilder = Host.CreateApplicationBuilder();
applicationBuilder.Services
	.AddOLDMAP()
	.AddCircleOfTrust()
	.AddCircleOfTrustView()
	//.AddSampleCircleOfTrustSimulation()
	.AddLargestCircleSimulation()
;
IHost host = applicationBuilder.Build();
await host.RunAsync();
