HomeKitIntro
============

This sample application shows how to use HomeKit to write home automation applications in Xamarin.iOS.

## Enabling HomeKit in a Xamarin Application

Before a Xamarin application can utilize the HomeKit Framework, the application must be correctly provisioned, in both the Apple Developer Portal and in Xamarin Studio.

Do the following to enable HomeKit support:

1. In a web browser, navigate to [http://developer.apple.com](http://developer.apple.com) and log into your account.
2. Click on **Certificates**, **Identifiers** and **Profiles**.
3. Select **Provisioning Profiles** and select **App IDs**, then click the **+** button.
4. Enter a **Name** for the new Profile.
5. Enter a **Bundle ID** following Apple’s naming recommendation.
6. Scroll down to the **App Services** section, select **HomeKit** and click the **Continue** button.
7. Verify all of the settings, then **Submit** the App ID.
8. Select **Provisioning Profiles** > **Development**, click the **+** button, select the **Apple ID**, then click **Continue**.
9. Click Select **All**, then click **Continue**.
10. Click **Select All** again, then click **Continue**.
11. Enter a **Profile Name** using Apple’s naming suggestions, then click **Continue**.
12. Start Xcode 6.
13. From the Xcode menu select **Preferences…**
14. Select **Accounts**, then click the **View Details…** button.
15. Click the **Refresh** Button in the lower left hand corner.
16. Ensure that the **Provisioning Profile** created above has been installed in Xcode.
17. Open the project to add HomeKit support to in Xamarin Studio.
18. In the **Solution Explorer**, select the **Project**.
19. Right-click the project and select **Options**.
20. In the **Options Dialog Box** select **iOS Application**, ensure that the **Bundle Identifier** matches the one that was defined in **App ID** created above in iTunes Connect for the application and that the Team matches your developer team.
21. Select **iOS Bundle Signing**, select the developer Identity and **Provisioning Profile** created above.
22. Click the **OK** button to save the changes and close the dialog box.
23. In the **Solution Explorer**, double-click the `Entitlements.plist` to open it for editing.
24. Scroll to the bottom of the list and place a check by the **Enable HomeKit** checkbox.
25. Save the changes to the entitlements.

##The HomeKit Accessory Simulator

To make testing of a HomeKit enable mobile applications possible, Apple has created a HomeKit Accessory Simulator that allows the developer to create and configure different types of simulated home automation accessories. Using the simulator, the developer can create a wide range of virtual hardware with different configurations of options and features.  Apple is providing the HomeKit Accessory Simulator as a separate download from Xcode 6.

To install the simulator, do the following:

1. In a web browser, navigate to [https://developer.apple.com/downloads/index.action](https://developer.apple.com/downloads/index.action)
2. From the list of available downloads select **Hardware IO Tools for Xcode**.
3. Download the tools.
4. Open the Downloads folder and unzip the tool.
5. Copy the contents of the zip file to the Applications folder.

###Creating Virtual Accessories

To start the HomeKit Accessory Simulator and create a few virtual accessories, do the following;

1. From the **Applications** folder, start the **HomeKit Accessory Simulator**.
2. Click the **+** button in the lower left hand corner of the window and select **New Accessory…**
3. Enter a **Name** and optionally a **Manufacturer** name and click the **Finish** button to create the new accessory.
4. Next, add a service for this accessory to provide by clicking the **Add Service** button and selecting a service type such as **Add Light Bulb**.
5. The light service will be added to the accessory and is ready to be configured. For example, click the **Service Title** and change it to **LED Bulb**.
6. Repeat the steps above to create a **Garage Door Opener**, **Thermostat**, **Lock** and **Switch** accessory for testing.

With some sample virtual HomeKit accessories created and configured, a Xamarin iOS 8 mobile application can be created to consume and control these accessories.

##Testing a HomeKit App

When the application is first run, the user will be asked if they want to allow it to access their HomeKit information. If the user answers **OK**, then the application will be able to work with their HomeKit Accessories otherwise it will not and any calls to HomeKit will fail with an error. 

A Primary Home must be created and configured before any other function of the HomeKit framework is available to an iOS 8 mobile application. It is also the responsibility of the application to provide a way for the user to create and assign a Primary Home if one does not already exist. After the application first starts or when it has returned to focus after being in the background, it should monitor the `DidUpdateHomes` event of the `HMHomeManager` and check for the existence of the Primary Home. If one does not exist, it should provide an interface for the user to create one.

Once a Primary Home has been created or accessed, the iOS 8 application can call the `MHAccessoryBrowser` to find any new home automation accessories and add them to a home. Once the new accessory has been found, it should be presented to the user and they should be allowed to select an accessory and add it to a home. When the user selects an accessory from the list, the application calls the Home’s `AddAccessory` method to add it to the Home’s collection of accessories. The user will be asked to enter the setup code for the device to add, In the HomeKit Accessory Simulator this number can be found under the **Accessories Name**. For a real HomeKit accessory, the setup code will either be presented in the accessory’s user manual, on the product box or on a label on the device itself.

When working with HomeKit Service Characteristics and simulated accessories, modifications to Characteristics values can be monitored inside the HomeKit Accessory Simulator. With the `HomeKitIntro` app running on real iOS Device Hardware, changes to a characteristic’s value should be seen nearly instantly in the HomeKit Accessory Simulator. 

---
**NOTE:** Testing HomeKit only works on a real iOS 8 Hardware Device and not in the iOS 8 Simulator.

---



