using System;
using System.Collections.Generic;
using System.Text;

namespace CoreService.Interfaces
{
    public interface IWebhookResponse
    {
        string ClientCode { get; set; }

        string Category { get; set; }

        string Type { get; set; }

        string Event { get; set; }

        dynamic Result { get; set; }
    }
}
