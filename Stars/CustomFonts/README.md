CustomFont
==========

This is a port of the WWDC2012 sample.

This sample demonstrates the following different techniques that can be used
 to make fonts available to your applications:
 
 	1. UIAppFonts in the applications Info.plist
 	2. Fonts embedded in code
 	3. Font data embedded in a plist
 	4. Font files present in your bundle.
 	
 Embedding fonts in code or a plist allows the developer to hide fonts from users that
 look inside their application's bundle, which could be useful for licensing reasons.
 This sample does not attempt to encrypt or produce a form or DRM for the font.
 
 The FontLoader class keeps track of fonts that are embedded directly into source code,
 a plist, or URLs. The developer can instantiate fonts that are readily usable by all font APIs.
 FontLoader also demonstrates how a developer can instantiate fonts in a private manner.
 
 Fontloader acts on font data that is formatted in a specific way. Included in the sample app
 is a tool that generates this data from font files directly. The GenEmbeddedFont sub project 
 contains the code and is run everytime you build this sample. The GenerateFontData.sh script 
 invokes the GenEmbeddedFont tool.
 
 This sample contains 9 different custom fonts. When the application launches, you will see a list
 of all the embedded fonts and some hand-picked fonts that ship with the OS. The custom fonts are
 limited by the fact that they only have either lowercased vowels, uppercased vowels, or digits.
 When displaying one of the custom fonts that does not have a needed glyph, the custom fallback font
 is displayed. This custom fallback font is loaded privately so it is not available to any of the
 font APIs. It is only available in CoreText fallbacks. CTLabel class demonstrates the custom font 
 fallback functionality.
 
 
Ported by: Peter Collins