using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

using Foundation;
using UIKit;
using CoreText;

namespace Quotes
{
	public class Page : NSObject
	{
		// The paragraphs loaded from the XML file
		public List<XElement> Paragraphs { get; set; }

		public string Title { get; set; }
		public nint SelectedParagraph { get; set; }
		public nfloat LineHeight { get; set; }

		public Page (string title, List<XElement> paragraphs)
		{
			Title = title;
			Paragraphs = paragraphs;
			SelectedParagraph = NSRange.NotFound;
			LineHeight = 25.0f;
		}

		public string SpeakerForParagraph (XElement paragraph)
		{
			var next = paragraph.XPathSelectElement ("key[text()='speaker']").NextNode as XElement;
			return next.Value;
		}

		public string TextForParagraph (XElement paragraph)
		{
			var next = paragraph.XPathSelectElement ("key[text()='text']").NextNode as XElement;
			return next.Value;
		}

		public bool ParagraphIsStageDirection (XElement paragraph)
		{
			var next = paragraph.XPathSelectElement ("key[text()='speaker']").NextNode as XElement;
			var body = next.Value;

			return body == "STAGE DIRECTION";
		}

		public NSAttributedString AttributedStringForParagraph (XElement paragraph)
		{
			var returnValue = new NSMutableAttributedString ();

			// TODO: find stage directions and format them differently
			if (ParagraphIsStageDirection (paragraph)) {
				var stageDirection = new NSAttributedString (TextForParagraph (paragraph), 
				                                             font: UIFont.FromName ("Helvetica-LightOblique", 24),
				                                             paragraphStyle: new NSMutableParagraphStyle () { Alignment = UITextAlignment.Center, LineSpacing = 10});
				returnValue.Append (stageDirection);
			} else {
				var speaker = new NSAttributedString (SpeakerForParagraph (paragraph),
	                                                  font: UIFont.FromName ("HoeflerText-Black", 24),
				                                      foregroundColor: UIColor.Brown
				                                      );
				var text = new NSAttributedString (TextForParagraph (paragraph), 
				                                   font: UIFont.FromName ("HoeflerText-Regular", 24.0f),
				                                   foregroundColor: UIColor.Black
#if TEST_OTHER_ATTRIBUTES
				                                   ,backgroundColor: UIColor.Yellow,
				                                   ligatures: NSLigatureType.None,
				                                   kerning: 10,
				                                   underlineStyle: NSUnderlineStyle.Single,
				                                   shadow: new NSShadow () { ShadowColor = UIColor.Red, ShadowOffset = new CoreGraphics.CGSize (5, 5) },
												   strokeWidth: 5
#endif
				);

				returnValue.Append (speaker, "  ", text);
			}

			if (Paragraphs.IndexOf (paragraph) == SelectedParagraph) {
				returnValue.AddAttribute (UIStringAttributeKey.ForegroundColor, UIColor.White, new NSRange (0, returnValue.Length));
				returnValue.AddAttribute (UIStringAttributeKey.BackgroundColor, UIColor.FromHSB (.6f, .6f, .7f), new NSRange (0, returnValue.Length));
			}

			returnValue.EnumerateAttribute (UIStringAttributeKey.ParagraphStyle, new NSRange (0, returnValue.Length), NSAttributedStringEnumeration.LongestEffectiveRangeNotRequired, 
			                                (NSObject value, NSRange range, ref bool stop) => {
				var style = value == null ? new NSMutableParagraphStyle () : (NSMutableParagraphStyle)value.MutableCopy ();
				style.MinimumLineHeight = LineHeight;
				style.MaximumLineHeight = LineHeight;

				returnValue.AddAttribute (UIStringAttributeKey.ParagraphStyle, style, range);
			});

			return returnValue;
		}

		public string StringForParagraph (XElement paragraph)
		{
			string speaker = SpeakerForParagraph (paragraph);
			string text = TextForParagraph (paragraph);

			return speaker + ": " + text;
		}
	}
}
