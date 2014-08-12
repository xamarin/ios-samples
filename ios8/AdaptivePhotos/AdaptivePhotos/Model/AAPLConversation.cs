using System;
using MonoTouch.Foundation;

namespace AdaptivePhotos
{
	public class AAPLConversation : NSObject
	{
		public NSArray Photos { get; private set; }

		public string Name { get; private set; }

		public static AAPLConversation ConversationWithDictionary (NSDictionary dictionary)
		{
			var photoValues = (NSArray)dictionary.ObjectForKey (new NSString ("photos"));
			var photos = new NSMutableArray (photoValues.Count);

			for (int i = 0; i < photoValues.Count; i++) {
				var photo = AAPLPhoto.PhotoWithDictionary (photoValues.GetItem <NSDictionary> (i));
				photos.Add (photo);
			}

			return new AAPLConversation {
				Name = (NSString)dictionary.ObjectForKey (new NSString ("name")),
				Photos = photos
			};
		}
	}
}

