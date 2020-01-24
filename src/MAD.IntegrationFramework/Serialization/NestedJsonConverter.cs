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
using System.Text.RegularExpressions;

namespace MAD.IntegrationFramework.Serialization
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

        private object ReadFlattenedJsonIntoTargetObjectType (IDictionary<string, object> flattenedJson, Type objectType)
        {
            object finalResult = Activator.CreateInstance(objectType);

            if (finalResult is IList finalList)
            {
                this.ReadFlattenedJsonIntoList(finalList, flattenedJson);
            }
            else
            {
                this.ReadFlattenedJsonIntoObject(finalResult, flattenedJson);
            }

            return finalResult;
        }

        private void ReadFlattenedJsonIntoObject(object finalResult, IDictionary<string, object> flattenedJson)
        {
            Type objectType = finalResult.GetType();
            JsonClassAttribute jsonClassAttribute = objectType.GetCustomAttribute<JsonClassAttribute>();

            PropertyInfo[] propertiesToDeserialize = objectType.GetProperties();
            foreach (PropertyInfo propToDeserialize in propertiesToDeserialize)
            {
                JsonPropertyAttribute jsonPropertyAttribute = propToDeserialize.GetCustomAttribute<JsonPropertyAttribute>(true);
                string flatJsonLookupKey;
                bool isJsonPropertyAttributeValueASelector;

                if (jsonPropertyAttribute != null)
                {
                    isJsonPropertyAttributeValueASelector = jsonPropertyAttribute.PropertyName.Split(',').Any(y => y == "{}" || y == "[]");
                    flatJsonLookupKey = this.PascalizePath(jsonPropertyAttribute.PropertyName);
                }
                else
                {
                    isJsonPropertyAttributeValueASelector = false;
                    flatJsonLookupKey = propToDeserialize.Name.Pascalize();
                }

                if (jsonClassAttribute?.IsEnumerablePath == false)
                    flatJsonLookupKey = $"{this.PascalizePath(jsonClassAttribute.Path)}.{flatJsonLookupKey}";

                string flatJsonLookupKeyParent;

                if (isJsonPropertyAttributeValueASelector)
                {
                    flatJsonLookupKeyParent = this.GetFirstFlatKeyThatMatchesSelector(flatJsonLookupKey, flattenedJson);
                }
                else
                {
                    flatJsonLookupKeyParent = this.GetFlatKeyParent(flatJsonLookupKey);
                }

                Type propertyTypeUnderlying = Nullable.GetUnderlyingType(propToDeserialize.PropertyType) ?? propToDeserialize.PropertyType;

                if (Type.GetTypeCode(propertyTypeUnderlying) == TypeCode.Object)
                {
                    // Get get all the props for that object 
                    Dictionary<string, object> propsForDeserializingObject = flattenedJson
                        .Where(y => y.Key.StartsWith($"{flatJsonLookupKeyParent}.") || y.Key.StartsWith($"{flatJsonLookupKeyParent}["))
                        .ToDictionary(
                            // and then trim the parent from the dictionary
                            keySelector: y => {
                                string newKey = y.Key.Substring(flatJsonLookupKeyParent.Length);

                                if (newKey.StartsWith("."))
                                    newKey = newKey.Substring(1);

                                return newKey;
                            },
                            elementSelector: y => y.Value);

                    object result = this.ReadFlattenedJsonIntoTargetObjectType(propsForDeserializingObject, propToDeserialize.PropertyType);

                    propToDeserialize.SetValue(finalResult, result);
                }
                else
                {
                    if (!flattenedJson.TryGetValue(flatJsonLookupKey, out object flatJsonLookupValue))
                    {
                        if (jsonPropertyAttribute == null)
                            continue;

                        if (flattenedJson.TryGetValue(flatJsonLookupKeyParent, out object flatJsonLookupKeyParentObject))
                        {
                            // If the key exists, but the value is null, it's ok, don't throw any error.
                            if (flatJsonLookupKeyParentObject == null)
                                continue;
                        }

                        continue;
                    }

                    if (flatJsonLookupValue != null)
                    {
                        Type underlyingPropertyType = Nullable.GetUnderlyingType(propToDeserialize.PropertyType);

                        flatJsonLookupValue = Convert.ChangeType(flatJsonLookupValue, underlyingPropertyType ?? propToDeserialize.PropertyType);
                    }


                    propToDeserialize.SetValue(finalResult, flatJsonLookupValue);
                }
            }
        }

        private void ReadFlattenedJsonIntoList(IList finalList, IDictionary<string, object> flattenedJson)
        {
            Type objectType = finalList.GetType();
            JsonClassAttribute jsonClassAttribute = objectType.GetCustomAttribute<JsonClassAttribute>();

            Type underlyingEnumerableType = objectType
                    .GetInterfaces()
                    .FirstOrDefault(y => typeof(IEnumerable<object>).IsAssignableFrom(y))
                    .GetGenericArguments()
                    .FirstOrDefault();

            if (jsonClassAttribute == null)
                jsonClassAttribute = underlyingEnumerableType.GetCustomAttribute<JsonClassAttribute>();

            string jsonClassAttributePath = jsonClassAttribute?.Path ?? "";
            jsonClassAttributePath = this.PascalizePath(jsonClassAttributePath);

            // Key all the keys which start with the JsonClassAttribute's path
            // And group them by the array index
            var groupedFlattenJsonByArrayIndex = flattenedJson

                // Must start with an array square bracket
                .Where(y => y.Key.StartsWith($"{jsonClassAttributePath}"))

                .GroupBy(y =>
                {
                    int indexOfArray = y.Key.IndexOf("[");

                    if (indexOfArray == -1)
                        return "";

                    // The array is part of a nested object, not the direct object
                    if (y.Key.IndexOf(".") < indexOfArray)
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
                Dictionary<string, object> arrayItemDictionary;

                if (!String.IsNullOrEmpty(group.Key))
                {
                    arrayItemDictionary = group.ToDictionary(
                        keySelector: y =>
                        {
                            string newKey = y.Key.Substring(y.Key.IndexOf("]") + 1);

                            if (newKey.StartsWith("."))
                                newKey = newKey.Substring(1);

                            return newKey;
                        },
                        elementSelector: pair => pair.Value);
                }
                else
                {
                    arrayItemDictionary = group.ToDictionary(y => y.Key, y => y.Value);
                }

                object targetObjectResult = this.ReadFlattenedJsonIntoTargetObjectType(arrayItemDictionary, underlyingEnumerableType);

                finalList.Add(targetObjectResult);
            }
        }

        private string GetFlatKeyParent(string flatJsonLookupKey)
        {
            if (flatJsonLookupKey.Contains("."))
            {
                return flatJsonLookupKey.Substring(0, flatJsonLookupKey.LastIndexOf("."));
            }
            else
            {
                return flatJsonLookupKey;
            }
        }

        private string GetFirstFlatKeyThatMatchesSelector(string flatJsonLookupKeySelector, IDictionary<string, object> flattenedJson)
        {
            NestedJsonConverterSelector selector = new NestedJsonConverterSelector(flatJsonLookupKeySelector);

            // Get the keys with the same parent as flatJsonLookupKey
            IEnumerable<string> keysWithSameParent = flattenedJson.Keys.Where(y => y.StartsWith(selector.Path));

            foreach (string key in keysWithSameParent)
            {
                string trimmedKey;

                if (!String.IsNullOrEmpty(selector.Path))
                {
                    trimmedKey = key.Substring(selector.Path.Length).TrimStart('.');
                }
                else
                {
                    trimmedKey = key;
                }

                foreach (string sType in selector.Selectors)
                {   
                    // Return the key which starts an object
                    if (sType == "{}" && trimmedKey.Contains("."))
                    {
                        string result = trimmedKey.Substring(0, trimmedKey.IndexOf("."));

                        if (!String.IsNullOrEmpty(selector.Path)) 
                            result = $"{selector.Path}.{result}";

                        if (result.EndsWith("]"))
                        {
                            result = result.Substring(0, result.LastIndexOf("["));
                        }

                        return result;
                    }

                    // Return the key which starts an array
                    else if (sType == "[]" && key.Contains("["))
                    {
                        return key.Substring(0, key.IndexOf("["));
                    }
                }
            }

            return String.Empty;
        }

        public override void WriteJson(JsonWriter writer,  object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        
    }
}
