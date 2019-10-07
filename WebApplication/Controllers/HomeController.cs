using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CoreService.Interfaces;
using CoreService.Model;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppSettings _config;
        private IOptions<AppSettings> _configOptions;
        public static IBus _webhookBusControl;
        public static IBus WebhookBusControl
        {
            get
            {
                return _webhookBusControl;
            }
        }

        public HomeController(IOptions<AppSettings> config, IBus webhookBusControl)
        {
            _configOptions = config;
            _config = config.Value;
            _webhookBusControl = webhookBusControl;
        }
        public IActionResult ServiceTest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ServiceTest(RequestModel collection)
        {
            //var msg = collection.TryGetValue

            //_webhookBusControl = ConfigureWebhookBus();
            //_webhookBusControl.Start();

            _webhookBusControl.Publish<IWebhookResponse>(new
            {
                ClientCode = "",
                Category = "",
                Type = "",
                Event = "",
                Result = collection.Message
            });
            //container.RegisterInstance<IBus>("WebhookBus", WebhookBusControl);

            return View();
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IBusControl ConfigureWebhookBus()
        {
            //var webhookRabbitMQHost = Convert.ToString(ConfigurationManager.AppSettings["WebhookRabbitMQHost"]);
            //var rabbitMqUserName = Convert.ToString(ConfigurationManager.AppSettings["RabbitMqUserName"]);
            //var rabbitMqPassword = EncryptDecrypt.AES_Decrypt(Convert.ToString(ConfigurationManager.AppSettings["RabbitMqPassword"]));
            //var clientCode = _clientCode;

            int prefetchCount = 0;
            string webhookRabbitMQHost = _config.RabbitMQHost;
            string rabbitMqUserName = _config.RabbitMqUserName;
            string rabbitMqPassword = ""; //EncryptDecrypt.AES_Decrypt(_config.RabbitMqPassword);

            var retryCount = Convert.ToInt32(_config.RetryCount);
            int.TryParse(_config.PrefetchCount, out prefetchCount);

            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(webhookRabbitMQHost), h =>
                {
                    h.Username(rabbitMqUserName);
                    h.Password(rabbitMqPassword);
                });
            });
        }
    }
}
