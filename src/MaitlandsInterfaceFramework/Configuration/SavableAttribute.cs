using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SavableAttribute : Attribute { }
}
