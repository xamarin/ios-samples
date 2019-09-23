---
name: Xamarin.iOS - Sign In with Apple Flow
description: "Provide a fast, secure, and privacy-friendly way for users to set up an account and start using your services (iOS13)"
page_type: sample
languages:
- csharp
products:
- xamarin
extensions:
    tags:
    - ios13
urlFragment: ios13-addingthesigninwithappleflowtoyourapp
---
# Adding the Sign In with Apple Flow to Your App

This is a Xamarin port of Apple's [Adding the Sign In with Apple Flow to Your App][1] sample.

Provide a fast, secure, and privacy-friendly way for users to set up an account and start using your services.

## Instructions

Perform the following steps before building and running the app. On [Apple Developer Certificates, Identifiers & Profiles][5] portal:

1. Create a new **App Ids** Identifier
2. Set a description in the **Description** field
3. Choose an **Explicit** Bundle ID and set `com.xamarin.AddingTheSignInWithAppleFlowToYourApp` in the field.
4. Enable **Sign In with Apple** capability and register the new Identity
5. Create a new Provisioning Profile with the new Identity
6. Download and install it on your device
7. In Visual Studio, enable the **Sign In with Apple** capability in **Entitlements.plist** file

## Build Requirements

Building this sample requires Xcode 11.0 and iOS 13.0 SDK

## References

* [Original sample page.][1]
* [Sign In with Apple Entitlement.][2]
* This sample code project is associated with WWDC 2019 session 706: [Introducing Sign In with Apple.][3]

## Target

This sample runnable on iPhoneSimulator/iPadSimulator iPhone/iPad

## License

Xamarin port changes are released under the MIT license.

The original sample is released under the following [license][4].

## Author

Ported to Xamarin.iOS by Israel Soto.

[1]: https://developer.apple.com/documentation/authenticationservices/adding_the_sign_in_with_apple_flow_to_your_app
[2]: https://developer.apple.com/documentation/bundleresources/entitlements/com_apple_developer_applesignin
[3]: https://developer.apple.com/videos/play/wwdc19/706/
[4]: ./LICENSE/LICENSE.txt
[5]: https://developer.apple.com/account/resources/identifiers/list
