---
name: 'Xamarin.iOS - Fit: Store and Retrieve HealthKit Data'
description: "Fit is a sample intended as a quick introduction to HealthKit. It teaches you everything from writing data into HealthKit to reading data (iOS8)"
page_type: sample
languages:
- csharp
products:
- xamarin
extensions:
    tags:
    - ios8
urlFragment: ios8-fit
---
# Fit: Store and Retrieve HealthKit Data

Fit is a sample intended as a quick introduction to HealthKit. It
teaches you everything from writing data into HealthKit to reading
data from HealthKit. This information may have been entered into the
store by some other app; e.g. a user's birthday may have been entered
into Health, and a user's weight by some popular weight tracker app.

Fit shows examples of using queries to retrieve information from
HealthKit using sample queries and statistics queries. Fit gives you a
quick introduction into using the new Foundation classes
NSLengthFormatter, NSMassFormatter, and NSEnergyFormatter.

## Requirements

This sample requires capabilities that are only available when run on
an iOS device.

To run the sample on a device, please create a valid AppID with
HealthKit enabled, and generate the corresponding Provisioning Profile
from the Dev Portal. Download and link Fit with this Provisioning
Profile. Don't forget to change the Bundle Identifier and
entitlements.plist to match the new AppId

## Build requirements

Building this sample requires Xcode 6.0 and iOS 8.0 SDK
Running the sample requires iPhone with iOS 8.0 or later.

## License

Xamarin port changes are released under the MIT license
