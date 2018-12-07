using Foundation;
using System;
using UIKit;

namespace UIImageEffects
{
    public partial class ViewController : UIViewController
    {
        private const string IsFirstRunKey = "IsFirstRun";

        private EffectType currentEffect = EffectType.None;

        private UIImage image;

        protected ViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            effectLabel.Text = string.Empty;
            image = imageView.Image;
            ShowAlertForFirstRun();
        }

        partial void Update(UITapGestureRecognizer sender)
        {
            UIImage effectImage = null;
            var effectText = string.Empty;
            var effectColor = UIColor.White;

            switch (currentEffect)
            {
                case EffectType.None:
                    currentEffect = EffectType.Light;

                    effectImage = image.ApplyLightEffect();
                    effectText = "Light";
                    break;

                case EffectType.Light:
                    currentEffect = EffectType.ExtraLight;

                    effectImage = image.ApplyExtraLightEffect();
                    effectText = "Extra Light";
                    effectColor = UIColor.LightGray;
                    break;

                case EffectType.ExtraLight:
                    currentEffect = EffectType.Dark;

                    effectImage = image.ApplyDarkEffect();
                    effectText = "Dark";
                    effectColor = UIColor.DarkGray;
                    break;

                case EffectType.Dark:
                    currentEffect = EffectType.ColorTint;

                    effectImage = image.ApplyTintEffect(UIColor.Blue);
                    effectText = "Color tint";
                    effectColor = UIColor.DarkGray;
                    break;

                case EffectType.ColorTint:
                    currentEffect = EffectType.None;

                    effectImage = image;
                    break;
            }

            imageView.Image = effectImage;
            effectLabel.Text = effectText;
            effectLabel.TextColor = effectColor;
        }

        private void ShowAlertForFirstRun()
        {
            var isFirstRun = NSUserDefaults.StandardUserDefaults.BoolForKey(IsFirstRunKey);
            if (!isFirstRun)
            {
                var alert = UIAlertController.Create("Tap to change image effect", "", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Dismiss", UIAlertActionStyle.Default, null));
                PresentViewController(alert, true, null);

                NSUserDefaults.StandardUserDefaults.SetBool(true, IsFirstRunKey);
            }
        }
    }

    enum EffectType
    {
        None,
        Light,
        ExtraLight,
        Dark,
        ColorTint
    }
}