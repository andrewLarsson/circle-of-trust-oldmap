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
ThreadPool.SetMinThreads(workerThreads: 1024, completionPortThreads: 1024);
System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
await host.RunAsync();
