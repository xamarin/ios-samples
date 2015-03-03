PassKit Sample
==============

This sample demonstrates how to build a PassKit Companion App to interact with passes you have issued in a users Passbook.

NOTE: This sample doen't work out of the box, you need to setup provisioning profile for you app and create certicicate for pass singing.

Instructions
------------

* Create an AppId and enable `Passbook`
* Generate `Provisioning Profile` for your app
* Register new `iOS Pass Type ID`
* Generate Certificate for your just created `iOS Pass Type ID` via `Edit > Create Certificate`
* Download and install your certificate for pass

Now you need to fix some files:  
* `Entitlements.plist` and `Info.Plist` – fix `BundleIdentifier` and `PassTypeIdentifiers`
* `CouponBanana2.pass/pass.json` – fix `passTypeIdentifier` and `teamIdentifier`

At this point you are ready to generate pass package. For sample we provide simpe util which create package for you. Go to terminal and run:

```
cd path/to/PassLibrary/sample
./signpass -p CouponBanana2.pass/ -c "Certificate_Common_Name"
```

You can find Certificate_Common_Name with `Keychain Access` app:

* Launch `Keychain Access` app
* Find certificate which you installed a few steps ago
* Select certificate > Right Click > Get Info
* Here you are able to see `Common Name`

At this point you should be able to find CouponBanana2.pkpass. Go to IDE, compile and run you app.

Refer to the [Introduction to PassKit](http://docs.xamarin.com/ios/tutorials/Introduction_to_PassKit) documentation on the Xamarin website.

![screenshot](https://github.com/xamarin/monotouch-samples/raw/master/PassKit/Screenshots/01-PassLibrary.png "PassLibrary")

Also refer to Apple's [Passbook for Developers](https://developer.apple.com/passbook/) site.
