using System;

using Foundation;

namespace Common
{
	[Register("ListItem")]
	public class ListItem: NSObject, ICloneable, INSCoding
	{
		const string ListItemEncodingTextKey = "text";
		const string ListItemEncodingCompletedKey = "completed";
		const string ListItemEncodingUUIDKey = "uuid";

		public string Text { get; set; }
		public bool IsComplete { get; set; }
		public Guid UID { get; private set; }

		public ListItem (string text, bool isComplete, Guid uid)
		{
			Text = text;
			IsComplete = isComplete;
			UID = uid;
		}

		public ListItem(string text)
			: this(text, false, Guid.NewGuid())
		{
		}

		[Export("initWithCoder:")]
		public ListItem(NSCoder coder)
		{
			Text = (string)(NSString)coder.DecodeObject (ListItemEncodingTextKey);
			NSUuid uid = (NSUuid)coder.DecodeObject (ListItemEncodingUUIDKey);
			UID = new Guid (uid.GetBytes());
			IsComplete = coder.DecodeBool (ListItemEncodingCompletedKey);
		}

		[Export ("encodeWithCoder:")]
		public void EncodeTo (NSCoder coder)
		{
			coder.Encode ((NSString)Text, ListItemEncodingTextKey);
			coder.Encode (new NSUuid(UID.ToByteArray ()), ListItemEncodingUUIDKey);
			coder.Encode (IsComplete, ListItemEncodingCompletedKey);
		}

		public void RefreshIdentity()
		{
			UID = Guid.Empty;
		}

		#region ICloneable implementation

		public object Clone ()
		{
			return new ListItem (Text, IsComplete, UID);
		}

		#endregion
	}
}

