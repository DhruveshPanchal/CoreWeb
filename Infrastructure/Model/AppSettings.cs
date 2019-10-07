using System;
using System.Collections.Generic;
using System.Text;

namespace CoreService.Model
{
    public class AppSettings
    {
        public string Title { get; set; }

        public string RabbitMQHost { get; set; }

        public string RabbitMqUserName { get; set; }

        public string RabbitMqPassword { get; set; }
        public int RetryCount { get; set; }
        public string PrefetchCount { get; set; }

        public string EndpointURL { get; set; }
    }
}
