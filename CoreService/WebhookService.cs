using CoreService.Model;
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using MassTransit;
using GreenPipes;
using Microsoft.Extensions.Options;

namespace CoreService
{
    public class WebhookService : ServiceBase
    {
        //private readonly IBus _bus;
        public static IBusControl _busControl;
        private readonly AppSettings _config;

        IOptions<AppSettings> _configOptions;
        public WebhookService(/*IBus bus*/ IOptions<AppSettings> config)
        {
            //_bus = bus;
            //InitializeComponent();
            ServiceName = "TestService";
            _configOptions = config;
            _config = config.Value;
        }

        public void Process()
        {
            OnStart(null);
        }

        public void Start(string[] args)
        {
            OnStart(args);
        }
        public new void Stop()
        {
            OnStop();
        }

        protected override void OnStart(string[] args)
        {

            int prefetchCount =  0;
            string rabbitMQHost = _config.RabbitMQHost;
            string rabbitMqUserName = _config.RabbitMqUserName;
            string rabbitMqPassword = _config.RabbitMqPassword; //EncryptDecrypt.AES_Decrypt(_config.RabbitMqPassword);

            var retryCount = Convert.ToInt32(_config.RetryCount);
            int.TryParse(_config.PrefetchCount, out prefetchCount);

            _busControl = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri(rabbitMQHost), h =>
                {
                    h.Username(rabbitMqUserName);
                    h.Password(rabbitMqPassword);
                });

                sbc.UseRetry(p => p.Immediate(retryCount));

                sbc.ReceiveEndpoint(host, "Webhook", ep =>
                {
                    if (prefetchCount > 0)
                        ep.PrefetchCount = (ushort)prefetchCount;

                    ep.Consumer(() => new Consumers.WebhookConsumers(_configOptions));
                });

            });

            _busControl.Start();

        }

        protected override void OnStop()
        {
            _busControl.Stop();
            base.OnStop();
        }
    }
}
