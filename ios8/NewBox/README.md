---
name: Xamarin.iOS - NewBox
description: 'Shows you how to: Use UIDocumentMenuViewController and UIDocumentMenuViewController. Create Document Provider extensions #ios8'
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: ios8-newbox
---
# NewBox

This sample shows you how to:
* Use `UIDocumentMenuViewController` and `UIDocumentMenuViewController`.
* Create Document Provider extensions: DocumentPickerViewController Extension and FileProvider Extension.
* Work with built-in iCloud storage provider.
 

We strongly recommend you to read the following guides before you start:
* [App Extension Programming Guide]
* [Document Picker Programming Guide]

When you run the app you will see two columns. First column shows Document Picker directly and the second shows you Document Picker Menu. Document Picker Menu allows you to select Document Picker and add custom options.

Document Provider extension consists in two parts: Document picker extension and File provider extension. These extensions are presented with the following projects:
* `NBox` – Document picker extension.
* `NBoxFileProvider` – File provider extension.

## Setup
You need to setup an Application IDs with App Groups and iCloud. Then you need to generate a provisioning profile linked to the newly created AppID.
To setup AppIDs, App Groups, Provisioning profiles please look at [this introduction]. Also have a look at next section, here you will find step by step instructions for Xamarin Studio (the same should work in Visual Studio)

iOS 8.0 provides a single and built-in iCloud storage provider. The goal of this sample is to add an other storage provider.

To start using the Document Picker and Open/Import from the iCloud storage Provider, you need to create some documents in iCloud iOS 8. The easiest way to do so is to use one of the following apps:
* Pages
* Numbers
* Keynote

You can donwload these apps from the [iOS Dev Center]. Choose iOS 8 tab and download the app's `*.dmg` file.

Also to use Document Picker's Export/Move feature you need to be able to export or move documents to an other app which is compatible with iCloud Drive feature. In order to use that, you need to update your mac to Yosemite. Then you will be able to export documents to Preview or TextEdit apps.

