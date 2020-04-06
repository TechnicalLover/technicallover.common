using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibOwin;
using Serilog;

namespace TechnicalLover.Common.AspNetCore.Logging
{
    using HttpContext = IDictionary<string, object>;
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class LoggingMiddleware : BaseMiddleware<HttpContext, AppFunc>
    {
        private readonly ILogger _logger;

        public LoggingMiddleware(AppFunc next, ILogger logger)
            : base(next)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task Invoke(HttpContext context)
        {
            var owinContext = new OwinContext(context);
            _logger.Information("Incoming request: {@Method}, {@Path}, {@Header}", owinContext.Request.Method, owinContext.Request.Path, owinContext.Request.Headers);
            await next(context);
            _logger.Information("Outgoing response: {@StatusCode}, {@Headers}", owinContext.Response.StatusCode, owinContext.Response.Headers);
        }
    }
}