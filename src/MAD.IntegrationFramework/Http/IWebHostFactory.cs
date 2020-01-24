using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Http
{
    internal interface IWebHostFactory
    {
        IWebHost CreateWebHost();
    }
}
