using MAD.IntegrationFramework.Integrations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.TestApp
{
    public class ExampleIntegration : TimedIntegration
    {
        public override TimeSpan Interval => TimeSpan.FromMinutes(1);
        public override bool IsEnabled => true;

        [Savable]
        public string AStringToSave { get; set; } 

        public override Task Execute()
        {
            // Execute your unit of work here. 
            // Any errors will be automatically logged to the database if a SQL Connection String is provided in the configuration class.

            throw new NotImplementedException();
        }
    }
}
