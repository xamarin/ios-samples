DocPicker
======================

DocPicker accompanies to the [Introduction to the Document Picker](http://developer.xamarin.com/guides/ios/platform_features/introduction_to_the_document_picker/) guide in the developer portal.

## Setting up sample

To run this sample you need to have an app ID with CloudKit
entitlements enabled and provisioning profile linked to it.  

Follow the steps in the [Introduction to the Document Picker](http://developer.xamarin.com/guides/ios/platform_features/introduction_to_the_document_picker/#Enabling_iCloud_in_Xamarin) guide to set up your provisioning profile. These are also listed below.

If you don't have one please go to
https://developer.apple.com/membercenter/index.action, select
"Certificates, Identifiers & Profiles" and use the following
instructions.

First of all, you should create an app ID.  To do that, select "App
ID's" in "Identifiers" section and then press add button at top-right
corner of the screen.

* Enter app ID description, e.g.: "CloudKit Atlas sample"
* Select "Explicit App ID" in App ID Suffix and enter bundle ID, e.g.:
  com.yourcompanyname.CloudKitAtlas
* In App Services services section select iCloud, Include CloudKit support
* Then press Continue button and check that everything is alright
* Press Submit

On the second step we should create provisioning profile linked to
newly created app ID. Select "All" in "Provisioning Profiles" section
and then press add button at top-right corner of the screen.

* Select "iOS App Development" and press Continue button
* Select newly created app ID in drop down list and press Continue button
* Then select your team members and press Continue button
* Select your devices and press Continue button
* Enter profile name, e.g.: "CloudKit Atlas Development"
* Download provisioning profile and then double click it to install

Finally, open sample in Xamarin Studio and open project settings.

* Set bundle identifier in "iOS Application" section and select
  provisioning profile created earlier in "iOS Bundle Signing".
* Close project's options and select Entitlements.plist in project
  explorer then nable iCloud and check the CloudKit option.
* Make sure you are signed into your iCloud account in the simulator
  or device before running the app.