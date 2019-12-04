using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SavableAttribute : Attribute { }
}
