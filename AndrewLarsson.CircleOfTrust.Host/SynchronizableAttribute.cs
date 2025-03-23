using developersBliss.OLDMAP.Application;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace AndrewLarsson.CircleOfTrust.Host;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SynchronizableAttribute(string Application) : Attribute, IAsyncActionFilter {
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
		if (context.HttpContext.Request.Headers.TryGetValue(Headers.SynchronizationToken, out StringValues synchronizationToken)) {
			var synchronizer = context.HttpContext.RequestServices.GetRequiredService<ISynchronizer>();
			await synchronizer.SynchronizeWith(Application, synchronizationToken.ToString());
		}
		await next();
	}
}
