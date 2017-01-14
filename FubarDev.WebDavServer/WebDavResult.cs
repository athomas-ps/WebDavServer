﻿using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FubarDev.WebDavServer.Model;

namespace FubarDev.WebDavServer
{
    public class WebDavResult : IWebDavResult
    {
        public WebDavResult(WebDavStatusCodes statusCode)
        {
            StatusCode = statusCode;
        }

        public WebDavStatusCodes StatusCode { get; }
        
        public virtual Task ExecuteResultAsync(IWebDavResponse response, CancellationToken ct)
        {
            response.Headers["DAV"] = response.Dispatcher.SupportedClasses.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            return Task.FromResult(0);
        }
    }
}
