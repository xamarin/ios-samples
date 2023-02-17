watchOS
=======

Xamarin has [watchOS](https://blog.xamarin.com/let-the-ios10-ing-begin/) support:

1. Add a new **watchOS > App > WatchKit App** project to an iOS 10 solution.
2. Right-click the **watchOS app** project and select **Set as startup project**
3. The simulator list should change to display a 38mm or 42mm watch option. If it doesn't change, restart Visual Studio for Mac and try again.
4. With a *watch simulator* selected, start debugging.

Refer to the [Xamarin watchOS docs](https://developer.xamarin.com/guides/ios/watch/) for more information. Also see the [release notes](https://developer.xamarin.com/releases/ios/xamarin.ios_10/xamarin.ios_10.0/#watchOS) and [API diff](https://developer.xamarin.com/releases/ios/api_changes/watchos_9.10.0_to_10.0.0/). 


Requirements
--------------------

watchOS has the following minimum requirements:

* Xcode 8.0 (download from Apple)
* Xamarin Studio 6.1 (available in Stable channel)
* Xamarin.iOS 10 (available in Stable channel)
 
Please raise any issues on [bugzilla](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS&component=Xamarin.WatchOS.dll).
