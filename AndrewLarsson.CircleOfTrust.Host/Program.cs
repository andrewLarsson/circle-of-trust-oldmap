using AndrewLarsson.CircleOfTrust.Domain;
using AndrewLarsson.CircleOfTrust.Host;
using AndrewLarsson.CircleOfTrust.Simulations;
using AndrewLarsson.CircleOfTrust.View;
using developersBliss.OLDMAP.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

var applicationBuilder = WebApplication.CreateBuilder(args);

applicationBuilder.Services.AddControllers().AddJsonOptions(opts => {
	var enumConverter = new JsonStringEnumConverter();
	opts.JsonSerializerOptions.Converters.Add(enumConverter);
});
applicationBuilder.Services.AddEndpointsApiExplorer();
applicationBuilder.Services.AddSwaggerGen();

/* Begin Authentication */
applicationBuilder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options => {
		options.Authority = "https://accounts.google.com";
		options.TokenValidationParameters = new TokenValidationParameters {
			ValidateIssuer = true,
			ValidIssuer = "https://accounts.google.com",
			ValidateAudience = true,
			ValidAudience = applicationBuilder.Configuration["Authentication:GoogleClientId"],
			ValidateLifetime = true
		};
	})
;
applicationBuilder.Services.AddAuthorization();
/* End Authentication */

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
application.UseAuthentication();
application.UseAuthorization();
application.MapControllers();
application.MapFallbackToFile("/index.html");

await application.RunAsync();
