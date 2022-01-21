
namespace ARMultiuser;

/// <summary>
/// A custom button that stands out over the camera view.
/// </summary>
[Register ("RoundedButton")]
public partial class RoundedButton : UIButton
{
        [Export ("initWithCoder:")]
        public RoundedButton (NSCoder coder) : base (coder)
        {
                Setup ();
        }

        [Export ("initWithFrame:")]
        public RoundedButton (CGRect frame) : base (frame)
        {
                Setup ();
        }

        void Setup ()
        {
                BackgroundColor = TintColor;
                Layer.CornerRadius = 8f;
                ClipsToBounds = true;
                SetTitleColor (UIColor.White, UIControlState.Normal);
                if (TitleLabel is not null)
                {
                        TitleLabel.Font = UIFont.BoldSystemFontOfSize (17f);
                }
        }

        public override bool Enabled
        {
                get => base.Enabled;

                set
                {
                        base.Enabled = value;
                        BackgroundColor = value ? TintColor : UIColor.Gray;
                }
        }
}
