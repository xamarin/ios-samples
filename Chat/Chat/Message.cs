using System;

namespace Chat
{
	public enum MessageType
	{
		Left,
		Right
	}

	public class Message
	{
		public MessageType Type { get; set; }
		public string Text { get; set; }
	}
}

