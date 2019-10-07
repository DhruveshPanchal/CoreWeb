using CoreService.Interfaces;
using CoreService.Model;
using MassTransit;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreService.Consumers
{
    public class WebhookConsumers : IConsumer<IWebhookResponse>
    {
        private readonly AppSettings _config;
        public WebhookConsumers(IOptions<AppSettings> config)
        {
            _config = config.Value;
        }

        public async Task Consume(ConsumeContext<IWebhookResponse> context)
        {
            if (string.IsNullOrEmpty(_config.EndpointURL)) return;

            try
            {
                var client = new RestClient(_config.EndpointURL);

                var request = new RestRequest(Method.POST).AddParameter("application/json", context.Message.Result, ParameterType.RequestBody);

                client.Execute(request);
            }
            catch (Exception ex)
            {
                //_logger.Info(string.Format("Error : {0}", ex));
            }
        }
    }
}
