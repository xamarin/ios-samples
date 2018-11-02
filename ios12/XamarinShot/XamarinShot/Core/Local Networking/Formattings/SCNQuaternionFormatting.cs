
namespace XamarinShot.Models.Formattings
{
    using System;
    using Newtonsoft.Json;
    using SceneKit;

    public class SCNQuaternionFormatting : JsonConverter<SCNQuaternion>
    {
        public override bool CanRead => false;

        public override SCNQuaternion ReadJson(JsonReader reader, Type objectType, SCNQuaternion existingValue, bool hasExistingValue, JsonSerializer serializer) => throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, SCNQuaternion value, JsonSerializer serializer)
        {
            var @object = new Newtonsoft.Json.Linq.JObject
            {
                { "X", Newtonsoft.Json.Linq.JToken.FromObject(value.X) },
                { "Y", Newtonsoft.Json.Linq.JToken.FromObject(value.Y) },
                { "Z", Newtonsoft.Json.Linq.JToken.FromObject(value.Z) },
                { "W", Newtonsoft.Json.Linq.JToken.FromObject(value.W) }
            };

            @object.WriteTo(writer);
        }
    }
}
