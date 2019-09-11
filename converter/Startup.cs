using System.Threading.Tasks;
using converterApi.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Owin;
using Serilog;
using Serilog.Events;

namespace converterApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var log = ConfigureLogger();

            app.UseServiceMonitoring(() => Task.FromResult(true));
            app.UseLoggingMiddleware(log);
            app.UseOwin(func =>
                func.UseNancy());
        }

        private ILogger ConfigureLogger()
        {
            return new LoggerConfiguration().Enrich.FromLogContext()
                .WriteTo
                .File("logs/log.txt", LogEventLevel.Verbose,"{NewLine}{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}
