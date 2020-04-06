using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibOwin;
using Serilog.Context;
using TechnicalLover.Common.AspNetCore.Constants;

namespace TechnicalLover.Common.AspNetCore.CorrelationToken
{
    using HttpContext = IDictionary<string, object>;
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class CorrelationTokenMiddleware : BaseMiddleware<HttpContext, AppFunc>
    {
        public CorrelationTokenMiddleware(AppFunc next)
            : base(next)
        {
        }

        public override async Task Invoke(HttpContext context)
        {
            Guid correlationToken;
            var owinContext = new OwinContext(context);
            if ((owinContext.Request.Headers[RequestHeaderConstants.CorrelationToken] != null
                && Guid.TryParse(owinContext.Request.Headers[RequestHeaderConstants.CorrelationToken], out correlationToken)))
                correlationToken = Guid.NewGuid();

            // add correlation token to OWIN context
            owinContext.Set(OwinContextConstants.CorrelationToken, correlationToken.ToString());

            // add correlation token to Serilog log context
            using (LogContext.PushProperty(LogContextPropertyConstants.CorrelationToken, correlationToken))
                await next(context);
        }
    }
}