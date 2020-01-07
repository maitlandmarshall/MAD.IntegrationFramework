using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RunAfterAttribute : Attribute
    {
        public Type InterfaceTypeToRunAfter { get; }

        public RunAfterAttribute(Type interfaceTypeToRunAfter)
        {
            if (!typeof(TimedInterface).IsAssignableFrom(interfaceTypeToRunAfter))
                throw new NotSupportedException($"{nameof(interfaceTypeToRunAfter)} must derive from {nameof(TimedInterface)}");

            this.InterfaceTypeToRunAfter = interfaceTypeToRunAfter;
        }
    }
}
