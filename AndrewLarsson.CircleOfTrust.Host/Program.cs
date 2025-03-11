using AndrewLarsson.CircleOfTrust.Domain;
using AndrewLarsson.CircleOfTrust.Host;
using AndrewLarsson.CircleOfTrust.Simulations;
using AndrewLarsson.CircleOfTrust.View;
using developersBliss.OLDMAP.Hosting;
using System.Text.Json.Serialization;

var applicationBuilder = WebApplication.CreateBuilder(args);

applicationBuilder.Services.AddControllers().AddJsonOptions(opts => {
	var enumConverter = new JsonStringEnumConverter();
	opts.JsonSerializerOptions.Converters.Add(enumConverter);
});
applicationBuilder.Services.AddEndpointsApiExplorer();
applicationBuilder.Services.AddSwaggerGen();

/* Begin Application Services */
applicationBuilder.Services
	.AddOLDMAP()
	.AddCircleOfTrust()
	.AddCircleOfTrustView()
	.AddCircleOfTrustHost()
	//.AddSampleCircleOfTrustSimulation()
	//.AddLargestCircleSimulation()
;
/* End Application Services */

var application = applicationBuilder.Build();

application.UseDefaultFiles();
application.UseStaticFiles();

if (application.Environment.IsDevelopment()) {
	application.UseSwagger();
	application.UseSwaggerUI();
}

application.UseHttpsRedirection();
application.UseAuthorization();
application.MapControllers();
application.MapFallbackToFile("/index.html");

await application.RunAsync();
