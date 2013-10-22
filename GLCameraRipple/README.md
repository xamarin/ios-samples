GLCameraRipple
==============

This is a port of Apple's GLCameraRipple sample to C# and MonoTouch.

http://developer.apple.com/library/ios/#samplecode/GLCameraRipple/Introduction/Intro.html

This sample shows how to use GLKit [1] to render OpenGL frames,
AVFoundation to fetch live video from the camera and OpenTK's API to
load a couple of shaders that simulate a water ripple effect when the
user touches the display.

This uses the CVOpenGLESTextureCache [2] class which was introduced
with iOS 5.0 which binds CoreVideo buffers to OpenGL textures.

[1] http://iosapi.xamarin.com/?link=N%3aMonoTouch.GLKit

[2] http://iosapi.xamarin.com/?link=T%3aMonoTouch.CoreVideo.CVOpenGLESTextureCache

Ported by: Miguel de Icaza (miguel@xamarin.com)