using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreService.Model;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace WebApplication
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
            // for iis configuration
            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });



            var config = new ConfigurationBuilder()
                             .AddJsonFile("appsettings.json",
                                             optional: false,
                                             reloadOnChange: true)
                             .Build();
            services.AddSingleton(config);

            services.AddOptions();
            services.Configure<AppSettings>(config.GetSection("AppSettings"));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var appSettings = services.BuildServiceProvider().GetService<IOptions<AppSettings>>()?.Value;

            var busControl =  Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(appSettings?.RabbitMQHost), h =>
                {
                    h.Username(appSettings?.RabbitMqUserName);
                    h.Password(appSettings?.RabbitMqPassword);
                });
            });

            services.AddSingleton<IBus>(busControl);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            busControl.Start();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=ServiceTest}/{id?}");
            });
        }
    }
}
