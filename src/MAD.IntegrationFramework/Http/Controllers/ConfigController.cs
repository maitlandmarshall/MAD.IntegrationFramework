using MAD.IntegrationFramework.Configuration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace MAD.IntegrationFramework.Http.Controllers
{
    internal sealed class ConfigController : Controller
    {
        private readonly IMIFConfigFactory mifConfigFactory;
        private readonly IMIFConfigRepository mifConfigRepository;

        public ConfigController(IMIFConfigFactory mifConfigFactory,
                                IMIFConfigRepository mifConfigRepository)
        {
            this.mifConfigFactory = mifConfigFactory;
            this.mifConfigRepository = mifConfigRepository;
        }

        public ActionResult Index()
        {
            return View(
                this.mifConfigFactory.Create()
            );
        }

        private string ReadBodyAsString()
        {
            using (StreamReader sr = new StreamReader(this.Request.Body))
            {
                return sr.ReadToEnd();
            }
        }

        public async Task<ActionResult> Save()
        {
            string submitBody = this.ReadBodyAsString();

            MIFConfig config = this.mifConfigFactory.Create();

            // Get all the properties on the MIF.Config which can be set / updated
            List<PropertyInfo> MIFConfigProperties = config
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToList();

            // The submitBody will be form encoded. i.e. key=value
            // Parse this into a NamedValueCollection
            NameValueCollection submitData = HttpUtility.ParseQueryString(submitBody);

            // Loop through each key in the NameValueCollection
            foreach (string key in submitData.AllKeys)
            {
                object value = submitData[key];

                // Does the key exist as a property for MIF.Config?
                PropertyInfo MIFConfigProp = MIFConfigProperties.FirstOrDefault(y => y.Name == key);

                // If it doesn't exist, skip to the next one.
                if (MIFConfigProp == null)
                    continue;

                // Each property value in submitData will be a string. If the MIFConfigProp.PropertyType isn't a string, convert it to the underlying PropertyType.
                if (MIFConfigProp.PropertyType != typeof(string))
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(MIFConfigProp.PropertyType);

                    if (converter.CanConvertFrom(typeof(string)))
                    {
                        value = converter.ConvertFromString(value as string);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }

                MIFConfigProp.SetValue(config, value);
            }

            await this.mifConfigRepository.Save(config);

            return RedirectToAction("Index");
        }
    }
}
