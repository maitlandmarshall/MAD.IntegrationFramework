﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Configuration
{
    internal interface IMIFConfigResolver
    {
        Type ResolveType();
    }
}
