using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using UIKit;
using Foundation;

namespace Cloud
{
	/// <summary>
	/// View containing Buttons and TextView to show off the samples
	/// </summary>
	public class KeyValueViewController : UIViewController
	{
		UIButton saveButton, reloadButton, clearButton;
		UITextView localText, testText, outputText;
		UILabel localLabel, testLabel, keyLabel, valueLabel;
		
		string sharedKeyName = "Shared";

		public override void ViewDidLoad ()
		{	
			base.ViewDidLoad ();
			
			#region UI Layout
			// Create the buttons and TextView to run the sample code
			saveButton = UIButton.FromType(UIButtonType.RoundedRect);
			saveButton.Frame = new CGRect(10,120,90,50);
			saveButton.SetTitle("Save", UIControlState.Normal);

			reloadButton = UIButton.FromType(UIButtonType.RoundedRect);
			reloadButton.Frame = new CGRect(110,120,90,50);
			reloadButton.SetTitle("Reload", UIControlState.Normal);
			
			clearButton = UIButton.FromType(UIButtonType.RoundedRect);
			clearButton.Frame = new CGRect(210,120,90,50);
			clearButton.SetTitle("Clear", UIControlState.Normal);
			
			outputText = new UITextView(new CGRect(10, 180, 300, 270));
			//outputText.BackgroundColor = UIColor.FromRGB(224,255,255);
			outputText.Editable = false;
			outputText.ScrollEnabled = true;
			
			localText = new UITextView(new CGRect (130, 40, 150, 30));
			localText.BackgroundColor = UIColor.FromRGB(224,255,255);
			testText = new UITextView(new CGRect (130, 80, 150, 30));
			testText.BackgroundColor = UIColor.FromRGB(224,255,255);

			localLabel = new UILabel(new CGRect(10,  40, 100, 30));
			localLabel.Text = UIDevice.CurrentDevice.Name + ":";
			testLabel  = new UILabel(new CGRect(10, 80, 100, 30));
			testLabel.Text = "Shared:";
			keyLabel  = new UILabel(new CGRect(10, 10, 200, 30));
			keyLabel.Text = "KEY";
 			valueLabel = new UILabel(new CGRect(150, 10, 200, 30));
			valueLabel.Text = "VALUE";

			// Add the controls to the view
			this.Add(saveButton);
			this.Add(reloadButton);
			this.Add(clearButton);
			this.Add(outputText);
			Add (testText);
			Add (localText);
			Add (localLabel);
			Add (testLabel);
			Add (keyLabel);
			Add (valueLabel);
			#endregion
			
			// Wire up the buttons to the SamplCode class methods
			saveButton.TouchUpInside += Save;

			reloadButton.TouchUpInside += Reload;
			
			clearButton.TouchUpInside += Clear;
		}
		
		// saves the inputs to iCloud keys
		void Save (object sender, EventArgs ea) {
			var store = NSUbiquitousKeyValueStore.DefaultStore;
			store.SetString(UIDevice.CurrentDevice.Name, localText.Text);
			store.SetString(sharedKeyName, testText.Text);
			var synchronized = store.Synchronize();
			outputText.Text += String.Format ("\n--- Local save ({4}) ---\n{0}: {1}\n{2}: {3}"
						,UIDevice.CurrentDevice.Name, localText.Text
						, sharedKeyName, testText.Text
						, synchronized);

			localText.ResignFirstResponder ();
			testText.ResignFirstResponder ();
		}
		// loads the inputs from iCloud keys
		void Reload (object sender, EventArgs ea) {
			var store = NSUbiquitousKeyValueStore.DefaultStore;
			var synchronized = store.Synchronize(); 
		
			testText.Text = store.GetString(sharedKeyName);
			localText.Text = store.GetString(UIDevice.CurrentDevice.Name);

			outputText.Text += String.Format ("\n--- Reload ({4}) ---\n{0}: {1}\n{2}: {3}"
							,UIDevice.CurrentDevice.Name, localText.Text
							, sharedKeyName, testText.Text
							, synchronized);

			localText.ResignFirstResponder ();
			testText.ResignFirstResponder ();
		}
		// clears the iCloud keys
		void Clear (object sender, EventArgs e) {
			var store = NSUbiquitousKeyValueStore.DefaultStore;
			store.Remove(UIDevice.CurrentDevice.Name);
			store.Remove(sharedKeyName);

			var synchronized = store.Synchronize();
			outputText.Text += String.Format ("\n--- Clear ({4}) ---\n{0}: {1}\n{2}: {3}"
						,UIDevice.CurrentDevice.Name, "<cleared>"
						, sharedKeyName, "<cleared>"
						, synchronized);
			localText.ResignFirstResponder ();
			testText.ResignFirstResponder ();
		}
		
		// notification when Key-Value changes are triggered by server
		NSObject keyValueNotification;
		
		// register for the notification when iCloud keys are changed
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			
			Reload (null,null);

			keyValueNotification = 
			NSNotificationCenter.DefaultCenter.AddObserver (
				NSUbiquitousKeyValueStore.DidChangeExternallyNotification
				, delegate (NSNotification n)
			{
				Console.WriteLine("Cloud notification received");
				NSDictionary userInfo = n.UserInfo;
			
				NSNumber reasonNumber = (NSNumber)userInfo.ObjectForKey(NSUbiquitousKeyValueStore.ChangeReasonKey);
				nint reason = reasonNumber.NIntValue;
				
				//Console.WriteLine("reason.IntValue: " + ireason);
				//Console.WriteLine("reason (enum): " + (NSUbiquitousKeyValueStoreChangeReason)ireason);
			
				NSArray changedKeys = (NSArray)userInfo.ObjectForKey (NSUbiquitousKeyValueStore.ChangedKeysKey);
				var changedKeysList = new List<string> ();
				for (uint i = 0; i < changedKeys.Count; i++)
				{
					var key = changedKeys.GetItem<NSString>(i); // resolve key to a string

					//Console.WriteLine("changedKey IntPtr: " + changedKeys.ValueAt(i));
					//Console.WriteLine("changedKey (value): " + key);
					
					changedKeysList.Add (key);
				}

				var store = NSUbiquitousKeyValueStore.DefaultStore;
				store.Synchronize(); 
					// now do something with the list...
					InvokeOnMainThread (()=>{
					outputText.Text += "\n--- Cloud Notification \uE049\uE049\uE049 ---";
					foreach (var k in changedKeysList) {
						outputText.Text += String.Format ("\n{0}: {1}",k, store.GetString(k));
					}
					testText.Text = store.GetString(sharedKeyName);
					localText.Text = store.GetString(UIDevice.CurrentDevice.Name);
				});
			});
		}

		// remove notification observer
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			if (keyValueNotification != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver (keyValueNotification);
		}
	}
}