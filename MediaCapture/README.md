---------------------------------------
Media Capture
---------------------------------------

This sample contains a complete solution for capturing still images, video and audio.  The design uses the iOS AVFoundation and AVCaptureSession to build a streaming media capture engine that also contains a frame grabber.  Individual capture types (images, audio, video) can be enabled and configured.  The programming model supports subscription to continuous runtime events that fire while the capture is in progress.

The major features of this sample are:
-------------------------------------

- Completely reusable CaptureManager class.  The files in the 'CaptureManager' directory can be dropped into any app that requires media capture and save capability.

- Sample app to expose settings and demonstrate the media capture.

- Complete encapsulation of the iOS classes.  The capture manager public API is .NET through and through and shields the client code from dealing with AVFoundation. 

- The settings UI stands apart from the capture classes but can easily be integrated into other projects

- The still images can be captured and saved to either the camera roll or a directory in 'MyDocuments'


The CaptureManager handles all of the low-level AVFoundation and device setup required to capture and save media.  This code also exposes the recording, configuration and notification capabilities in a manner consistent with standard .NET coding practices.  The client code will not need to deal with threads, delegate classes, buffer management, or objective-C data types but instead will create .NET objects and handle standard .NET events.  

The underlying iOS concepts and classes are detailed in the iOS developer online help (TP40010188-CH5-SW2).

https://developer.apple.com/library/mac/#documentation/AudioVideo/Conceptual/AVFoundationPG/Articles/04_MediaCapture.html#//apple_ref/doc/uid/TP40010188-CH5-SW2


Enhancements, suggestions and bug reports can be sent to steve.millar@infinitekdev.com


---------------------------------------
Using the Test App
---------------------------------------

1. Press 'start' and watch the preview window show the still images being captured.  This looks like a video but is not being saved.

2. Press 'stop' and then press 'settings' to change what is being captured.  You can also change the camera, save directories and resolution.  try enabling audio and video recording and also enable the saving of still images to 'MyDocuments.'

3. Press 'start' again.  Note that there is currently no preview for video recordings but they are being recorded if you enabled recording in settings.

4. Press 'stop' and then press 'browse'.  This will bring up a media browser dialog that lets you see your captured images and recorded movies.  

Note that you may also choose the camera roll for the still image save location.   


---------------------------------------
Limitations
---------------------------------------

- There is currently no video preview window while recording is in progress, however, there is a media browser and playback capability.

- Still image captures and video captures do not seem to work at the same time.  The configuration will not cause an error at runtime but only the movie recording actually happens.


---------------------------------------
Requires
---------------------------------------

- iOS device (the emulator does not a camera or microphone device)

- Purchased copy of MonoTouch

- The sample is a universal app but the UI portion is more suited to an iPad and that is where most of the testing happened

---------------------------------------
Author
---------------------------------------

Steve Millar
steve.millar@infinitekdev.com
