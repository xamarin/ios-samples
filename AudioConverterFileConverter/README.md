---
name: Xamarin.iOS - Audio Converter File Converter
description: "Demonstrates the use of the Audio Converter API to convert from a PCM audio format in an AIF file to a compressed format in a CAF file..."
page_type: sample
languages:
- csharp
products:
- xamarin
urlFragment: audioconverterfileconverter
---
# Audio Converter File Converter Demo

This sample demonstrates the use of the Audio Converter API to convert
from a PCM audio format in an AIF file to a compressed format in a CAF
file and will support AAC encode on appropriate hardware such as the
iPhone 3GS.

Touching the "Convert" button calls the method DoConvertFile() producing
an output.caf file using the encoding and sample rate chosen in the UI.

The output.caf file is then played back after conversion using AVAudioPlayer
to confirm success and Audio format information for both the source and
output file is displayed.

Interruption handling during audio processing (conversion) is also demonstrated.

Audio converter objects convert between various linear PCM audio formats.
They can also convert between linear PCM and compressed formats.

Supported transformations include the following:

* PCM bit depth<
* PCM sample rate
* PCM fixed point to and from PCM integer
* PCM interleaved to and from PCM deinterleaved
* PCM to and from compressed formats<

# Source

This is port of Apple's iPhoneACFileConvertTest sample

https://developer.apple.com/library/ios/#samplecode/iPhoneACFileConvertTest/Introduction/Intro.html
