using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace converter.Middleware
{
    public class ServiceStatusMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Health check function.
        /// </summary>
        private readonly Func<Task<bool>> _serviceStatusCheck;

        /// <summary>
        /// ServiceStatus endpoint path 
        /// </summary>
        private static readonly PathString StatePath = new PathString("/_check");

        /// <summary>
        /// Constructor
        /// </summary>
        /// 
        /// 
        public ServiceStatusMiddleware(RequestDelegate next, Func<Task<bool>> serviceStatusCheck)
        {
            this._next = next;
            this._serviceStatusCheck = serviceStatusCheck;
        }

        /// <summary>
        /// Where the middleware magic happens
        /// </summary>
        /// 
        /// <returns>Task</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            // If the path is different from the statePath let the request through the normal pipeline.
            if (!httpContext.Request.Path.Equals(StatePath))
            {
                await this._next.Invoke(httpContext);
            }
            else
            {
                // If the path is statePath call the health check function.
                await CheckAsync(httpContext);
            }
        }

        /// <summary>
        /// Call the health check function and set the response.
        /// </summary>
        /// 
        /// <returns>Task</returns>
        private async Task CheckAsync(HttpContext httpContext)
        {
            if (await this._serviceStatusCheck().ConfigureAwait(false))
            {
                // Service is available.
                await WriteResponseAsync(httpContext, HttpStatusCode.OK, new ServiceStatus(true));
            }
            else
            {
                // Service is unavailable.
                await WriteResponseAsync(httpContext, HttpStatusCode.ServiceUnavailable, new ServiceStatus(false));
            }
        }

        /// <summary>
        /// Writes a response of the Service Status Check.
        /// </summary>
        /// 
        /// 
        /// 
        /// <returns>Task</returns>
        private Task WriteResponseAsync(HttpContext httpContext, HttpStatusCode httpStatusCode, ServiceStatus serviceStatus)
        {
            // Set content type.
            httpContext.Response.Headers["Content-Type"] = new[] { "application/json" };

            // Minimum set of headers to disable caching of the response.
            httpContext.Response.Headers["Cache-Control"] = new[] { "no-cache, no-store, must-revalidate" };
            httpContext.Response.Headers["Pragma"] = new[] { "no-cache" };
            httpContext.Response.Headers["Expires"] = new[] { "0" };

            // Set status code.
            httpContext.Response.StatusCode = (int)httpStatusCode;

            // Write the content.
            var content = JsonConvert.SerializeObject(serviceStatus);
            return httpContext.Response.WriteAsync(content);
        }
    }

    /// <summary>
    /// ServiceStatus to hold the response. 
    /// </summary>
    public class ServiceStatus
    {
        public ServiceStatus(bool available)
        {
            Available = available;
        }

        /// <summary>
        /// Tells if the service is available
        /// </summary>
        /// <returns>True if the service is available</returns>
        public bool Available { get; }
    }

    /// <summary>
    /// Service Status Middleware Extensions
    /// </summary>
    public static class ServiceStatusMiddlewareExtensions
    {
        public static IApplicationBuilder UseServiceStatus(
          this IApplicationBuilder app,
          Func<Task<bool>> serviceStatusCheck)
        {
            app.UseMiddleware<ServiceStatusMiddleware>(serviceStatusCheck);

            return app;
        }
    }
}
