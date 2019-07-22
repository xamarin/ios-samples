---
name: Xamarin.iOS - CloudKitAtlas
description: "CloudKitAtlas is a sample intended as a quick introduction to CloudKit. It teaches you how to use Discoverability... #ios8"
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: ios8-cloudkitatlas
---
# CloudKitAtlas

CloudKitAtlas is a sample intended as a quick introduction to
CloudKit. It teaches you how to use Discoverability to get the first
name and last name of the user logged into iCloud. It can add a
CKRecord with a location and query for CKRecords near a location. You
can upload and retrieve images as CKAssets. It also shows how to use
CKReferences. Finally, it also shows how to use CKSubscription to get
push notifications when a new item is added for a record type.

## Setting up sample

To run this sample you need to have an app ID with CloudKit
entitlements enabled and provisioning profile linked to it.  If you
don't have one please go to
https://developer.apple.com/membercenter/index.action, select
"Certificates, Identifiers & Profiles" and use the following
instructions.

Folow the instructions which can be found in [this guide](https://docs.microsoft.com/xamarin/ios/platform/document-picker#Enabling_iCloud_in_Xamarin), or follow the steps below.

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

# License

Xamarin port changes are released under the MIT license

# Author

Ported to Xamarin.iOS by Rustam Zaitov and Oleg Demchenko
