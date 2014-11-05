using System;
using Foundation;

namespace AppPrefs
{
	public enum TextColors {
		Blue = 1,
		Red,
		Green
	};

	public enum BackgroundColors {
		Black = 1,
		White,
		Blue,
		Pattern
	};
	
	/// <summary>
	/// This class manages the system settings.
	/// </summary>
	public class Settings
	{
		public static string FirstName { get; private set;}
		public static string LastName { get; private set;}
		public static TextColors TextColor { get; private set;}
		public static BackgroundColors BackgroundColor { get; private set;}
		
		const string firstNameKey = "firstNameKey";
		const string lastNameKey = "lastNameKey";
		const string nameColorKey = "nameColorKey";
		const string backgroundColorKey = "backgroundColorKey";
		
		static void LoadDefaultValues ()
		{
			var settingsDict = new NSDictionary (NSBundle.MainBundle.PathForResource ("Settings.bundle/Root.plist", null));
			
			var prefSpecifierArray = settingsDict[(NSString)"PreferenceSpecifiers"] as NSArray;
			
			foreach (var prefItem in NSArray.FromArray<NSDictionary> (prefSpecifierArray)) {
				var key = prefItem[(NSString)"Key"] as NSString;
				if (key == null)
					continue;
				var val = prefItem[(NSString)"DefaultValue"];
				switch (key.ToString ()) {
				case firstNameKey:
					FirstName = val.ToString ();
					break;
				case lastNameKey:
					LastName = val.ToString ();
					break;
				case nameColorKey:
					TextColor =  (TextColors)((NSNumber)val).Int32Value;
					break;
				case backgroundColorKey:
					BackgroundColor =  (BackgroundColors)((NSNumber)val).Int32Value;
					break;
				}
			}
			var appDefaults = NSDictionary.FromObjectsAndKeys (new object[] {
					new NSString (FirstName),
					new NSString (LastName),
					new NSNumber ((int)TextColor),
					new NSNumber ((int)BackgroundColor)
				},
				new object [] { firstNameKey, lastNameKey, nameColorKey, backgroundColorKey }
			);
			
			NSUserDefaults.StandardUserDefaults.RegisterDefaults (appDefaults);
			NSUserDefaults.StandardUserDefaults.Synchronize ();
		}
		
		public static void SetupByPreferences ()
		{
			var testValue = NSUserDefaults.StandardUserDefaults.StringForKey (firstNameKey);
			if (testValue == null)
				LoadDefaultValues ();
			FirstName = NSUserDefaults.StandardUserDefaults.StringForKey (firstNameKey);
			LastName = NSUserDefaults.StandardUserDefaults.StringForKey (lastNameKey);
			TextColor = (TextColors)(int)NSUserDefaults.StandardUserDefaults.IntForKey (nameColorKey);
			BackgroundColor = (BackgroundColors)(int)NSUserDefaults.StandardUserDefaults.IntForKey (backgroundColorKey);
		}
	}
}

