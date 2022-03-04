using System;
using Newtonsoft.Json;

namespace Bidvalet.Business
{
	public class SerializeHelper
	{

		public static string JsonObjectToStringSerializer<T>(T t)
		{
			return  JsonConvert.SerializeObject(t);


		}


		public static T ConvertJSonStringToObject<T>(string jsonString)
		{
			T obj = JsonConvert.DeserializeObject<T>(jsonString);
			return obj;
		}

        public static string JsonObjectToStringSerializerMethod<T>(T t)
        {
            string result = string.Empty;
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,

            };
            result = JsonConvert.SerializeObject(t, jsonSerializerSettings);

            return result;

        }
	}
}

