using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace AstroLayout {
    [Register ("AstroViewController")]
    public class AstroViewController : UIViewController {
        
		NSLayoutConstraint[] regularConstraints;
		NSLayoutConstraint[] compactConstraints;
		List<NSLayoutConstraint> sharedConstraints;

		UIImageView mercury;
		UIImageView venus;
		UIImageView earth;
		UIImageView mars;
		UIImageView jupiter;
		UIImageView saturn;
		UIImageView uranus;
		UIImageView neptune;

		NSLayoutConstraint mercuryLeadingToTrailing;
		NSLayoutConstraint venusLeadingToTrailing;
		NSLayoutConstraint earthLeadingToTrailing;
		NSLayoutConstraint marsLeadingToTrailing;
		NSLayoutConstraint jupiterLeadingToTrailing;
		NSLayoutConstraint saturnLeadingToTrailing;
		NSLayoutConstraint uranusLeadingToTrailing;
		NSLayoutConstraint neptuneLeadingToTrailing;

		NSLayoutConstraint mercuryCenter;
		NSLayoutConstraint venusCenter;
		NSLayoutConstraint earthCenter;
		NSLayoutConstraint marsCenter;
		NSLayoutConstraint jupiterCenter;
		NSLayoutConstraint saturnCenter;
		NSLayoutConstraint uranusCenter;
		NSLayoutConstraint neptuneCenter;

		public AstroViewController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public AstroViewController (NSCoder coder) : base (coder)
		{
		}

		public override void ViewDidLoad ()
        {
			View.BackgroundColor = UIColor.Black;

			CreatePlanetViews ();
			CreateConstraints ();
			SetUpGestures ();
        }

		public override void TraitCollectionDidChange (UITraitCollection previousTraitCollection)
		{
			if (TraitCollection.Contains (UITraitCollection.FromHorizontalSizeClass (UIUserInterfaceSizeClass.Compact))) {
				if (regularConstraints [0].Active) {
					NSLayoutConstraint.DeactivateConstraints (regularConstraints);
					NSLayoutConstraint.ActivateConstraints (compactConstraints);
				}
			} else {
				if (compactConstraints [0].Active) {
					NSLayoutConstraint.DeactivateConstraints (compactConstraints);
					NSLayoutConstraint.ActivateConstraints (regularConstraints);
				}
			}
		}

        void CreateConstraints ()
        {
			PlanetSizes ();
			CreateCompactConstraints ();
			CreateRegularConstraints ();

			NSLayoutConstraint.ActivateConstraints (regularConstraints);
			NSLayoutConstraint.ActivateConstraints (sharedConstraints.ToArray ());
        }

        void SetLayoutIdentifierForArray (NSString identifier, NSArray constraintsArray)
		{
			for (nuint i = 0; i < constraintsArray.Count; i++) {
				var constraint = constraintsArray.GetItem<NSLayoutConstraint> (i);
				constraint.SetIdentifier (identifier);
			}
        }

        void CreatePlanetViews ()
        {   
			mercury = CreatePlanet ("Mercury");
			venus = CreatePlanet ("Venus");
			earth = CreatePlanet ("Earth");
			mars = CreatePlanet ("Mars");
			jupiter = CreatePlanet ("Jupiter");
			saturn = CreatePlanet ("Saturn");
			uranus = CreatePlanet ("Uranus");
			neptune = CreatePlanet ("Neptune");
        }

		UIImageView CreatePlanet (string planetName)
		{
			var image = UIImage.FromBundle (planetName);
			var planet = new UIImageView (image) {
				TranslatesAutoresizingMaskIntoConstraints = false,
				ContentMode = UIViewContentMode.ScaleAspectFit,
				AccessibilityIdentifier = planetName
			};
			View.AddSubview (planet);
			return planet;
		}

        void PlanetSizes ()
        {
			NSLayoutConstraint mercuryHeight = mercury.HeightAnchor.ConstraintEqualTo (earth.HeightAnchor, .38f);
			NSLayoutConstraint mercuryWidth = mercury.WidthAnchor.ConstraintEqualTo (mercury.HeightAnchor, 1f);

			NSLayoutConstraint venusHeight = venus.HeightAnchor.ConstraintEqualTo (earth.HeightAnchor, .95f);
			NSLayoutConstraint venusWidth = venus.WidthAnchor.ConstraintEqualTo (venus.HeightAnchor, 1f);
            
			NSLayoutConstraint marsHeight = mars.HeightAnchor.ConstraintEqualTo (earth.HeightAnchor, .53f);
			NSLayoutConstraint marsWidth = mars.WidthAnchor.ConstraintEqualTo (mars.HeightAnchor, 1f);

			NSLayoutConstraint jupiterHeight = jupiter.HeightAnchor.ConstraintEqualTo (earth.HeightAnchor, 11.2f);
			NSLayoutConstraint jupiterWidth = jupiter.WidthAnchor.ConstraintEqualTo (jupiter.HeightAnchor, 1f);

			NSLayoutConstraint saturnHeight = saturn.HeightAnchor.ConstraintEqualTo (earth.HeightAnchor, 9.45f);
			NSLayoutConstraint saturnWidth = saturn.WidthAnchor.ConstraintEqualTo (saturn.HeightAnchor, 1.5f);

			NSLayoutConstraint uranusHeight = uranus.HeightAnchor.ConstraintEqualTo (earth.HeightAnchor, 4f);
			NSLayoutConstraint uranusWidth = uranus.WidthAnchor.ConstraintEqualTo (uranus.HeightAnchor, 1f);
            
			NSLayoutConstraint neptuneHeight = neptune.HeightAnchor.ConstraintEqualTo (earth.HeightAnchor, 3.88f);
			NSLayoutConstraint neptuneWidth = neptune.HeightAnchor.ConstraintEqualTo (neptune.HeightAnchor, 1f);
            
			NSLayoutConstraint earthWidth = earth.WidthAnchor.ConstraintEqualTo (earth.HeightAnchor);
            
			mercuryHeight.SetIdentifier ("mercuryHeight");
			mercuryHeight.SetIdentifier ("mercuryHeight");
			mercuryWidth.SetIdentifier ("mercuryWidth");
			venusHeight.SetIdentifier ("venusHeight");
			venusWidth.SetIdentifier ("venusWidth");
			marsHeight.SetIdentifier ("marsHeight");
			marsWidth.SetIdentifier ("marsWidth");
			jupiterHeight.SetIdentifier ("jupiterHeight");
			jupiterWidth.SetIdentifier ("jupiterWidth");
			saturnHeight.SetIdentifier ("saturnHeight");
			saturnWidth.SetIdentifier ("saturnWidth");
			uranusHeight.SetIdentifier ("uranusHeight");
			uranusWidth.SetIdentifier ("uranusWidth");
			neptuneHeight.SetIdentifier ("neptuneHeight");
			neptuneWidth.SetIdentifier ("neptuneWidth");
			earthWidth.SetIdentifier ("earthWidth");

			NSLayoutConstraint.ActivateConstraints (new [] {
				mercuryHeight, venusHeight, marsHeight, jupiterHeight, saturnHeight,
				uranusHeight, neptuneHeight, mercuryWidth, venusWidth, earthWidth,
				marsWidth, jupiterWidth, saturnWidth, uranusWidth, neptuneWidth
			});
		}

        void CreateCompactConstraints()
        {
			if (compactConstraints?.Length > 0)
				return;
			
            mercuryCenter = CreateCenterXConstraint (mercury, "mercuryCenterX");
			venusCenter = CreateCenterXConstraint (venus, "venusCenterX");
			earthCenter = CreateCenterXConstraint (earth, "earthCenterX");
			marsCenter = CreateCenterXConstraint (mars, "marsCenterX");
			jupiterCenter = CreateCenterXConstraint (jupiter, "jupiterCenterX");
			saturnCenter = CreateCenterXConstraint (saturn, "saturnCenterX");
			uranusCenter = CreateCenterXConstraint (uranus, "uranusCenterX");
			neptuneCenter = CreateCenterXConstraint (neptune, "neptuneCenterX");

			compactConstraints = new [] {
				mercuryCenter, venusCenter, earthCenter, marsCenter, jupiterCenter, saturnCenter, uranusCenter, neptuneCenter
			};
        }
			
		NSLayoutConstraint CreateCenterXConstraint (UIView planetToCenter, string identifierName)
		{
			NSLayoutConstraint newConstraint = planetToCenter.CenterXAnchor.ConstraintEqualTo (View.CenterXAnchor);
			newConstraint.SetIdentifier (identifierName);
			return newConstraint;
		}

        void CreateRegularConstraints ()
        {
			if (regularConstraints?.Length > 0 && sharedConstraints?.Count > 0)
            	return;
			
            UILayoutGuide leadingMercuryGuide = NewLayoutGuide ("leadingMercuryGuide");
			UILayoutGuide leadingVenusGuide = NewLayoutGuide ("leadingVenusGuide");
			UILayoutGuide leadingEarthGuide = NewLayoutGuide ("leadingEarthGuide");
			UILayoutGuide leadingMarsGuide = NewLayoutGuide ("leadingMarsGuide");
			UILayoutGuide leadingJupiterGuide = NewLayoutGuide ("leadingJupiterGuide");
			UILayoutGuide leadingSaturnGuide = NewLayoutGuide ("leadingSaturnGuide");
			UILayoutGuide leadingUranusGuide = NewLayoutGuide ("leadingUranusGuide");
			UILayoutGuide leadingNeptuneGuide = NewLayoutGuide ("leadingNeptuneGuide");

			UILayoutGuide trailingMercuryGuide = NewLayoutGuide ("trailingMercuryGuide");
			UILayoutGuide trailingVenusGuide = NewLayoutGuide ("trailingVenusGuide");
			UILayoutGuide trailingEarthGuide = NewLayoutGuide ("trailingEarthGuide");
			UILayoutGuide trailingMarsGuide = NewLayoutGuide ("trailingMarsGuide");
			UILayoutGuide trailingJupiterGuide = NewLayoutGuide ("trailingJupiterGuide");
			UILayoutGuide trailingSaturnGuide = NewLayoutGuide ("trailingSaturnGuide");
			UILayoutGuide trailingUranusGuide = NewLayoutGuide ("trailingUranusGuide");
			UILayoutGuide trailingNeptuneGuide = NewLayoutGuide ("trailingNeptuneGuide");
            
			IUILayoutSupport topLayoutGuide = TopLayoutGuide;

			var planetsAndGuides = NSDictionary.FromObjectsAndKeys (new object[] {
				mercury, venus, earth, mars, jupiter, saturn, uranus, neptune,
				leadingMercuryGuide, leadingVenusGuide, leadingEarthGuide, leadingMarsGuide,
				leadingJupiterGuide, leadingSaturnGuide, leadingUranusGuide, leadingNeptuneGuide,
				trailingMercuryGuide, trailingVenusGuide, trailingEarthGuide, trailingMarsGuide,
				trailingJupiterGuide, trailingSaturnGuide, trailingUranusGuide, trailingNeptuneGuide, topLayoutGuide
			}, new object[] {
				"mercury", "venus", "earth", "mars", "jupiter", "saturn", "uranus", "neptune",
				"leadingMercuryGuide", "leadingVenusGuide", "leadingEarthGuide", "leadingMarsGuide",
				"leadingJupiterGuide", "leadingSaturnGuide", "leadingUranusGuide", "leadingNeptuneGuide",
				"trailingMercuryGuide", "trailingVenusGuide", "trailingEarthGuide", "trailingMarsGuide",
				"trailingJupiterGuide", "trailingSaturnGuide", "trailingUranusGuide", "trailingNeptuneGuide", "topLayoutGuide"
			});

			var topToBottom = NSLayoutConstraint.FromVisualFormat ("V:|[topLayoutGuide]-[leadingMercuryGuide]-" +
				"[leadingVenusGuide]-[leadingEarthGuide]-[leadingMarsGuide]-" +
				"[leadingJupiterGuide][leadingSaturnGuide][leadingUranusGuide]-" +
				"[leadingNeptuneGuide]-20-|", NSLayoutFormatOptions.DirectionLeadingToTrailing, null, planetsAndGuides);
                
			sharedConstraints = new List<NSLayoutConstraint> (topToBottom);
			SetLayoutIdentifierForArray ((NSString)"topToBottom", NSArray.FromObjects (topToBottom));

			NewHorizontalArray ("|[leadingMercuryGuide][mercury][trailingMercuryGuide]|", "hMercury", planetsAndGuides);
			NewHorizontalArray ("|[leadingVenusGuide][venus][trailingVenusGuide]|", "hVenus", planetsAndGuides);
			NewHorizontalArray ("|[leadingEarthGuide][earth][trailingEarthGuide]|", "hEarth", planetsAndGuides);
			NewHorizontalArray ("|[leadingMarsGuide][mars][trailingMarsGuide]|", "hMars", planetsAndGuides);
			NewHorizontalArray ("|[leadingJupiterGuide][jupiter][trailingJupiterGuide]|", "hJupiter", planetsAndGuides);
			NewHorizontalArray ("|[leadingSaturnGuide][saturn][trailingSaturnGuide]|", "hSaturn", planetsAndGuides);
			NewHorizontalArray ("|[leadingUranusGuide][uranus][trailingUranusGuide]|", "hUranus", planetsAndGuides);
			NewHorizontalArray ("|[leadingNeptuneGuide][neptune][trailingNeptuneGuide]|", "hNeptune", planetsAndGuides);
            
			sharedConstraints.Add (GuideHeightToPlanet (leadingMercuryGuide, mercury, "guideHeightToMercury"));
			sharedConstraints.Add (GuideHeightToPlanet (leadingVenusGuide, venus, "guideHeightToVenus"));
			sharedConstraints.Add (GuideHeightToPlanet (leadingEarthGuide, earth, "guideHeightToEarth"));
			sharedConstraints.Add (GuideHeightToPlanet (leadingMarsGuide, mars, "guideHeightToMars"));
			sharedConstraints.Add (GuideHeightToPlanet (leadingJupiterGuide, jupiter, "guideHeightToJupiter"));
			sharedConstraints.Add (GuideHeightToPlanet (leadingSaturnGuide, saturn, "guideHeightToSaturn"));
			sharedConstraints.Add (GuideHeightToPlanet (leadingUranusGuide, uranus, "guideHeightToUranus"));
			sharedConstraints.Add (GuideHeightToPlanet (leadingNeptuneGuide, neptune, "guideHeightToNeptune"));

			mercuryLeadingToTrailing = leadingMercuryGuide.WidthAnchor.ConstraintEqualTo (trailingMercuryGuide.WidthAnchor, .02f);
			venusLeadingToTrailing = leadingVenusGuide.WidthAnchor.ConstraintEqualTo (trailingVenusGuide.WidthAnchor, .03f);
			earthLeadingToTrailing = leadingEarthGuide.WidthAnchor.ConstraintEqualTo (trailingEarthGuide.WidthAnchor, .06f);
			marsLeadingToTrailing = leadingMarsGuide.WidthAnchor.ConstraintEqualTo (trailingMarsGuide.WidthAnchor, .1f);
			jupiterLeadingToTrailing = leadingJupiterGuide.WidthAnchor.ConstraintEqualTo (trailingJupiterGuide.WidthAnchor, .2f);
			saturnLeadingToTrailing = leadingSaturnGuide.WidthAnchor.ConstraintEqualTo (trailingSaturnGuide.WidthAnchor, 1f);
			uranusLeadingToTrailing = leadingUranusGuide.WidthAnchor.ConstraintEqualTo (trailingUranusGuide.WidthAnchor, 2.7f);
			neptuneLeadingToTrailing = leadingNeptuneGuide.WidthAnchor.ConstraintEqualTo (trailingNeptuneGuide.WidthAnchor, 10f);
            
			mercuryLeadingToTrailing.SetIdentifier ("leadingTrailingAnchorMercury");
			venusLeadingToTrailing.SetIdentifier ("leadingTrailingAnchorVenus");
			earthLeadingToTrailing.SetIdentifier ("leadingTrailingAnchorEarth");
			marsLeadingToTrailing.SetIdentifier ("leadingTrailingAnchorMars");
			jupiterLeadingToTrailing.SetIdentifier ("leadingTrailingAnchorJupiter");
			saturnLeadingToTrailing.SetIdentifier ("leadingTrailingAnchorSaturn");
			uranusLeadingToTrailing.SetIdentifier ("leadingTrailingAnchorUranus");
			neptuneLeadingToTrailing.SetIdentifier ("leadingTrailingAnchorNeptune");
            
			regularConstraints = new [] {
				mercuryLeadingToTrailing, venusLeadingToTrailing, earthLeadingToTrailing, marsLeadingToTrailing,
				saturnLeadingToTrailing, jupiterLeadingToTrailing,  uranusLeadingToTrailing, neptuneLeadingToTrailing
			};
		}

		void NewHorizontalArray (string layoutString, string arrayID, NSDictionary planetsAndGuides)
		{
			var horizontalConstraintsArray = NSLayoutConstraint.FromVisualFormat (layoutString, NSLayoutFormatOptions.AlignAllCenterY, null, planetsAndGuides);
			sharedConstraints.AddRange (horizontalConstraintsArray);
			SetLayoutIdentifierForArray ((NSString)arrayID, NSArray.FromObjects (horizontalConstraintsArray));
		}

		NSLayoutConstraint GuideHeightToPlanet (UILayoutGuide layoutGuide, UIView planet, string identifier)
		{
			NSLayoutConstraint guideHeightToPlanet = layoutGuide.HeightAnchor.ConstraintEqualTo (planet.HeightAnchor);
			guideHeightToPlanet.SetIdentifier (identifier);
			return guideHeightToPlanet;
		}

		UILayoutGuide NewLayoutGuide (string identifierName)
		{
			var newGuide = new UILayoutGuide {
				Identifier = identifierName
			};

			View.AddLayoutGuide (newGuide);
			return newGuide;
		}

        void ChangeLayout (UIGestureRecognizer tapGesture)
        {
			if (tapGesture.State != UIGestureRecognizerState.Ended)
				return;

			NSLayoutConstraint regularConstraint = regularConstraints.First ();
			NSLayoutConstraint compactConstraint = compactConstraints.First ();
			if (regularConstraint.Active) {
				UIView.Animate (1.0, () => {
					NSLayoutConstraint.DeactivateConstraints (regularConstraints);
					NSLayoutConstraint.ActivateConstraints (compactConstraints);
					View.LayoutIfNeeded ();
				});
			} else if (compactConstraint.Active) {
				UIView.Animate (1.0, () => {
					NSLayoutConstraint.DeactivateConstraints (compactConstraints);
					NSLayoutConstraint.ActivateConstraints (regularConstraints);
					View.LayoutIfNeeded ();
				});
			}
        }

        void KeyframeBasedLayoutChange (UIGestureRecognizer twoFingerDoubleTap)
        {
			if (twoFingerDoubleTap.State != UIGestureRecognizerState.Ended)
				return;

			NSLayoutConstraint regularConstraint = regularConstraints.First ();
			NSLayoutConstraint compactConstraint = compactConstraints.First ();
			if (regularConstraint.Active) {
				UIView.AnimateKeyframes (1.5, 0.0, UIViewKeyframeAnimationOptions.CalculationModeLinear,
					AnimateToCompact, finished => {
				});
			} else if (compactConstraint.Active) {
				UIView.AnimateKeyframes (1.5, 0.0, UIViewKeyframeAnimationOptions.CalculationModeLinear,
					AnimateToRegular, finished => {
				});
			}
        }

        void AnimateToRegular ()
        {
			UIView.AddKeyframeWithRelativeStartTime (0.0, 1.0, () => {
				NSLayoutConstraint.DeactivateConstraints (new [] { mercuryCenter });
				NSLayoutConstraint.ActivateConstraints (new [] { mercuryLeadingToTrailing });
				View.LayoutIfNeeded ();
			});

			UIView.AddKeyframeWithRelativeStartTime (0.0, 0.9, () => {
				NSLayoutConstraint.DeactivateConstraints (new [] { venusCenter, neptuneCenter });
				NSLayoutConstraint.ActivateConstraints (new [] { venusLeadingToTrailing, neptuneLeadingToTrailing });
				View.LayoutIfNeeded ();
			});

			UIView.AddKeyframeWithRelativeStartTime (0.0, 0.7, () => {
				NSLayoutConstraint.DeactivateConstraints (new [] { earthCenter, uranusCenter });
				NSLayoutConstraint.ActivateConstraints (new [] { earthLeadingToTrailing, uranusLeadingToTrailing });
				View.LayoutIfNeeded ();
			});

			UIView.AddKeyframeWithRelativeStartTime (0.0, 0.5, () => {
				NSLayoutConstraint.DeactivateConstraints (new [] { marsCenter, jupiterCenter, saturnCenter });
				NSLayoutConstraint.ActivateConstraints (new [] { marsLeadingToTrailing, jupiterLeadingToTrailing, saturnLeadingToTrailing });
				View.LayoutIfNeeded ();
			});
        }

        void AnimateToCompact ()
        {
			UIView.AddKeyframeWithRelativeStartTime (0.0, 1.0, () => {
				NSLayoutConstraint.DeactivateConstraints (new [] { marsLeadingToTrailing, jupiterLeadingToTrailing, saturnLeadingToTrailing });
				NSLayoutConstraint.ActivateConstraints (new [] { marsCenter, jupiterCenter, saturnCenter });
				View.LayoutIfNeeded ();
			});

			UIView.AddKeyframeWithRelativeStartTime (0.1, 0.9, () => {
				NSLayoutConstraint.DeactivateConstraints (new [] { earthLeadingToTrailing, uranusLeadingToTrailing });
				NSLayoutConstraint.ActivateConstraints (new [] { earthCenter, uranusCenter });
				View.LayoutIfNeeded ();
			});

			UIView.AddKeyframeWithRelativeStartTime (0.3, 0.7, () => {
				NSLayoutConstraint.DeactivateConstraints (new [] { venusLeadingToTrailing, neptuneLeadingToTrailing });
				NSLayoutConstraint.ActivateConstraints (new [] { venusCenter, neptuneCenter });
				View.LayoutIfNeeded ();
			});

			UIView.AddKeyframeWithRelativeStartTime (0.5, 0.5, () => {
				NSLayoutConstraint.DeactivateConstraints (new [] { mercuryLeadingToTrailing });
				NSLayoutConstraint.ActivateConstraints (new [] { mercuryCenter });
				View.LayoutIfNeeded ();
			});
        }

        void SetUpGestures ()
        {
			UITapGestureRecognizer doubleTap = null;
			doubleTap = new UITapGestureRecognizer (() => ChangeLayout (doubleTap)) {
				NumberOfTapsRequired = 2,
				NumberOfTouchesRequired = 1
			};
			View.AddGestureRecognizer (doubleTap);

			UITapGestureRecognizer twoFingerDoubleTap = null;
			twoFingerDoubleTap = new UITapGestureRecognizer (() => KeyframeBasedLayoutChange (twoFingerDoubleTap)) {
				NumberOfTapsRequired = 2,
				NumberOfTouchesRequired = 2
			};
			View.AddGestureRecognizer (twoFingerDoubleTap);
        }
    }
}