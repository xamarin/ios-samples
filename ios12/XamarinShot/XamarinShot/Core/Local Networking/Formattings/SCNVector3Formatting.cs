
namespace XamarinShot.Models.Formattings
{
    using System;
    using Newtonsoft.Json;
    using SceneKit;

    public class SCNVector3Formatting : JsonConverter<SCNVector3>
    {
        public override bool CanRead => false;

        public override SCNVector3 ReadJson(JsonReader reader, Type objectType, SCNVector3 existingValue, bool hasExistingValue, JsonSerializer serializer) => throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, SCNVector3 value, JsonSerializer serializer)
        {
            var @object = new Newtonsoft.Json.Linq.JObject
            {
                { "X", Newtonsoft.Json.Linq.JToken.FromObject(value.X) },
                { "Y", Newtonsoft.Json.Linq.JToken.FromObject(value.Y) },
                { "Z", Newtonsoft.Json.Linq.JToken.FromObject(value.Z) }
            };

            @object.WriteTo(writer);
        }
    }
}
