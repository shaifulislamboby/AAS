using System;
using System.Collections.Generic;

namespace ComplexAssetAdministrationShellScenario
{
    public class DataStorage
    {
        public Dictionary<string, Dictionary<string, object>> dataDictionary;

        public DataStorage()
        {
            dataDictionary = new Dictionary<string, Dictionary<string, object>>();
        }

        public void SaveData(string key, Dictionary<string, object> data)
        {
            if (!dataDictionary.ContainsKey(key))
            {
                dataDictionary.Add(key, data);
            }
            else
            {
                Dictionary<string, object> existingData = dataDictionary[key];
                // Handle key collision if necessary (update existing data or throw an exception)
                // For this example, we will just ignore the duplicate key.
            }
        }

        public void ModifyInnerValue(string key, string innerKey, object newValue)
        {
            if (dataDictionary.TryGetValue(key, out var innerDictionary))
            {
                innerDictionary[innerKey] = newValue;
            }
        }

        public object GetInnerValue(string key, string innerKey)
        {
            if (dataDictionary.TryGetValue(key, out var innerDictionary) &&
                innerDictionary.TryGetValue(innerKey, out var value))
            {
                return value;
            }
            return null;
        }

        public void AddInnerValue(string key, string innerKey, object value)
        {
            if (dataDictionary.TryGetValue(key, out var innerDictionary))
            {
                innerDictionary.Add(innerKey, value);
            }
        }

        public void DeleteData(string key)
        {
            dataDictionary.Remove(key);
        }
    }
}