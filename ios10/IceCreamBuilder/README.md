---
name: 'Xamarin.iOS - Ice Cream Builder: A simple Messages app extension'
description: This sample is a simple example of building an app extension that interacts with the Messages app and lets users send interactive messages and...
page_type: sample
languages:
- csharp
products:
- xamarin
technologies:
- xamarin-ios
urlFragment: ios10-icecreambuilder
---
# Ice Cream Builder: A simple Messages app extension

This sample is a simple example of building an app extension that interacts with the Messages app and lets users send interactive messages and create stickers.

The extension is based on a MSMessagesAppViewController subclass that then presents a child view controller depending on the current conversation, message state and presentation style. It also updates a conversation with new or updated messages and posts stickers to a conversation.

## Instructions

To test this sample you'll need to deploy it on device/simulator. Main app will show you only white screen. However the most interesting part of the app is app extension. Open Messages app and choose Ice Cream Builder in extensions.

## Build Requirements

Xcode 8.0, iOS 10.0 SDK

## Useful links

[Swift version of sample](https://developer.apple.com/library/prerelease/content/samplecode/IceCreamBuilder/Introduction/Intro.html)

## Copyright

Xamarin port changes are released under the MIT license

## Author 

Ported to Xamarin.iOS by Oleg Demchenko
