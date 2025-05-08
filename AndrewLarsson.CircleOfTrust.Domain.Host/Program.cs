using AndrewLarsson.CircleOfTrust.Domain;
using developersBliss.OLDMAP.Hosting;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder applicationBuilder = Host.CreateApplicationBuilder();
applicationBuilder.Services
	.AddOLDMAP()
	.AddOLDMAP()
	.AddCircleOfTrust()
;
IHost host = applicationBuilder.Build();
await host.RunAsync();
