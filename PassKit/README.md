PassKit Sample
==============

This sample demonstrates how to build a PassKit Companion App to interact with passes you have issued in a users Passbook.

NOTE: For this sample you need to register BundleId with enabled PassKit. Also create PassTypeId on iOS Provisioning Portal. Be sure to check the Entitlements.plist - you must provide value for `com.apple.developer.pass-type-identifiers` key and ensure that you replace BundleId within Info.plist.

Refer to the [Introduction to PassKit](http://docs.xamarin.com/ios/tutorials/Introduction_to_PassKit) documentation on the Xamarin website.

![screenshot](https://github.com/xamarin/monotouch-samples/raw/master/PassKit/Screenshots/01-PassLibrary.png "PassLibrary")

Also refer to Apple's [Passbook for Developers](https://developer.apple.com/passbook/) site.
