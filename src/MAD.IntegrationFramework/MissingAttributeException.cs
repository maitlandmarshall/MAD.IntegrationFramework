using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework
{
    public class MissingAttributeException : Exception
    {
        public MissingAttributeException(Type attribute) : base($"{attribute.Name} is missing.") { }
    }
}
