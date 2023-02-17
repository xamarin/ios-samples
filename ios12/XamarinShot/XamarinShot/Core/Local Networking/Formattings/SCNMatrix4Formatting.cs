
namespace XamarinShot.Models.Formattings {
	using System;
	using Newtonsoft.Json;
	using SceneKit;

	public class SCNMatrix4Formatting : JsonConverter<SCNMatrix4> {
		public override bool CanRead => false;

		public override SCNMatrix4 ReadJson (JsonReader reader, Type objectType, SCNMatrix4 existingValue, bool hasExistingValue, JsonSerializer serializer) => throw new NotImplementedException ();

		public override void WriteJson (JsonWriter writer, SCNMatrix4 value, JsonSerializer serializer)
		{
			var @object = new Newtonsoft.Json.Linq.JObject
			{
				{ "Row0", Newtonsoft.Json.Linq.JToken.FromObject(value.Row0, serializer) },
				{ "Row1", Newtonsoft.Json.Linq.JToken.FromObject(value.Row1, serializer) },
				{ "Row2", Newtonsoft.Json.Linq.JToken.FromObject(value.Row2, serializer) },
				{ "Row3", Newtonsoft.Json.Linq.JToken.FromObject(value.Row3, serializer) },
			};

			@object.WriteTo (writer);
		}
	}
}
