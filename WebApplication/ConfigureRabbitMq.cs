using CoreService.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication
{
    public class ConfigureRabbitMq
    {
        private readonly AppSettings _config;
        private IOptions<AppSettings> _configOptions;

        public ConfigureRabbitMq(IOptions<AppSettings> config)
        {
            _configOptions = config;
            _config = config.Value;
        }

        public void ConfigureMass(IServiceCollection services)
        {

        }
    }
}
