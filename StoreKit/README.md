---
name: Xamarin.iOS - In-App Purchase Samples
description: 'This sample contains four projects: Consumable purchases docs Non-Consumable purchases docs Non-Consumable purchases using iOS 6 hosted content...'
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: storekit
---
# In-App Purchase Samples

This sample contains four projects:

* Consumable purchases [docs](http://docs.xamarin.com/ios/tutorials/In-App_Purchasing)

* Non-Consumable purchases [docs](http://docs.xamarin.com/ios/tutorials/In-App_Purchasing)

* Non-Consumable purchases using iOS 6 hosted content [docs](http://docs.xamarin.com/ios/tutorials/Introduction_to_iOS_6/Changes_to_StoreKit)

* ProductView using iOS 6 feature purchase iTunes/App Store/iBookstore content in your app [docs](http://docs.xamarin.com/ios/tutorials/Introduction_to_iOS_6/Changes_to_StoreKit)

There is also a directory containing the Xcode projects that are used to build Hosted Content for iOS 6 in-app purchases.


## In-App Purchasing
![screenshot](https://github.com/xamarin/monotouch-samples/raw/master/StoreKit/Screenshots/01-Consumable.png "Consumable") ![screenshot](https://github.com/xamarin/monotouch-samples/raw/master/StoreKit/Screenshots/02-NonConsumable.png "NonConsumable") ![screenshot](https://github.com/xamarin/monotouch-samples/raw/master/StoreKit/Screenshots/03-Hosted.png "Hosted")


NOTE: it does NOT demonstrate RECEIPT VERIFICATION, so you'll have to add this in yourself. 

You might also consider services like http://urbanairship.com/ or http://www.beeblex.com/ (although I have not tried them, so can't recommend).

Check out @redth's server-side code to help build your own receipt verification logic with ASP.NET:

https://github.com/Redth/APNS-Sharp/tree/master/JdSoft.Apple.AppStore

FYI the sample code is based in-part on @jtclancey's AppStore code here: 

https://github.com/Clancey/ClanceyLib

## Store Product View
This sample demonstrates the new iOS 6 SKStoreProductViewController that lets applications display iTunes, App Store and iBookstore products for review or purchase.

![screenshot](https://github.com/xamarin/monotouch-samples/raw/master/StoreKit/Screenshots/04-ProductView.png "ProductView")


## Setup

There's a bit of set-up required for In-App Purchases (registering your bank details with Apple, setting up the products in the iOS Developer Portal, Provisioning your app correctly). These steps are the same for MonoTouch and Objective-C, so Apple's setup doco might help [1]. You should also read Apple's In-App Purchase programming docs [2], for familiarity.

[1] http://developer.apple.com/library/ios/#technotes/tn2259/_index.html

[2] https://developer.apple.com/library/ios/#documentation/NetworkingInternet/Conceptual/StoreKitGuide/Introduction/Introduction.html