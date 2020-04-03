using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;

namespace TechnicalLover.Common.AspNetCore.Middlewares
{
    using HttpContext = IDictionary<string, object>;
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class GlobalErrorLoggingMiddleware : BaseMiddleware<HttpContext, AppFunc>
    {
        private readonly ILogger _logger;

        public GlobalErrorLoggingMiddleware(AppFunc next, ILogger logger)
            : base(next)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unhandled exception");
            }
        }
    }
}