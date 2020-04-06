using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MAD.IntegrationFramework.Http
{
    internal class InternalControllerFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            if (typeInfo.Assembly == typeof(InternalControllerFeatureProvider).Assembly
                && !typeInfo.IsAbstract)
                return true;

            return base.IsController(typeInfo);
        }
    }
}
