using System;
using System.Runtime.Serialization;
namespace MusicKitSample.Models
{
	public class SerializationException : Exception
	{
		#region Properties

		public string JsonKey { get; private set; }

		public override string Message {
			get {
				string message = base.Message;

				if (!string.IsNullOrEmpty (JsonKey))
					return message + Environment.NewLine + $"The key {JsonKey} is missing.";
				
				return message;
			}
		}

		#endregion

		#region Constructors

		public SerializationException () : this (string.Empty)
		{
		}

		public SerializationException (string jsonKey)
		{
			JsonKey = jsonKey;
		}

		public SerializationException (string jsonKey, string message) : base (message)
		{
			JsonKey = jsonKey;
		}

		public SerializationException (string jsonKey, string message, Exception inner) : base (message, inner)
		{
			JsonKey = jsonKey;
		}

		public SerializationException (SerializationInfo info, StreamingContext context) : base (info, context)
		{
		}

		#endregion
	}
}
