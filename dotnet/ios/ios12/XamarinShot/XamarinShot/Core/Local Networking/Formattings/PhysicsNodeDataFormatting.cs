namespace XamarinShot.Models.Formattings;

public class PhysicsNodeDataFormatting : JsonConverter<PhysicsNodeData>
{
        public override bool CanRead => true;

        public override PhysicsNodeData? ReadJson (JsonReader reader, Type objectType, PhysicsNodeData? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
                var result = default (PhysicsNodeData);
                if (reader.TokenType != JsonToken.Null)
                {
                        result = new PhysicsNodeData ();

                        var jsonObject = JObject.Load (reader);
                        if (jsonObject is null)
                                return result;
                        result.IsAlive = jsonObject [nameof (result.IsAlive)]?.Value<bool> () ?? false;
                        if (result.IsAlive)
                        {
                                result.IsMoving = jsonObject [nameof (result.IsAlive)]?.Value<bool> () ?? false;
                                result.Team = (Team)(jsonObject [nameof (result.Team)]?.Value<int> () ?? 0);

                                result.Position = jsonObject [nameof (result.Position)]?.ToObject<SCNVector3> (serializer) ?? SCNVector3.Zero; ;
                                result.Orientation = jsonObject [(nameof (result.Orientation))]?.ToObject<SCNQuaternion> (serializer) ?? SCNQuaternion.Identity;

                                if (result.IsMoving)
                                {
                                        result.Velocity = jsonObject [nameof (result.Velocity)]?.ToObject<SCNVector3> (serializer) ?? SCNVector3.Zero;
                                        result.AngularVelocity = jsonObject [nameof (result.AngularVelocity)]?.ToObject<SCNVector4> (serializer) ?? SCNVector4.Zero;
                                }
                        }
                }

                return result;
        }

        public override void WriteJson (JsonWriter writer, PhysicsNodeData? value, JsonSerializer serializer)
        {
                var jsonObject = new JObject ();
                jsonObject.Add (nameof (value.IsAlive), JToken.FromObject (value?.IsAlive ?? false, serializer));

                if (value is not null && value.IsAlive)
                {
                        jsonObject.Add (nameof (value.IsMoving), JToken.FromObject (value.IsMoving, serializer));
                        jsonObject.Add (nameof (value.Team), JToken.FromObject (value.Team, serializer));
                        jsonObject.Add (nameof (value.Position), JToken.FromObject (value.Position, serializer));
                        jsonObject.Add (nameof (value.Orientation), JToken.FromObject (value.Orientation, serializer));

                        if (value.IsMoving)
                        {
                                jsonObject.Add (nameof (value.Velocity), JToken.FromObject (value.Velocity, serializer));
                                jsonObject.Add (nameof (value.AngularVelocity), JToken.FromObject (value.AngularVelocity, serializer));
                        }
                }

                jsonObject.WriteTo (writer);
        }
}
