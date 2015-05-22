`AlertCenter` displays notifications in a manner similar to native iOS
banner notifications.

## iOS Example

```csharp
using Xamarin.Controls;
...

public override void ViewDidLoad ()
{
	base.ViewDidLoad ();

	AlertCenter.Default.PostMessage ("Knock knock!", "Who's there?");
	AlertCenter.Default.PostMessage ("Interrupting cow.", "Interrupting cow who?",
		                        UIImage.FromFile ("cow.png"), delegate {
		Console.WriteLine ("Moo!");
	});
}
```

## Android Example

Using `AlertCenter` on Android is similar to iOS, except that you need
to initialize the `AlertCenter` with a `Context`. The permission
`SYSTEM_ALERT_WINDOW` will also need to be declared in your `AndroidManfiest.xml`.

```csharp
using Xamarin.Controls;
...

protected override void OnCreate (Bundle bundle)
{
	base.OnCreate (bundle);
			
	AlertCenter.Default.Init (Application);
			
	AlertCenter.Default.PostMessage ("Knock knock!", "Who's there?", Resource.Drawable.Icon);
	AlertCenter.Default.PostMessage ("Interrupting cow.", "Interrupting cow who?",
		                        Resource.Drawable.Icon, () => {
		Console.WriteLine ("Moo!");
	});
}
```
