using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LibOwin;

namespace TechnicalLover.Common.AspNetCore.Middlewares
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class MonitoringMiddleware
    {
        private AppFunc next;
        private readonly Func<Task<bool>> healthCheck;

        private static readonly PathString monitorPath =
            new PathString("/_monitor");
        private static readonly PathString monitorShallowPath =
            new PathString("/_monitor/shallow");
        private static readonly PathString monitorDeepPath =
            new PathString("/_monitor/deep");

        public MonitoringMiddleware(AppFunc next, Func<Task<bool>> healthCheck)
        {
            this.healthCheck = healthCheck;
            this.next = next;
        }

        public Task Invoke(IDictionary<string, object> env)
        {
            var context = new OwinContext(env);
            if (context.Request.Path.StartsWithSegments(monitorPath))
                return ShallowEndpoint(context);
            else
                return this.next(env);
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