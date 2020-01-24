using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Serialization
{
    public class JsonClassAttribute : Attribute
    {
        public string Path { get; set; }
        public bool IsEnumerablePath { get; set; } = false;


        public JsonClassAttribute (string path)
        {
            this.Path = path;
        }
    }
}
