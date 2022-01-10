﻿/*
See LICENSE folder for this sample’s licensing information.

Abstract:
A cell for displaying an item in our outline view
*/

using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Conference_Diffable {
	public partial class OutlineItemCell : UICollectionViewCell {
		public static readonly NSString Key = new NSString (nameof (OutlineItemCell));

		public UILabel Label { get; private set; }
		public UIView ContainerView { get; private set; }
		public UIImageView ImageView { get; private set; }

		int indentLevel;
		public int IndentLevel {
			get => indentLevel;
			set {
				indentLevel = value;
				indentConstraint.Constant = 20 * indentLevel;
			}
		}

		bool expanded;
		public bool Expanded {
			get => expanded;
			set {
				expanded = value;
				ConfigureChevron ();
			}
		}

		bool group;
		public bool Group {
			get => group;
			set {
				group = value;
				ConfigureChevron ();
			}
		}

		public override bool Highlighted {
			get => base.Highlighted;
			set {
				base.Highlighted = value;
				ConfigureChevron ();
			}
		}

		public override bool Selected {
			get => base.Selected;
			set {
				base.Selected = value;
				ConfigureChevron ();
			}
		}

		protected OutlineItemCell (IntPtr handle) : base (handle) { }

		public void ConfigureIfNeeded ()
		{
			if (Label != null)
				return;

			Configure ();
			ConfigureChevron ();
		}

		NSLayoutConstraint indentConstraint;
		nfloat inset = 10;

		private void Configure ()
		{
			ContainerView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false };

			ImageView = new UIImageView { TranslatesAutoresizingMaskIntoConstraints = false };
			ContainerView.AddSubview (ImageView);

			ContentView.AddSubview (ContainerView);

			Label = new UILabel {
				TranslatesAutoresizingMaskIntoConstraints = false,
				Font = UIFont.GetPreferredFontForTextStyle (UIFontTextStyle.Headline),
				AdjustsFontForContentSizeCategory = true
			};
			ContainerView.AddSubview (Label);

			indentConstraint = ContainerView.LeadingAnchor.ConstraintEqualTo (ContentView.LeadingAnchor, inset);
			indentConstraint.Active = true;

			ContainerView.TrailingAnchor.ConstraintEqualTo (ContentView.TrailingAnchor).Active = true;
			ContainerView.TopAnchor.ConstraintEqualTo (ContentView.TopAnchor).Active = true;
			ContainerView.BottomAnchor.ConstraintEqualTo (ContentView.BottomAnchor).Active = true;

			ImageView.LeadingAnchor.ConstraintEqualTo (ContainerView.LeadingAnchor, inset).Active = true;
			ImageView.HeightAnchor.ConstraintEqualTo (25).Active = true;
			ImageView.WidthAnchor.ConstraintEqualTo (25).Active = true;
			ImageView.CenterYAnchor.ConstraintEqualTo (ContainerView.CenterYAnchor).Active = true;

			Label.LeadingAnchor.ConstraintEqualTo (ImageView.TrailingAnchor, inset).Active = true;
			Label.TrailingAnchor.ConstraintEqualTo (ContainerView.TrailingAnchor).Active = true;
			Label.BottomAnchor.ConstraintEqualTo (ContainerView.BottomAnchor).Active = true;
			Label.TopAnchor.ConstraintEqualTo (ContainerView.TopAnchor).Active = true;
		}

		void ConfigureChevron ()
		{
			var rtl = EffectiveUserInterfaceLayoutDirection == UIUserInterfaceLayoutDirection.RightToLeft;
			var chevron = rtl ? "chevron.left.circle.fill" : "chevron.right.circle.fill";
			var chevronSelected = rtl ? "chevron.left.circle.fill" : "chevron.right.circle.fill";
			var circle = "circle.fill";
			var circleFill = "circle.fill";
			var highlighted = Highlighted || Selected;

			if (Group) {
				var imageName = highlighted ? chevronSelected : chevron;
				var image = UIImage.GetSystemImage (imageName);
				ImageView.Image = image;
				var rtlMultiplier = rtl ? -1.0 : 1.0;
				var rotationTransform = Expanded ?
					CGAffineTransform.MakeRotation ((nfloat)(rtlMultiplier * Math.PI / 2)) : CGAffineTransform.MakeIdentity ();
				ImageView.Transform = rotationTransform;
			} else {
				var imageName = Highlighted ? circleFill : circle;
				var image = UIImage.GetSystemImage (imageName);
				ImageView.Image = image;
				ImageView.Transform = CGAffineTransform.MakeIdentity ();
			}

			ImageView.TintColor = Highlighted ? UIColor.Gray : UIColorExtensions.CornflowerBlue;
		}
	}
}
