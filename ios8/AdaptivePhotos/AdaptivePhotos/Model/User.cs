using System;
using Foundation;

namespace AdaptivePhotos
{
	public class User
	{
		public string Name { get; private set; }

		public NSArray Conversations { get; private set; }

		public Photo LastPhoto { get; set; }

		public static User UserWithDictionary (NSDictionary dictionary)
		{
			string name = (NSString)dictionary.ObjectForKey (new NSString ("name"));
			var conversationDictionaries = (NSArray)dictionary.ObjectForKey (new NSString ("conversations"));
			var conversations = new NSMutableArray (conversationDictionaries.Count);

			for (nuint i = 0; i < conversationDictionaries.Count; i++) {
				var conversation = Conversation.ConversationWithDictionary (conversationDictionaries.GetItem<NSDictionary> (i));
				conversations.Add (conversation);
			}

			var lastPhotoDictionary = NSDictionary.FromDictionary ((NSDictionary)dictionary.ObjectForKey (new NSString ("lastPhoto")));

			return new User {
				Name = name,
				Conversations = conversations,
				LastPhoto = Photo.PhotoWithDictionary (lastPhotoDictionary)
			};
		}
	}
}

