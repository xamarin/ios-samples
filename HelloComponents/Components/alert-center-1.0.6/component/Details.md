`AlertCenter` displays notifications in a manner similar to native iOS
banner notifications.

```csharp
using Xamarin.Controls;
...

public override void ViewDidLoad ()
{
	base.ViewDidLoad ();

	//the notification banner will be shown for 3 seconds
	AlertCenter.Default.TimeToClose = new TimeSpan (0, 0, 0, 3, 0);
	
	AlertCenter.Default.BackgroundColor = UIColor.Red;

	AlertCenter.Default.PostMessage ("Knock knock!", "Who's there?");

	AlertCenter.Default.PostMessage ("Interrupting cow.", "Interrupting cow who?",
		UIImage.FromFile ("cow.png"), delegate {
		Console.WriteLine ("Moo!");
	});
}
```