## Setup. Xamarin Studio
1. Open the solution
2. Inside the NBox project open the `Entitlements.plist` file and scroll down to `App Groups` section and change 
`group.com.xamarin.NewBox,security` to `group.com.yourcompanyname.NewBox,security` replacing `yourcompanyname` with your company name (to make id unique)
3. Open the `Info.plist` and also change the Application Name and Bundle Identifier to `com.yourcompanyname.NewBox.NBox`
4. In the NBoxFileProvider project open the `Entitlements.plist` file and scroll down to `App Groups` section and change 
`group.com.xamarin.NewBox,security` to `group.com.yourcompanyname.NewBox,security` replacing `yourcompanyname` with your company name (to make id unique)
5. Open the `Info.plist` and change the Bundle Identifier to `com.yourcompanyname.NewBox.NBoxFileProvider`
6. Click on the `Source` tab and scroll down to and expand the `NSExtension` key. Change the value for: `NSExtensionFileProviderDocumentGroup` from `group.com.xamarin.NewBox.security` to  `group.com.yourcompanyname.NewBox.security`
7. In the NewBox project open the `Entitlements.plist` file. In the `iCloud` section, change: `iCloud.com.xamarin.NewBox` to `iCloud.com.yourcompanyname.NewBox`
8. Click on the `Source` tab and change the value for the `com.apple.developer.icloud-container-identifiers` key from: `iCloud.com.xamarin.NewBox` to `iCloud.com.yourcompanyname.NewBox`
9. Open the `Info.plist` file and change the Bundle Identifier from: `com.xamarin.NewBox` to `com.yourcompanyname.NewBox`
10. Go to [Apple's Developer portal](https://developer.apple.com/account/ios/certificate/certificateList.action)
11. Make sure you have a valid Developer certificate and that it has been installed on your Mac.
12. In the `Identifiers` section select `iCloud Containers` and click the `+` button to add a new iCloud Container App ID. Name it whatever you like (but memorable) and make the ID: `iCloud.com.yourcompanyname.NewBox`
13. In the `Identifiers` section select `App Groups` and click the `+` button to add a new App Group ID. Name it whatever you like (but memorable) and make the ID: `group.com.yourcompanyname.NewBox.security`

You will need to do the following three times, once for each project in the NewBox solution. 

14. In the `Identifiers` section select App IDs and click the `+` button to add a new App ID. Set App ID Description to whatever you like (but memorable). Use an `Explicit App ID` for App ID Suffix and use the bundle identifier for the project. So you need three of these App IDs, one for each bundle identifier:
`com.yourcompanyname.NewBox`
`com.yourcompanyname.NewBox.NBoxFileProvider`
`com.yourcompanyname.NewBox.NBox`
15. In the `Enable Services` section For all three App Ids, enable `iCloud and App Groups`, and use the `Include CloudKit support` option for iCloud.
16. Click `Continue`. You may note that App Groups and/or iCloud show up as configurable. We will configure that in a bit.
17. Click `Submit`
18. Once you have the App Ids for all three projects, then you need to configure the `App Groups` and `iCloud` for the App Ids. 
19. Select each of the three app ids you just created and choose `Edit`.
20. Click the `Edit` button next to `App Groups` and select the App Group ID you created step 13 above. Click `Continue`.
21. Click the `Edit` button next to `iCloud` and select the iCloud Container you created step 12 above. Click `Continue`.
22. When done, click `Done`.
23. Now you need to create three provisioning profiles, one for each App ID created in steps above. Use type iOS App Development.
24. After selecting the App ID for the profile, click `Continue` and select the Developer certificate(s) to use with that profile. 
25. Click `Continue` and select the Devices that the profile will be valid on. (you may need to set up some devices in the Devices section if you have not done that already).
26. Click `Continue` and name the profile (something memorable) and click `Generate`.
27. Once generated, click the `Download` button to download it to your Mac. Double click the downloaded Provisioning profile to install it in XCode. 
28. Once you have all of those profiles created, make sure to go into the Project Options page for each project and set your `Signing Identity and Provisioning Profile` to the certificates ad profiles created above. (Project Options->Build->iOS Bundle Signing page)
29. Try running the NewBox sample!

## Open/Import documents from iCloud document provider with Document Picker
* Create some documents in iCloud container. For this step you need Pages, Numbers or Keynote.
* Run the NewBox app.
* Choose `Open` or `Import`.
* Choose a file and tap on it. `UIDocumentViewController` will be dismissed.
* Look at the device's console output to see the results (file content).

## Export/Move documents to iCloud document provider with Document Picker
* For this step you need mac with Yosemite.
* Run the NewBox app.
* Choose `Export` or `Move`.
* Select a destination (e.g. within `Preview` or `TextEdit` folder) and tap on `Export(Move) to this location`
* Go to `Finder` on your Mac and choose `iCloud Drive`. Choose a folder (e.g. `Preview` or `TextEdit`)
* You will find your file.

## Open/Import documents from NBox file provider extension
* Run the NewBox app.
* Choose `Open/Import`.
* Enable your extension. Tap on `Locations` then `More ...`. Enable extension. Tap `Done`.
* Choose `Open`/`Import` again. Tap on "Locations" and choose your extension.
* Tap on `Untitled.txt` button. At this point file provider will provide a file for you.
* Look at the console output.

## Export/Move document to NBox file provider extension
* Run the NewBox app.
* Choose Export/Move.
* Enable your extension. Tap on `Locations` then `More ...`. Enable extension. Tap `Done`.
* Choose `Export/Move` again. Tap on `Locations` choose your extension.
* Tap on `Export/Move to this location` button.
* Go to NBox user interface again and you will see the imported file.

## Note
Don't use `NSFileCoordinator` inside your app extension. Apple provide an [explanation](https://developer.apple.com/library/ios/technotes/tn2408/_index.html#//apple_ref/doc/uid/DTS40014939)

## Build Requirements

Building this sample requires Xcode 6.0 and iOS 8.0 SDK.

## Target
This sample is runnable on iPhone/iPad.

## Copyright

Xamarin port changes are released under the MIT license

## Author 

Ported to Xamarin.iOS by Rustam Zaitov.

[App Extension Programming Guide]:https://developer.apple.com/library/prerelease/mac/documentation/General/Conceptual/ExtensibilityPG/FileProvider.html

[Document Picker Programming Guide]:https://developer.apple.com/library/prerelease/ios/documentation/FileManagement/Conceptual/DocumentPickerProgrammingGuide/AccessingDocuments/AccessingDocuments.html

[iOS Dev Center]:https://developer.apple.com/devcenter/ios/index.action

[this introduction]:http://developer.xamarin.com/guides/ios/platform_features/introduction_to_the_document_picker/
