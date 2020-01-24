using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace MAD.IntegrationFramework.Serialization
{
    public class NestedJsonConverterSelector
    {
        public string Path { get; private set; }
        public string[] Selectors { get; private set; }

        public NestedJsonConverterSelector(string selector)
        {
            // Remove the selectors from the path
            this.Path = Regex.Replace(selector, @"(\[\])*(\{\})*,*", "");

            List<string> selectors = new List<string>();

            Match match = Regex.Match(selector, @"(\[\])*(\{\})*");

            while (match.Captures.Count > 0)
            {
                if (!String.IsNullOrEmpty(match.Value))
                {
                    if (match.Value != "{}" && match.Value != "[]")
                        throw new NotSupportedException("Supported selectors are [] and {}");

                    selectors.Add(match.Value);
                }

                match = match.NextMatch();
            }

            this.Selectors = selectors.ToArray();
        }
    }
}
