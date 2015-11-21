namespace DistributedMaster
{
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Hosting;
    using Microsoft.Framework.DependencyInjection;
    using Microsoft.Framework.Logging;
    using Models;
    using Newtonsoft.Json.Serialization;
    using Services;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config =>
            {

            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddLogging();

            services.AddSingleton<ProcessingJobRepository>();
            services.AddSingleton<ProcessingJobWorkRepository>();

            services.AddTransient<ProcessingJobService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDebug(LogLevel.Debug);
            loggerFactory.AddConsole(LogLevel.Debug);

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                //routes.MapRoute(
                //    name: "job",
                //    template: "{controller}/{action}/{id?}",
                //    defaults: new { controller = "ProcessingJob", action = "Get" });
            });
        }
    }
}