using CoreGraphics;
using MapCallouts.Annotations;
using MapKit;
using UIKit;

namespace MapCallouts
{
    /// <summary>
    /// The custom MKAnnotationView object representing a generic location, displaying a title and image.
    /// </summary>
    public class CustomAnnotationView : MKAnnotationView
    {
        private const float BoxInset = 10f;
        private const float InterItemSpacing = 10f;
        private const float MaxContentWidth = 90f;
        private readonly UIEdgeInsets contentInsets = new UIEdgeInsets(10f, 30f, 20f, 20f);

        private NSLayoutConstraint imageHeightConstraint;
        private UILabel label;
        private UIImageView imageView;
        private UIStackView stackView;

        [Foundation.Export("initWithAnnotation:reuseIdentifier:")]
        public CustomAnnotationView(MKAnnotation annotation, string reuseIdentifier) : base(annotation, reuseIdentifier)
        {
            this.BackgroundColor = UIColor.Clear;
            this.TranslatesAutoresizingMaskIntoConstraints = false;

            this.InitializeElements();

            base.AddSubview(this.stackView);

            // Anchor the top and leading edge of the stack view to let it grow to the content size.
            this.stackView.LeadingAnchor.ConstraintEqualTo(this.LeadingAnchor, this.contentInsets.Left).Active = true;
            this.stackView.TopAnchor.ConstraintEqualTo(this.TopAnchor, this.contentInsets.Top).Active = true;

            // Limit how much the content is allowed to grow.
            this.imageView.WidthAnchor.ConstraintLessThanOrEqualTo(MaxContentWidth).Active = true;
            this.label.WidthAnchor.ConstraintEqualTo(this.imageView.WidthAnchor).Active = true;
        }

        public override CGSize IntrinsicContentSize
        {
            get
            {
                var size = this.stackView.Bounds.Size;
                size.Width += this.contentInsets.Left + this.contentInsets.Right;
                size.Height += this.contentInsets.Top + this.contentInsets.Bottom;

                return size;
            }
        }

        private void InitializeElements()
        {
            this.label = new UILabel(CGRect.Empty)
            {
                TextColor = UIColor.White,
                LineBreakMode = UILineBreakMode.WordWrap,
                BackgroundColor = UIColor.Clear,
                Lines = 2,
                Font = UIFont.PreferredCaption1
            };

            this.imageView = new UIImageView();

            this.stackView = new UIStackView(new UIView[] { this.label, this.imageView })
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Axis = UILayoutConstraintAxis.Vertical,
                Alignment = UIStackViewAlignment.Top,
                Spacing = InterItemSpacing
            };
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();
            this.imageView.Image = null;
            this.label.Text = null;
        }

        public override void PrepareForDisplay()
        {
            base.PrepareForDisplay();

            /*
             * If using the same annotation view and reuse identifier for multiple annotations, iOS will reuse this view by calling `prepareForReuse()`
             * so the view can be put into a known default state, and `prepareForDisplay()` right before the annotation view is displayed. This method is
             * the view's oppurtunity to update itself to display content for the new annotation.
             */
            if (base.Annotation is CustomAnnotation annotation)
            {
                this.label.Text = annotation.Title;

                using (var image = UIImage.FromBundle(annotation.ImageName))
                {
                    this.imageView.Image = image;

                    /*
                     The image view has a width constraint to keep the image to a reasonable size. A height constraint to keep the aspect ratio
                     proportions of the image is required to keep the image packed into the stack view. Without this constraint, the image's height
                     will remain the intrinsic size of the image, resulting in extra height in the stack view that is not desired.
                     */

                    if (this.imageHeightConstraint != null)
                    {
                        this.imageView.RemoveConstraint(this.imageHeightConstraint);
                    }

                    var ratio = image.Size.Height / image.Size.Width;
                    this.imageHeightConstraint = this.imageView.HeightAnchor.ConstraintEqualTo(this.imageView.WidthAnchor, ratio, 0);
                    this.imageHeightConstraint.Active = true;
                }
            }

            // Since the image and text sizes may have changed, require the system do a layout pass to update the size of the subviews.
            this.SetNeedsLayout();
        }

        public override void LayoutSubviews()
        {
            // The stack view will not have a size until a `layoutSubviews()` pass is completed. As this view's overall size is the size
            // of the stack view plus a border area, the layout system needs to know that this layout pass has invalidated this view's
            // `intrinsicContentSize`.
            this.InvalidateIntrinsicContentSize();

            // The annotation view's center is at the annotation's coordinate. For this annotation view, offset the center so that the
            // drawn arrow point is the annotation's coordinate.
            var contentSize = this.IntrinsicContentSize;
            this.CenterOffset = new CGPoint(contentSize.Width / 2f, contentSize.Height / 2f);

            // Now that the view has a new size, the border needs to be redrawn at the new size.
            this.SetNeedsDisplay();
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            // Used to draw the rounded background box and pointer.
            UIColor.DarkGray.SetFill();

            // Draw the pointed shape.
            using (var pointShape = new UIBezierPath())
            {
                pointShape.MoveTo(new CGPoint(14f, 0f));
                pointShape.AddLineTo(CGPoint.Empty);
                pointShape.AddLineTo(new CGPoint(rect.Size.Width, rect.Size.Height));
                pointShape.Fill();
            }

            // Draw the rounded box.
            var box = new CGRect(BoxInset, 0, rect.Size.Width - BoxInset, rect.Size.Height);
            using (var roundedRect = UIBezierPath.FromRoundedRect(box, 5f))
            {
                roundedRect.LineWidth = 2f;
                roundedRect.Fill();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (this.label != null)
            {
                this.label.Dispose();
                this.label = null;
            }

            if (this.imageView != null)
            {
                this.imageView.Dispose();
                this.imageView = null;
            }

            if (this.stackView != null)
            {
                this.stackView.Dispose();
                this.stackView = null;
            }

            if (this.imageHeightConstraint != null)
            {
                this.imageHeightConstraint.Dispose();
                this.imageHeightConstraint = null;
            }
        }
    }
}