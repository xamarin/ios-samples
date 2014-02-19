This is a port of Apple's UIImageEffect sample
----------------------------------------------

From Apple:

	UIImageEffects shows how to create and apply blur and tint
	effects to an image using the vImage, Quartz, and UIKit
	frameworks. The vImage framework is suited for
	high-performance image processing. Using vImage, y our app
	gets all the benefits of vector processing without the need
	for you to write vectorized code.
	
Main classes:

	AppDelegate.cs: 

		Sets up the application, loads the image and calls the
		various filters on each tap.

	UIImageEffect.cs:

		Contains various extension methods that allow the user
		to blur images and tint those images.  The convenience
		methods show how to use the API.