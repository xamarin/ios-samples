using System;

using Foundation;

namespace ListerKit
{
	[Register("ListItem")]
	public class ListItem: NSObject, ICloneable, INSCoding
	{
		const string TextKey = "text";
		const string CompletedKey = "completed";
		const string UUIDKey = "uuid";

		public string Text { get; set; }
		public bool IsComplete { get; set; }
		public Guid UID { get; private set; }

		public ListItem (string text, bool isComplete, Guid uid)
		{
			Text = text;
			IsComplete = isComplete;
			UID = uid;
		}

		public ListItem(string text, bool isComplete)
			: this(text, isComplete, Guid.NewGuid())
		{

		}

		public ListItem(string text)
			: this(text, false)
		{
		}

		[Export("initWithCoder:")]
		public ListItem(NSCoder coder)
		{
			Text = (string)(NSString)coder.DecodeObject (TextKey);
			NSUuid uid = (NSUuid)coder.DecodeObject (UUIDKey);
			UID = new Guid (uid.GetBytes());
			IsComplete = coder.DecodeBool (CompletedKey);
		}

		[Export ("encodeWithCoder:")]
		public void EncodeTo (NSCoder coder)
		{
			coder.Encode ((NSString)Text, TextKey);
			coder.Encode (new NSUuid(UID.ToByteArray ()), UUIDKey);
			coder.Encode (IsComplete, CompletedKey);
		}

		public void RefreshIdentity()
		{
			UID = Guid.Empty;
		}

		// TODO: how does app use it?
		public object Clone ()
		{
			return new ListItem (Text, IsComplete, UID);
		}

		// TODO: ensure that this method calls
		public override bool IsEqual (NSObject anObject)
		{
			if (anObject == null)
				return false;

			if (anObject.GetType () != typeof(ListItem))
				return false;

			ListItem second = (ListItem)anObject;
			return UID = second.UID;
		}

		public override string ToString ()
		{
			return string.Format ("[ListItem: Text={0}, IsComplete={1}, UID={2}]", Text, IsComplete, UID);
		}
	}
}