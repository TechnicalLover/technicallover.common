using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LibOwin;

namespace TechnicalLover.Common.AspNetCore.Middlewares
{
    using HttpContext = IDictionary<string, object>;
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class MonitoringMiddleware : BaseMiddleware<HttpContext, AppFunc>
    {
        private readonly Func<Task<bool>> healthCheck;

        private static readonly PathString monitorPath =
            new PathString("/_monitor");
        private static readonly PathString monitorShallowPath =
            new PathString("/_monitor/shallow");
        private static readonly PathString monitorDeepPath =
            new PathString("/_monitor/deep");

        public MonitoringMiddleware(AppFunc next, Func<Task<bool>> healthCheck)
            : base(next)
        {
            this.healthCheck = healthCheck ?? throw new ArgumentNullException(nameof(healthCheck));
        }

        public override Task Invoke(HttpContext context)
        {
            var owinContext = new OwinContext(context);
            if (owinContext.Request.Path.StartsWithSegments(monitorPath))
                return ShallowEndpoint(owinContext);
            else
                return this.next(context);
        }

        private Task HandleMonitorEndpoint(OwinContext context)
        {
            if (context.Request.Path.StartsWithSegments(monitorShallowPath))
                return ShallowEndpoint(context);
            if (context.Request.Path.StartsWithSegments(monitorShallowPath))
                return DeepEndpoint(context);
            return Task.FromResult(0);
        }

        private async Task DeepEndpoint(OwinContext context)
        {
            if (await healthCheck())
                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
            else
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
        }

        private Task ShallowEndpoint(OwinContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
            return Task.FromResult(0);
        }
    }
}