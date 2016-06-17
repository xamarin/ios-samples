using System;
using System.Linq;

using Foundation;
using UIKit;

namespace ApplicationShortcuts
{
	enum ShortcutIdentifierType
	{
		First,
		Second,
		Third,
		Fourth,
	}

	static class ShortcutIdentifierTypeExtensions
	{
		public static string GetTypeName (this ShortcutIdentifierType self)
		{
			return string.Format ("{0} {1}", NSBundle.MainBundle.BundleIdentifier, self);
		}
	}

	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		public static readonly NSString ApplicationShortcutUserInfoIconKey = (NSString)"applicationShortcutUserInfoIconKey";

		class ShortcutIdentifier
		{
			public ShortcutIdentifierType Type { get; private set; }

			ShortcutIdentifier ()
			{
			}

			public static ShortcutIdentifier Create(string fullType)
			{
				if (string.IsNullOrWhiteSpace (fullType))
					return null;

				string last = fullType.Split (new []{ '.' }, StringSplitOptions.RemoveEmptyEntries).Last ();

				ShortcutIdentifierType type;
				bool isParsed = Enum.TryParse<ShortcutIdentifierType> (last, out type);
				return isParsed ? new ShortcutIdentifier { Type = type } : null;
			}
		}

		public override UIWindow Window { get; set; }

		// Saved shortcut item used as a result of an app launch, used later when app is activated.
		UIApplicationShortcutItem launchedShortcutItem;

		bool HandleShortCutItem(UIApplicationShortcutItem shortcutItem)
		{
			// Verify that the provided `shortcutItem`'s `type` is one handled by the application.
			var shortcutIdentifier = ShortcutIdentifier.Create (shortcutItem.Type);
			if (shortcutIdentifier == null)
				return false;

			// Construct an alert using the details of the shortcut used to open the application.
			var alertController = UIAlertController.Create ("Shortcut Handled", shortcutItem.LocalizedTitle, UIAlertControllerStyle.Alert);
			var okAction = UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null);
			alertController.AddAction (okAction);

			// Display an alert indicating the shortcut selected from the home screen.
			var rootViewController = Window.RootViewController;
			if (rootViewController != null)
				rootViewController.PresentViewController (alertController, true, null);

			return true;
		}

		public override void OnActivated (UIApplication application)
		{
			var shortcut = launchedShortcutItem;
			if (shortcut == null)
				return;

			HandleShortCutItem (shortcut);
			launchedShortcutItem = null;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			var shouldPerformAdditionalDelegateHandling = true;

			// If a shortcut was launched, display its information and take the appropriate action
			if(launchOptions != null) {
				var shortcutItem = launchOptions [UIApplication.LaunchOptionsShortcutItemKey] as UIApplicationShortcutItem;
				if (shortcutItem != null) {
					launchedShortcutItem = shortcutItem;

					// This will block "performActionForShortcutItem:completionHandler" from being called.
					shouldPerformAdditionalDelegateHandling = false;
				}
			}

			// Install initial verstions of our two extra dynamic shortcuts.
			// Update the application providing the initial 'dynamic' shortcut items.
			if (application.ShortcutItems.Length == 0)
				application.ShortcutItems = new [] { CreatePlayShortcut (), CreatePauseShortcut () };

			return shouldPerformAdditionalDelegateHandling;
		}

		static UIMutableApplicationShortcutItem CreatePlayShortcut()
		{
			string type = ShortcutIdentifierType.Third.GetTypeName ();
			var icon = CreateIcon (UIApplicationShortcutIconType.Play);
			var userInfo = CreateUserInfo(UIApplicationShortcutIconType.Play);
			return new UIMutableApplicationShortcutItem (type, "Play", "Will Play an item", icon, userInfo);
		}

		static UIMutableApplicationShortcutItem CreatePauseShortcut ()
		{
			var type = ShortcutIdentifierType.Fourth.GetTypeName ();
			var icon = CreateIcon(UIApplicationShortcutIconType.Pause);
			var userInfo = CreateUserInfo (UIApplicationShortcutIconType.Pause);
			return new UIMutableApplicationShortcutItem (type, "Pause", "Will Pause an item", icon, userInfo);
		}

		static NSDictionary<NSString, NSObject> CreateUserInfo(UIApplicationShortcutIconType type)
		{
			int rawValue = Convert.ToInt32 (type);
			return new NSDictionary<NSString, NSObject>(ApplicationShortcutUserInfoIconKey, new NSNumber (rawValue));
		}

		static UIApplicationShortcutIcon CreateIcon (UIApplicationShortcutIconType type)
		{
			return UIApplicationShortcutIcon.FromType (type);
		}

		// Called when the user activates your application by selecting a shortcut on the home screen, except when 
		// WillFinishLaunching (UIApplication, NSDictionary) or FinishedLaunching (UIApplication, NSDictionary) returns `false`.
		// You should handle the shortcut in those callbacks and return `false` if possible. In that case, this 
		// callback is used if your application is already launched in the background.
		public override void PerformActionForShortcutItem (UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler)
		{
			bool handledShortCutItem = HandleShortCutItem (shortcutItem);
			completionHandler (handledShortCutItem);
		}
	}
}