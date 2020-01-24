using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SavableAttribute : Attribute { }
}
