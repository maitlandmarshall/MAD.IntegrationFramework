using MAD.IntegrationFramework.Integrations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.TestApp
{
    [Daily(StartHour = 8)]
    public class ExampleIntegration : IIntegration
    {
        public bool IsEnabled => true;

        [Savable]
        public string AStringToSave { get; set; } 

        public Task Execute()
        {
            // Execute your unit of work here. 
            // Any errors will be automatically logged to the database if a SQL Connection String is provided in the configuration class.

            throw new NotImplementedException();
        }
    }
}
