using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using MonoTouch.Dialog;
using System.Drawing;
using CoreText;

namespace ios7fonts
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		const string lorem = 
			"\nLorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. ";

		DialogViewController dialog;
		UIFontDescriptor baseline;
		UIWindow window;

		public Element MakeSample (string text, UIFontDescriptorSymbolicTraits trait)
		{
			var font = UIFont.FromDescriptor (baseline.CreateWithTraits (trait), 0);

			return new StyledStringElement (text) { Font = font };
		}

		public Element MakeMultilineSample (string text, UIFontDescriptorSymbolicTraits trait)
		{
			var font = UIFont.FromDescriptor (baseline.CreateWithTraits (trait), 0);

			return new StyledMultilineElement (text) { Font = font };
		}

		public UIFont ResizeProportionalAndAlternative (UIFont font)
		{
			var attributes = new UIFontAttributes (
				                 new UIFontFeature (CTFontFeatureNumberSpacing.Selector.ProportionalNumbers),
				                 new UIFontFeature ((CTFontFeatureCharacterAlternatives.Selector)1));

			var newDesc = font.FontDescriptor.CreateWithAttributes (attributes);
			return UIFont.FromDescriptor (newDesc, 40);	
		}

		public UIFont ResizeProportional (UIFont font)
		{
			var attributes = new UIFontAttributes (new UIFontFeature (CTFontFeatureNumberSpacing.Selector.ProportionalNumbers));
			var newDesc = font.FontDescriptor.CreateWithAttributes (attributes);
			return UIFont.FromDescriptor (newDesc, 40);	
		}

		public UIFont ResizeAlternative (UIFont font)
		{
			var attributes = new UIFontAttributes (new UIFontFeature ((CTFontFeatureCharacterAlternatives.Selector)1));
			var newDesc = font.FontDescriptor.CreateWithAttributes (attributes);
			return UIFont.FromDescriptor (newDesc, 40);	
		}

		public UIFont Resize (UIFont font)
		{
			return UIFont.FromDescriptor (font.FontDescriptor, 40);	
		}


		RootElement MakeFonts ()
		{
			Console.WriteLine ("Changing");
			// The baseline to show various adjustments you can make to a font
			baseline = UIFontDescriptor.PreferredBody;

			return new RootElement ("Fonts") {
				new Section () {
					new RootElement ("System Preferred Fonts"){
						new Section () {
							new StyledStringElement ("Headline") { Font = UIFont.PreferredHeadline },
							new StyledStringElement ("Subheadline") { Font = UIFont.PreferredSubheadline },
							new StyledStringElement ("Body") { Font = UIFont.PreferredBody },
							new StyledStringElement ("Caption1") { Font = UIFont.PreferredCaption1 },
							new StyledStringElement ("Caption2") { Font = UIFont.PreferredCaption2 },
							new StyledStringElement ("Footnote") { Font = UIFont.PreferredFootnote },

						}
					},
					new RootElement ("Font weight"){
						new Section (){
							MakeSample ("Body Plain", 0),
							MakeSample ("Body Bold", UIFontDescriptorSymbolicTraits.Bold),
							MakeSample ("Body Italic", UIFontDescriptorSymbolicTraits.Italic),
						}
					},
					new RootElement ("Line Spacing") {
						new Section (){
							MakeMultilineSample ("Line Spacing; regular; " + lorem, 0),
							MakeMultilineSample ("Line Spacing: tight; " + lorem, UIFontDescriptorSymbolicTraits.TightLeading),
							MakeMultilineSample ("Line Spacing: loose; " + lorem,  UIFontDescriptorSymbolicTraits.LooseLeading),
						}
					},
					new RootElement ("CoreText Font Features"){
						new Section ("Use  Proportional Number") {
							new StyledStringElement ("10:20") { Font = Resize (UIFont.PreferredBody) },
							new StyledStringElement ("10:20") { Font = ResizeProportional (UIFont.PreferredBody) },
						},
						new Section ("Use Character Alternatives") {
							new StyledStringElement ("10:20") { Font = Resize (UIFont.PreferredBody) },
							new StyledStringElement ("10:20") { Font = ResizeAlternative (UIFont.PreferredBody) },
						},
						new Section ("Use Proportional + Alternative") {
							new StyledStringElement ("10:20") { Font = Resize (UIFont.PreferredBody) },
							new StyledStringElement ("10:20") { Font = ResizeProportionalAndAlternative (UIFont.PreferredBody) },
						},
					}
				},
			};
		}

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			var fonts = MakeFonts ();
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			dialog = new DialogViewController (fonts);
			var nav = new UINavigationController (dialog);
			window.RootViewController = nav;
			window.MakeKeyAndVisible ();

			UIApplication.Notifications.ObserveContentSizeCategoryChanged (delegate { 
				dialog.Root = MakeFonts (); 
				nav.PopToRootViewController (false);
			});
			return true;
		}

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}

