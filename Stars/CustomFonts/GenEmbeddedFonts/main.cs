using System;
using System.Linq;
using System.IO;

using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.CoreFoundation;

namespace GenEmbeddedFonts
{
	public class main
	{
		//When storing font data into our internal dictionaries, we use the argument index as a key to serialize the data
		//Data that we wish to serialize as code will have its argument index increased by kCodeArgumentIndexSpread.
		const int CODE_ARG_INDEX_SPREAD = 1000;

		static int Main (string[] args)
		{
			string procName = "";

			if (args.Length == 0) {
				PrintHelpAndExit ("", -1);
			}

			else if (args.Length < 2) {
				procName = args [0];
				PrintHelpAndExit (procName, -1);
			}

			var outputFilePrefix = "GeneratedFonts.cs";
			var outputPlistPrefix = "SupportingData";
			var outputPath = "./";
			bool havePlistEntries = false;

			var fontsDict = new NSMutableDictionary ();
			var fontNames = new NSMutableDictionary ();
			var plistMapping = new NSMutableDictionary ();

			for (int curArg = 0; curArg < args.Length; curArg++) {
				if (args [curArg] == "-code") {
					if (curArg + 1 == args.Length) {
						PrintHelpAndExit (procName, -1);
					}
					AbsorbFontFile (args[++curArg], fontsDict, fontNames, plistMapping, curArg + CODE_ARG_INDEX_SPREAD);
				}
				else if (args [curArg] == "-plist") {
					if (curArg + 1 == args.Length) {
						PrintHelpAndExit (procName, -1);
					}
					AbsorbFontFile (args[++curArg], fontsDict, fontNames, plistMapping, curArg);
					havePlistEntries = true;
				}
				else if (args [curArg] == "-outputDir") {
					if (curArg + 1 == args.Length) {
						PrintHelpAndExit (procName, -1);
					}
					outputPath = args[++curArg];
				} else {
					AbsorbFontFile (args[curArg], fontsDict, fontNames, plistMapping, curArg + CODE_ARG_INDEX_SPREAD);
				}
			}


			if (fontsDict.Count != 0) {

				var outputFile = outputPath + outputFilePrefix;

				var theCodeFileNameString = outputFilePrefix;
				var outputFileStr = outputFile;
				var codeFile = File.Open (outputFileStr, FileMode.Create);

				if (codeFile != null) {
				}

			}

			return 0;
		}

		static void PrintHelpAndExit(string procName, int exitValue) 
		{
			Console.WriteLine ("[-outputDir outputfileprefixpath] [-code | -plist] fontInputFile1...[-code | -plist] fontInputFileN\n", procName);
			Console.WriteLine ("This tool will take one or more font files and re-package them either as code or in a dictionary plist.\n");
			Console.WriteLine (" -code\t\tGenerates data for the font file specified to be compiled with code. " +
			                   "This is the default if -code or -plist is not specified.\n");
			Console.WriteLine (" -plist\t\tGenerates plist data for the font file specified.\n");
			Console.WriteLine (" -outputDir\tPath where files will be generated.\n");
			Environment.Exit(exitValue);
		}

		static void PrintHeader (string filePath, string codeOrHeader, int argc, string[] argv) 
		{
			Console.WriteLine (filePath, "// This was auto-generated using: \n//\t\t", codeOrHeader);
			Console.WriteLine (filePath + " " + argv[0]);

			for (int i = 1; i < argc; i++) {
				var url = NSUrl.FromFilename (argv[i]);
				if (url != null && url.IsFileUrl) {
					Console.WriteLine (filePath + " " + url.Path);
				} else {
					Console.WriteLine (filePath, " " + argv[i]);
				}
				Console.WriteLine (filePath, "\n//\n// Please see the GenEmbeddedFont target and GenerateFontData.sh" +
				                   "for details on how this %s was generated.", codeOrHeader);
				Console.WriteLine (filePath, "\n// This file will be used by the FontLoader class in the CustomFonts target.");
				Console.WriteLine (filePath, "\n// The FontLoader class will make the font data pointed by this file available in your application.");
			}
		}

		static void AbsorbFontFile (string fontFilePath, NSMutableDictionary fontsDict, NSMutableDictionary fontNames, NSMutableDictionary plistMapping, int curArg)
		{
			bool argIsValid = false;

			NSData data = NSData.FromFile (fontFilePath);

//			var dataProvider = new CGDataProvider(fontFilePath);
//
//			if (dataProvider != null && data != null) {
//				var font = CGFont.CreateFromProvider (dataProvider);
//
//				if (font != null) {
//					var postName = font.PostScriptName;
//
//					if (postName != null) {
//						fontNames.Add (data, new NSString (postName));
//						fontsDict.Add (data, new NSNumber (curArg));
//						plistMapping.Add (new NSString (curArg.ToString()), new NSString (postName));
//						argIsValid = true;
//					}
//				}
//			}
			if (argIsValid == false){
				Console.WriteLine (fontFilePath + " was ignored - not a valid font file.");
			}
		}
	}
}