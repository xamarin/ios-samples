AQTapDemo
=========

This sample shows how to use the iOS 6 AudioQueue Processing Taps.
With Audio Processing Taps, you can either apply effects to your audio
or analize data as it is being decoded.  

This sample shows how to change the pitch of the audio being played
back.  As a bonus, it also contains code showing how to playback
streaming audio from the network.

This sample is a port of Chris Adamson's AQTapDemo from CocoaConf
which he has been kind enough to license under the MIT X11 license.
Marek Safar implemented the Xamarin.iOS/Xamarin.Mac support for
AudioQueueProcessingTap as well as the AudioUnit stack and ported
the original Objective-C code to C#.

**NOTE: This sample will only work with Xcode 5.0 or above.

Troubleshooting
--------------
If the app doesn't play any audio make sure that you are able to listen the music from online radio via your browser: http://1661.live.streamtheworld.com:80/CBC_R3_WEB_SC

Resources
=========

For more information, see Chris Adamson's slides from CocoaConf:

http://www.slideshare.net/invalidname/core-audio-in-ios-6-cocoaconf-raleigh-dec-12

His blog talks more about audio queue processing taps:

http://www.subfurther.com/blog/2012/10/30/cocoaconf-portland-12-and-the-audioqueueprocessingtap/

And you can browse our API documentation for the AudioQueueProcessingTap:

http://iosapi.xamarin.com/?link=T%3aMonoTouch.AudioToolbox.AudioQueueProcessingTap

