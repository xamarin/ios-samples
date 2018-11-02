
namespace XamarinShot.Utils
{
    using System.Collections.Generic;

    public static class ParserExtensions
    {
        public static T Parse<T>(this string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        public static bool TryGet<T>(this Dictionary<string, object> dictionary, string key, out T result)
        {
            var isSucceed = false;
            result = default(T);

            if (dictionary.TryGetValue(key, out object value))
            {
                if (value is T converted)
                {
                    result = converted;
                    isSucceed = true;
                }
                else
                {
                    try
                    {
                        result = value.ToString().Parse<T>();
                        isSucceed = true;
                    }
                    catch(Newtonsoft.Json.JsonException) { /* another type possible */ }
                }
            }

            return isSucceed;
        }
    }
}