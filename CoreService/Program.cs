using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;

namespace CoreService
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // Create service collection and configure our services
            var services = ConfigureServices();
            // Generate a provider
            var serviceProvider = services.BuildServiceProvider();


            var service = serviceProvider.GetService<WebhookService>();

            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            if (!Environment.UserInteractive || isService)
            {
                //if (!Environment.UserInteractive || isService)
                //{
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    service
                };
                ServiceBase.Run(ServicesToRun);
            }

            Console.WriteLine("Core service Starting as console app....");
            service.Start(args);
            Console.WriteLine("Service Started.. - Press Enter To Exit");
            Console.ReadLine();
            service.Stop();

            //#if (!DEBUG)

            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[]
            //{
            //    new WebhookService()
            //};
            //ServiceBase.Run(ServicesToRun);

            //Console.ReadKey();

            //#else
            //            WebhookService myServ = new WebhookService();
            //            myServ.Process();
            //            // here Process is my Service function
            //            // that will run when my service onstart is call
            //            // you need to call your own method or function name here instead of Process();
            //#endif
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            // Set up the objects we need to get to configuration settings
            var config = LoadConfiguration();
            services.AddSingleton(config);

            services.AddTransient<WebhookService>();

            services.AddOptions();
            services.Configure<Model.AppSettings>(config.GetSection("AppSettings"));

            return services;
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(PlatformServices.Default.Application.ApplicationBasePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {

            // Add framework services
            //services.AddHealthChecks();

            //services.AddMvc();

            // Register your consumers if the need dependencies
            //services.AddScoped<SomeDependency>();

            // Register MassTransit
            //services.AddMassTransit(x =>
            //{
            //    x.AddConsumer<OrderConsumer>();

            //    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            //    {
            //        var host = cfg.Host(new Uri("localhost"), hostConfigurator =>
            //        {
            //            hostConfigurator.Username("guest");
            //            hostConfigurator.Password("guest");
            //        });

            //        cfg.ReceiveEndpoint(host, "submit-order", ep =>
            //        {
            //            ep.PrefetchCount = 16;
            //            ep.UseMessageRetry(r => r.Interval(2, 100));

            //            ep.ConfigureConsumer<OrderConsumer>(provider);
            //        });
            //    }));
            //});
        }
    }
}
