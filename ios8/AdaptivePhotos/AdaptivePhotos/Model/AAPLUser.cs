using System;
using MonoTouch.Foundation;

namespace AdaptivePhotos
{
	public class AAPLUser
	{
		public string Name { get; private set; }

		public NSArray Conversations { get; private set; }

		public AAPLPhoto LastPhoto { get; set; }

		public static AAPLUser UserWithDictionary (NSDictionary dictionary)
		{
			string name = (NSString)dictionary.ObjectForKey (new NSString ("name"));
			var conversationDictionaries = (NSArray)dictionary.ObjectForKey (new NSString ("conversations"));
			var conversations = new NSMutableArray (conversationDictionaries.Count);

			for (int i = 0; i < conversationDictionaries.Count; i++) {
				var conversation = AAPLConversation.ConversationWithDictionary (conversationDictionaries.GetItem<NSDictionary> (i));
				conversations.Add (conversation);
			}

			var lastPhotoDictionary = NSDictionary.FromDictionary ((NSDictionary)dictionary.ObjectForKey (new NSString ("lastPhoto")));

			return new AAPLUser {
				Name = name,
				Conversations = conversations,
				LastPhoto = AAPLPhoto.PhotoWithDictionary (lastPhotoDictionary)
			};
		}
	}
}

