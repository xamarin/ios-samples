
namespace XamarinShot.Models.Formattings {
	using System;
	using Newtonsoft.Json;

	public class BoolFormatting : JsonConverter<bool> {
		public override bool CanRead => true;

		public override bool ReadJson (JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return reader.ReadAsBoolean () ?? false;
		}

		public override void WriteJson (JsonWriter writer, bool value, JsonSerializer serializer)
		{
			writer.WriteValue (value ? 1 : 0);
		}
	}
}
