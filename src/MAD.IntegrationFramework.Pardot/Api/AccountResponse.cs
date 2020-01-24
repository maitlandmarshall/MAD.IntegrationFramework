using MAD.IntegrationFramework.Pardot.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Pardot.Api
{
    public class AccountResponse : ApiResponse
    {
        public Account Account { get; set; }
    }
}
