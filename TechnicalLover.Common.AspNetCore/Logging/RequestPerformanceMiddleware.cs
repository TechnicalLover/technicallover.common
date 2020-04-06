using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibOwin;
using Serilog;
using TechnicalLover.Common.Extensions;

namespace TechnicalLover.Common.AspNetCore.Logging
{
    using HttpContext = IDictionary<string, object>;
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class RequestPerformanceMiddleware : BaseMiddleware<HttpContext, AppFunc>
    {
        private readonly ILogger _logger;

        public RequestPerformanceMiddleware(AppFunc next, ILogger logger)
            : base(next)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task Invoke(HttpContext context)
        {
            var stopWatch = ValueStopwatch.StartNew();
            await next(context);
            var requestTime = stopWatch.GetElapsedTime().Milliseconds;
            var owinContext = new OwinContext(context);
            _logger.Information(
                "{@Method} {@Path} executed in {RequestTime:000} ms",
                owinContext.Request.Method,
                owinContext.Request.Path,
                requestTime);
        }
    }
}