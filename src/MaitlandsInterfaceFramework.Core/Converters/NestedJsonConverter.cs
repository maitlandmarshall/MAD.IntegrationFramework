using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Humanizer;
using System.Linq;
using System.Collections;

namespace MaitlandsInterfaceFramework.Core.Converters
{
    public class NestedJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        private string PascalizePath (string path)
        {
            string[] pathSplit = path.Split('.').Select(y => y.Pascalize()).ToArray();
            string pascalizedPath = String.Join(".", pathSplit);

            return pascalizedPath;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, object> flattenedJson = new Dictionary<string, object>();

            do
            {
                string path = this.PascalizePath(reader.Path);

                switch (reader.TokenType)
                {
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.String:
                    case JsonToken.Boolean:                        
                    case JsonToken.Null:
                    case JsonToken.Date:
                    case JsonToken.Bytes:
                        flattenedJson[path] = reader.Value;
                        break;
                }
            }
            while (reader.Read());

            return this.ReadFlattenedJsonIntoTargetObjectType(flattenedJson, objectType);
        }

        private object ReadFlattenedJsonIntoTargetObjectType (IDictionary<string, object> flattenedJson, Type objectType, string trailingJsonPropertyPath = null)
        {
            JsonClassAttribute jsonClassAttribute = objectType.GetCustomAttribute<JsonClassAttribute>();
            object finalResult = Activator.CreateInstance(objectType);

            if (finalResult is IList finalList)
            {
                Type underlyingEnumerableType = objectType
                    .GetInterfaces()
                    .FirstOrDefault(y => typeof(IEnumerable<object>).IsAssignableFrom(y))
                    .GetGenericArguments()
                    .FirstOrDefault();

                if (jsonClassAttribute == null)
                    jsonClassAttribute = underlyingEnumerableType.GetCustomAttribute<JsonClassAttribute>();

                string jsonClassAttributePath;

                if (String.IsNullOrEmpty(trailingJsonPropertyPath))
                {
                    jsonClassAttributePath = jsonClassAttribute?.Path ?? "";
                }
                else
                {
                    jsonClassAttributePath = $"{trailingJsonPropertyPath}.{jsonClassAttribute?.Path ?? ""}";
                }

                jsonClassAttributePath = this.PascalizePath(jsonClassAttributePath);

                // Key all the keys which start with the JsonClassAttribute's path
                // And group them by the array index
                var groupedFlattenJsonByArrayIndex = flattenedJson
                    .Where(y => y.Key.StartsWith(jsonClassAttributePath))
                    .GroupBy(y =>
                    {
                        int indexOfArray = y.Key.IndexOf("[");

                        if (indexOfArray == -1)
                            return "";

                        string pathToArray = y.Key.Substring(0, indexOfArray);

                        if (!pathToArray.EndsWith(jsonClassAttributePath))
                            return "";

                        string index = y.Key.Substring(indexOfArray + 1);
                        index = index.Substring(0, index.IndexOf("]"));

                        return index;
                    });

                foreach (var group in groupedFlattenJsonByArrayIndex)
                {
                    Dictionary<string, object> arrayItemDictionary = group.ToDictionary(pair => pair.Key, pair => pair.Value);

                    object targetObjectResult;
                    if (String.IsNullOrEmpty(group.Key))
                    {
                        targetObjectResult = this.ReadFlattenedJsonIntoTargetObjectType(arrayItemDictionary, underlyingEnumerableType, $"{jsonClassAttributePath}");
                    }
                    else
                    {
                        targetObjectResult = this.ReadFlattenedJsonIntoTargetObjectType(arrayItemDictionary, underlyingEnumerableType, $"{jsonClassAttributePath}[{group.Key}]");
                    }

                    finalList.Add(targetObjectResult);
                }
            }
            else
            {
                PropertyInfo[] propertiesToDeserialize = objectType.GetProperties();
                foreach (PropertyInfo propToDeserialize in propertiesToDeserialize)
                {
                    JsonPropertyAttribute jsonPropertyAttribute = propToDeserialize.GetCustomAttribute<JsonPropertyAttribute>(true);
                    string flatJsonLookupKey;

                    if (jsonPropertyAttribute != null)
                    {
                        flatJsonLookupKey = this.PascalizePath(jsonPropertyAttribute.PropertyName);
                    }
                    else
                    {
                        flatJsonLookupKey = propToDeserialize.Name.Pascalize();
                    }

                    if (jsonClassAttribute?.IsEnumerablePath == false)
                        flatJsonLookupKey = $"{this.PascalizePath(jsonClassAttribute.Path)}.{flatJsonLookupKey}";

                    if (!String.IsNullOrEmpty(trailingJsonPropertyPath))
                        flatJsonLookupKey = $"{trailingJsonPropertyPath}.{flatJsonLookupKey}";

                    if (!flattenedJson.TryGetValue(flatJsonLookupKey, out object flatJsonLookupValue))
                    {
                        if (jsonPropertyAttribute == null)
                            continue;

                        string flatJsonLookupKeyParent = flatJsonLookupKey.Substring(0, flatJsonLookupKey.LastIndexOf("."));

                        if (flattenedJson.TryGetValue(flatJsonLookupKeyParent, out object flatJsonLookupKeyParentObject))
                        {
                            // If the key exists, but the value is null, it's ok, don't throw any error.
                            if (flatJsonLookupKeyParentObject == null)
                                continue;
                        }

                        throw new Exception($"{flatJsonLookupKey} does not exist in the provided json.");
                    }

                    if (flatJsonLookupValue != null)
                    {
                        Type underlyingPropertyType = Nullable.GetUnderlyingType(propToDeserialize.PropertyType);

                        flatJsonLookupValue = Convert.ChangeType(flatJsonLookupValue, underlyingPropertyType ?? propToDeserialize.PropertyType);
                    }
                        

                    propToDeserialize.SetValue(finalResult, flatJsonLookupValue);
                }
            }

            return finalResult;
        }

        public override void WriteJson(JsonWriter writer,  object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        
    }
}
