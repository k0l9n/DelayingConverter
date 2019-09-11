using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace converterApi.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _log;

        public LoggingMiddleware(RequestDelegate next, ILogger log)
        {
            this._next = next;
            this._log = log;
        }

        /// <summary>
        /// Where the middleware magic happens
        /// </summary>
        /// 
        /// <returns>Task</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            // If the path is different from the statePath let the request through the normal pipeline.
            _log.Information($"Request: {httpContext.Request.Path},{httpContext.Request.Method},{httpContext.Request.Headers}");
            await this._next.Invoke(httpContext);
            _log.Information($"Response: Code:{httpContext.Response.StatusCode},L:{httpContext.Response.ContentLength} executed in {stopWatch.ElapsedMilliseconds:000}ms");

        }
    }
    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleware(
            this IApplicationBuilder app,
            ILogger log)
        {
            app.UseMiddleware<LoggingMiddleware>(log);

            return app;
        }
    }
}
