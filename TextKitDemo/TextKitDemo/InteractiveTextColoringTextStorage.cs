using System;
using System.Collections.Generic;

using Foundation;
using UIKit;
using CoreText;

namespace TextKitDemo
{
	/*
	 * Shows how to use color specific portions of a buffer.
	 */
	public class InteractiveTextColoringTextStorage : NSTextStorage
	{
		public Dictionary<string, NSDictionary> Tokens; 
		NSMutableAttributedString backingStore;
		bool dynamicTextNeedsUpdate;

		public InteractiveTextColoringTextStorage () : base ()
		{
			backingStore = new NSMutableAttributedString ();
		}

		public override IntPtr LowLevelValue {
			get {
				return backingStore.LowLevelValue;
			}
		}

		public override IntPtr LowLevelGetAttributes (nint location, out NSRange effectiveRange)
		{
			return backingStore.LowLevelGetAttributes (location, out effectiveRange);
		}

		public override void Replace (NSRange range, string newValue)
		{
			BeginEditing ();
			backingStore.Replace (range, newValue);
			Edited (NSTextStorageEditActions.Characters | NSTextStorageEditActions.Attributes, range, newValue.Length - range.Length);
			dynamicTextNeedsUpdate = true;
			EndEditing ();
		}

		public override void LowLevelSetAttributes (IntPtr attrs, NSRange range)
		{
			BeginEditing ();
			backingStore.LowLevelSetAttributes (attrs, range);
			Edited (NSTextStorageEditActions.Attributes, range, 0);
			EndEditing ();
		}

		void PerformReplacementsForCharacterChange (NSRange changeRange)
		{
			if (changeRange.Location == 0 && changeRange.Length == 0)
				return;

			var searchRange = new NSRange (0,Value.Length);
			nint startLoc = Value.Substring (0, (int)changeRange.Location).LastIndexOfAny (" \n\t.".ToCharArray ());
			nint endLoc = Value.Substring ((int)(changeRange.Location + changeRange.Length)).IndexOfAny (" \n\t.".ToCharArray ());
			if(startLoc != -1)
				searchRange.Location = startLoc;
			if (endLoc != -1)
				searchRange.Length = changeRange.Location + endLoc + 1 - startLoc;
			else
				searchRange.Length = searchRange.Length - searchRange.Location;

			searchRange.Location = (nint)Math.Min (searchRange.Location, changeRange.Location);
			searchRange.Length = (nint)Math.Max (searchRange.Length, changeRange.Length);
			ApplyTokenAttributes (searchRange);
		}

		void ApplyTokenAttributes (NSRange searchRange)
		{
			NSDictionary attributesForToken = Tokens ["DefaultTokenName"];
			nint startPos = searchRange.Location;

			string text = backingStore.Value.Substring ((int)searchRange.Location,(int)searchRange.Length);
			int nextSpace = text.IndexOfAny (" \n\t.".ToCharArray ());
			int lastPos = 0;
			string token;
			NSRange tokenRange;
			while (true) {
				if (nextSpace == -1)
					nextSpace = text.Length;

				token = text.Substring (lastPos, nextSpace - lastPos);
				tokenRange = new NSRange (lastPos + startPos, nextSpace - lastPos);

				if (Tokens.ContainsKey (token))
					attributesForToken = Tokens [token];
				else
					attributesForToken = Tokens ["DefaultTokenName"];

				AddAttributes (attributesForToken, tokenRange);
				if (nextSpace == text.Length)
					break;
				lastPos = nextSpace + 1;
				nextSpace = text.IndexOfAny (" \n\t.".ToCharArray (), nextSpace + 1);
			}
		}

		public override void ProcessEditing ()
		{
			if (dynamicTextNeedsUpdate) {
				dynamicTextNeedsUpdate = true;
				PerformReplacementsForCharacterChange (EditedRange);
			}

			base.ProcessEditing ();
		}
	}
}

