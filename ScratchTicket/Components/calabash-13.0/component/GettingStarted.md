Xamarin Test Cloud Agent consists of libraries that enable test-code to programmatically interact with native and hybrid apps. 

This component is also available as a [NuGet package](https://www.nuget.org/packages/Xamarin.TestCloud.Agent/). 

###So Easy###

It couldn't be any easier to incorporate the Xamarin Test Cloud Agent into your application. All it takes is just one line of code, which is added to your `AppDelegate`'s `FinishedLaunching` method:

```csharp
Xamarin.Calabash.Start();
```

Once up and running, the Xamarin Test Cloud Agent advertises itself using the [Bonjour protocol](https://developer.apple.com/bonjour/). This lets the client discover, connect, and begin testing your app.

In order for the Xamarin Test Cloud Agent to pull off its magic, it has to use some of Apple's private APIs. That means you don't want to ship your application with Xamarin Test Cloud Agent bits still in it.

To make your life easy, we leverage our sophisticated linker which removes code that isn't needed. Just wrap your call to `Start` Xamarin Test Cloud in an `#if` directive:

```csharp
#if DEBUG
Xamarin.Calabash.Start();
#endif
```

This will ensure that your release build will not include any of the Xamarin 
Test Cloud bits.
