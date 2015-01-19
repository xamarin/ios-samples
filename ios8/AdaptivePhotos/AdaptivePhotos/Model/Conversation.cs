using System;
using Foundation;

namespace AdaptivePhotos
{
	public class Conversation : NSObject
	{
		public NSArray Photos { get; private set; }

		public string Name { get; private set; }

		public static Conversation ConversationWithDictionary (NSDictionary dictionary)
		{
			var photoValues = (NSArray)dictionary.ObjectForKey (new NSString ("photos"));
			var photos = new NSMutableArray (photoValues.Count);

			for (nuint i = 0; i < photoValues.Count; i++) {
				var photo = Photo.PhotoWithDictionary (photoValues.GetItem <NSDictionary> (i));
				photos.Add (photo);
			}

			return new Conversation {
				Name = (NSString)dictionary.ObjectForKey (new NSString ("name")),
				Photos = photos
			};
		}
	}
}

