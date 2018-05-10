
namespace Fox2.macOS
{
    using System;
    using Foundation;
    using SceneKit;

    [Register("GameViewMacOS")]
    public class GameViewMacOS : SCNView
    {
        public GameViewMacOS(IntPtr handle) : base(handle) { }

        public WeakReference<GameViewControllerMacOS> ViewController { get; set; }

		public override void KeyDown(AppKit.NSEvent @event)
        {
            GameViewControllerMacOS controller;
            if (this.ViewController.TryGetTarget(out controller))
            {
                if (!controller.KeyDown(this, @event))
                {
                    base.KeyDown(@event);
                }
            }
        }

		public override void KeyUp(AppKit.NSEvent @event)
        {
            GameViewControllerMacOS controller;
            if (this.ViewController.TryGetTarget(out controller))
            {
                if (!controller.KeyUp(this, @event))
                {
                    base.KeyUp(@event);
                }
            }
        }

        public override void SetFrameSize(CoreGraphics.CGSize newSize)
        {
            base.SetFrameSize(newSize);
            (this.OverlayScene as Overlay)?.Layout2DOverlay();
        }

        public override void ViewDidMoveToWindow()
        {
            //disable retina
            this.Layer.ContentsScale = 1f;
        }
    }
}
