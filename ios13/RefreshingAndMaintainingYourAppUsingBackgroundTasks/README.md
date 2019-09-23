---
name: Xamarin.iOS - Refreshing And Maintaining Your App Using Background Tasks
description: "Use scheduled background tasks for refreshing your app content and for performing maintenance (iOS13)"
page_type: sample
languages:
- csharp
products:
- xamarin
extensions:
    tags:
    - ios13
urlFragment: ios13-refreshingandmaintainingyourappusingbackgroundtasks
---
# Refreshing And Maintaining Your App Using Background Tasks

This is a Xamarin port of Apple's [Refreshing And Maintaining Your App Using Background Tasks][1] sample. 

Use scheduled background tasks for refreshing your app content and for performing maintenance.

## Instructions

To be able to test Background tasks, please, follow these steps:

1. Add the following key to **Info.plist** to enable Background modes:
	
	```xml
	<key>UIBackgroundModes</key>
	<array>
		<string>fetch</string>
		<string>processing</string>
	</array>
	```

2. Add the following key and uniques task ids to **Info.plist**:
	
	```xml
	<key>BGTaskSchedulerPermittedIdentifiers</key>
	<array>
		<string>com.xamarin.ColorFeed.refresh</string>
		<string>com.xamarin.ColorFeed.cleaning_db</string>
	</array>
	```

	You need to add a unique task id into **Info.plist** for each background task you want to execute.

3. To be able to test a background task, you need to the debug the app with Xcode by:
	
	* Following [these steps][6]
	* or by running the app on Visual Studio with Debug configuration and attaching the process to Xcode by clicking on **Debug** menu > **Attach to process** > **ColorFeed** (or the app name.)

4. Once attached to Xcode, send the app to background and open it again; this, to register the background tasks in the OS.
5. On Xcode, enable the `lldb` in the debugger by pausing the debug session.
6. In the debugger, execute the line shown below, substituting the identifier of the desired task for `TASK_IDENTIFIER`:

	```sh
	e -l objc -- (void)[[BGTaskScheduler sharedScheduler] _simulateLaunchForTaskWithIdentifier:@"TASK_IDENTIFIER"]
	```

7. Resume your app. The system launches the app in the background to run the desired task.
8. To force termination of a task, execute the line shown below, substituting the identifier of the desired task for `TASK_IDENTIFIER`.

	```sh
	e -l objc -- (void)[[BGTaskScheduler sharedScheduler] _simulateExpirationForTaskWithIdentifier:@"TASK_IDENTIFIER"]

	```

9. Resume your app. The system calls the expiration handler for the desired task.

For more info, visit the following [link][3].

## Build Requirements

Building this sample requires Xcode 11.0 and iOS 13.0 SDK

## Refs

* [Original sample page.][1]
* [class `BGTaskScheduler`][2]
* [Starting and Terminating Tasks During Development][3]
* This sample code project is associated with WWDC 2019 session 707: [Advances in App Background Execution.][4]

## Target

This sample runnable on iPhone/iPad

## Copyright

Xamarin port changes are released under the MIT license.

The original sample is released under the following [license][5].

## Author

Ported to Xamarin.iOS by Israel Soto.

[1]: https://developer.apple.com/documentation/backgroundtasks/refreshing_and_maintaining_your_app_using_background_tasks
[2]: https://developer.apple.com/documentation/backgroundtasks/bgtaskscheduler
[3]: https://developer.apple.com/documentation/backgroundtasks/starting_and_terminating_tasks_during_development
[4]: https://developer.apple.com/videos/play/wwdc19/707/
[5]: ./LICENSE/LICENSE.txt
[6]: https://docs.microsoft.com/en-us/xamarin/ios/troubleshooting/questions/debugging-with-xcode
