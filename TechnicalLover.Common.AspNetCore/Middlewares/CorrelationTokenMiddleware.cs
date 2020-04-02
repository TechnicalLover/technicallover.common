using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibOwin;
using Serilog.Context;

namespace TechnicalLover.Common.AspNetCore.Middlewares
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class CorrelationTokenMiddleware
    {
        private readonly AppFunc next;

        public CorrelationTokenMiddleware(AppFunc next)
        {
            this.next = next;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            Guid correlationToken;
            var context = new OwinContext(env);
            if ((context.Request.Headers["Correlation-Token"] != null
                && Guid.TryParse(context.Request.Headers["Correlation-Token"], out correlationToken)))
                correlationToken = Guid.NewGuid();

            // add correlation token to OWIN context
            context.Set("correlationToken", correlationToken.ToString());

            // add correlation token to Serilog log context
            using (LogContext.PushProperty("CorrelationToken", correlationToken))
                await next(env);
        }
    }
}