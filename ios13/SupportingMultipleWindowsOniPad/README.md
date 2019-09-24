---
name: Xamarin.iOS - Supporting Multiple Windows on iPad
description: "Use scheduled background tasks for refreshing your app content and for performing maintenance (iOS13)"
page_type: sample
languages:
- csharp
products:
- xamarin
extensions:
    tags:
    - ios13
urlFragment: ios13-supportingmultiplewindowsonipad
---
# Supporting Multiple Windows on iPad

This is a Xamarin port of Apple's [Supporting Multiple Windows on iPad][1] sample.

Use the UIScene lifecycle to support two side-by-side instances of your interface and learn how to create a new window with drag and drop.

## Instructions

To be able to test Background tasks, please, follow these steps:

1. Add the following keys to **Info.plist** to support multiple windows:

	```xml
	<key>NSUserActivityTypes</key>
	<array>
		<string>com.xamarin.Gallery.openDetail</string>
	</array>
	<key>UIApplicationSceneManifest</key>
	<dict>
		<key>UIApplicationSupportsMultipleScenes</key>
		<true/>
		<key>UISceneConfigurations</key>
		<dict>
			<key>UIWindowSceneSessionRoleApplication</key>
			<array>
				<dict>
					<key>UISceneConfigurationName</key>
					<string>Default Configuration</string>
					<key>UISceneDelegateClassName</key>
					<string>SceneDelegate</string>
					<key>UISceneStoryboardFile</key>
					<string>Main</string>
				</dict>
			</array>
		</dict>
	</dict>
	```

2. Enjoy the app.

## Build Requirements

Building this sample requires Xcode 11.0 and iOS 13.0 SDK

## Refs

- [Original sample page.][1]
- [protocol UIWindowSceneDelegate][2]
- [class UIWindowScene][3]
- [protocol UISceneDelegate][4]
- [class UIScene][5]
- This sample code project is associated with WWDC 2019 session 212: [Introducing Multiple Windows on iPad.][6]

## Target

This sample runnable on iPhoneSimulator/iPadSimulator iPhone/iPad

## License

Xamarin port changes are released under the MIT license.

The original sample is released under the following [license][7].

[1]: https://developer.apple.com/documentation/uikit/app_and_environment/scenes/supporting_multiple_windows_on_ipad
[2]: https://developer.apple.com/documentation/uikit/uiwindowscenedelegate
[3]: https://developer.apple.com/documentation/uikit/uiwindowscene
[4]: https://developer.apple.com/documentation/uikit/uiscenedelegate
[5]: https://developer.apple.com/documentation/uikit/uiscene
[6]: https://developer.apple.com/videos/play/wwdc19/212/
[7]: ./LICENSE/LICENSE.txt
