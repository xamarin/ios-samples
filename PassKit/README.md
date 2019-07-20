---
name: Xamarin.iOS - PassKit Sample
description: 'This sample demonstrates how to build a PassKit Companion App to interact with passes you have issued in a users Passbook. >NOTE: This sample...'
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: passkit
---
# PassKit Sample

This sample demonstrates how to build a PassKit Companion App to interact with passes you have issued in a users Passbook.

>NOTE: This sample doen't work out of the box, you need to setup provisioning profile for you app and create certicicate for pass singing.


## Instructions

When you open this app, _it will not run_ because the solution requires the file **CouponBanana2.pkpass** which does not exist. Follow the steps below to create the sample Pass file, _then_ you'll be able to run the sample.

### Create a Pass

To use passes your app needs to be provisioned correctly, and you need to make some changes to the example pass to match your personal provisioning information:

* Create an AppId in the provisioning portal and enable `Wallet`
* Generate `Provisioning Profile` for your app
* Register new `iOS Pass Type ID`
* Generate Certificate for your just created `iOS Pass Type ID` via `Edit > Create Certificate`
* Download and install your certificate for pass

**IMPORTANT:** Now you need to fix some files:  
* `Entitlements.plist` and `Info.Plist` – fix `BundleIdentifier` and `PassTypeIdentifiers`
* `CouponBanana2.pass/pass.json` – fix `passTypeIdentifier` and `teamIdentifier`

At this point you are ready to generate pass package. For this sample we provide a simple utility which create package for you (the **signpass** executable is included in this repo). Go to terminal and run:

```
cd path/to/PassLibrary/sample
./signpass -p CouponBanana2.pass/ -c "Common_Name"
```

You can determine the correct value for `Common_Name` with **Keychain Access** app:

* Launch **Keychain Access** app
* Find the certificate which you installed a few steps ago
* **Select certificate > Right Click > Get Info**
* Here you are able to see `Common Name` - it could be something like `"Pass Type ID: pass.com.yourcompany.passkitnameyouchose"`

After running that command in the **Terminal**, you should be able to find **CouponBanana2.pkpass** in the same folder as the command was run. 

#### Pass Troubleshooting

If you don't update the values in **pass.json**, you'll get an error 
`Could not initialize an instance of the type 'PassKit.PKPass': the native 'initWithData:error:' method returned nil*`

### Test the Pass

You can easily test a compiled Pass by dragging the file into a running iOS Simulator window. It should appear in the simulator and allow you to add it directly to the **Wallet**. In the Wallet you can then view and delete the pass.

If the pass does not appear, or cannot be added to the Wallet, verify all the steps above were completed correctly (especially the certificate generation, downloading, and the personalized values in **pass.json**).

Do not attempt to run the sample app until the Pass is generated and used successfully.

### Run the sample app

Go to Xamarin Studio - the solution should now include the **CouponBanana2.pkpass** file. Compile and run the sample app.

#### App Troubleshooting

If the Pass cannot be added in the app, verify the **CouponBanana2.pkpass** file exists (you generate it via the command line in the steps above). It should also have **Build Action: Bundle Resource** so that it is deployed with the app.

Also verify:
- App ID has **Wallet** enabled
- Pass Type ID was created correctly
- App Bundle ID and Pass Type ID are correctly entered.
- Provisioning Profile has been downloaded and installed.
- The correct provisioning profile is being used when the app is built.

# Further Reading

Refer to the [Introduction to PassKit](http://docs.xamarin.com/ios/tutorials/Introduction_to_PassKit) documentation on the Xamarin website.

![screenshot](https://github.com/xamarin/monotouch-samples/raw/master/PassKit/Screenshots/01-PassLibrary.png "PassLibrary")

Also refer to Apple's [Passbook for Developers](https://developer.apple.com/passbook/) site.
