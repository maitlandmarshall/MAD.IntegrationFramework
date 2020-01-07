using MaitlandsInterfaceFramework.Pardot.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Pardot.Api
{
    public class AccountResponse : ApiResponse
    {
        public Account Account { get; set; }
    }
}
