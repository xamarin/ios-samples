---
name: Xamarin.iOS - Eliza Chat
description: 'This sample application shows how to use SiriKit to enable Siri Voice Control in a Xamarin.iOS running iOS 10 (or greater). NOTE: The Eliza Chat...'
page_type: sample
languages:
- csharp
products:
- xamarin
technologies:
- xamarin-ios
urlFragment: ios10-elizachat
---
# Eliza Chat

## This sample application shows how to use SiriKit to enable Siri Voice Control in a Xamarin.iOS running iOS 10 (or greater). 
## **NOTE**: The Eliza Chat sample app must be singed with a valid developer account and provisioned with the correct App ID, Entitlements and Provisioning Profile before it can be compiled and tested on an iOS 10 device. Testing SiriKit only works on a real iOS 10 Hardware Device and not in the iOS 10 Simulator. See the following instructions for details.

## Enabling SiriKit Support in a Xamarin Application

Before a Xamarin application can utilize the SiriKit Framework, the application must be correctly provisioned, in both the Apple Developer Portal and in Xamarin Studio.

Do the following to enable SiriKit support:

1. In a web browser, navigate to [http://developer.apple.com](http://developer.apple.com) and log into your account.
2. Click on **Certificates**, **Identifiers** and **Profiles**.
3. Select **Provisioning Profiles** and select **App IDs**, then click the **+** button.
4. Enter a **Name** for the new Profile.
5. Enter a **Bundle ID** following Apple’s naming recommendation.
6. Scroll down to the **App Services** section, select **SiriKit** and click the **Continue** button.
7. Verify all of the settings, then **Submit** the App ID.
8. Select **Provisioning Profiles** > **Development**, click the **+** button, select the **Apple ID**, then click **Continue**.
9. Click Select **All**, then click **Continue**.
10. Click **Select All** again, then click **Continue**.
11. Enter a **Profile Name** using Apple’s naming suggestions, then click **Continue**.
12. Start Xcode.
13. From the Xcode menu select **Preferences…**
14. Select **Accounts**, then click the **View Details…** button.
15. Click the **Refresh** Button in the lower left hand corner.
16. Ensure that the **Provisioning Profile** created above has been installed in Xcode.
17. Open the project to add SiriKit support to in Xamarin Studio.
18. In the **Solution Explorer**, select the **Project**.
19. Right-click the project and select **Options**.
20. In the **Options Dialog Box** select **iOS Application**, ensure that the **Bundle Identifier** matches the one that was defined in **App ID** created above in iTunes Connect for the application and that the Team matches your developer team.
21. Select **iOS Bundle Signing**, select the developer Identity and **Provisioning Profile** created above.

##Testing a SiriKit App

When the application is first run, the user will be asked if they want to allow it to access their Siri information. If the user answers **OK**, then the application will be able to work with Siri on their iOS 10 device, otherwise it will not and any calls to SiriKit will fail with an error. 

Please see our SiriKit documentation for more information:

* [Understanding SiriKit Concepts](https://developer.xamarin.com/guides/ios/platform_features/introduction-to-ios10/sirikit/understanding-sirikit/) - This article covers the key concepts that will be required for working with SiriKit in a Xamarin.iOS app. It covers the new Intents and Intents UI Extension Points and how they work with App and User Vocabulary to open an app to Siri.
## * [Implementing SiriKit](https://developer.xamarin.com/guides/ios/platform_features/introduction-to-ios10/sirikit/implementing-sirikit/) - This article covers the steps required to implement SiriKit support in a Xamarin.iOS apps. The developer should read the Understanding SiriKit Concepts guide above before attempting to add SiriKit support to an app, as key concepts are covered that will be required for successful implementation.
## **NOTE:** Testing SiriKit only works on a real iOS 10 Hardware Device and not in the iOS 10 Simulator.



