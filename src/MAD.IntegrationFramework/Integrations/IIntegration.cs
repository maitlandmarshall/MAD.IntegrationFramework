using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Integrations
{
    public interface IIntegration
    {
        abstract bool IsEnabled { get; }
        abstract Task Execute();
    }
}
