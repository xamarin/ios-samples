
namespace Fox2
{
    using CoreGraphics;
    using Foundation;
    using Fox2.UI;
    using SpriteKit;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class manages the 2D overlay (score).
    /// </summary>
    public class Overlay : SKScene
    {
        private List<SKSpriteNode> collectedGemsSprites;
        private SKNode congratulationsGroupNode;
        private SKSpriteNode collectedKeySprite;
        private SKNode overlayNode;

        // demo UI
        private Menu demoMenu;

        private int collectedGemsCount = 0;

        public Overlay(CGSize size, GameController controller) : base(size)
        {
            this.overlayNode = new SKNode();

            var width = size.Width;
            var height = size.Height;

            this.collectedGemsSprites = new List<SKSpriteNode>();

            // Setup the game overlays using SpriteKit.
            this.ScaleMode = SKSceneScaleMode.ResizeFill;

            this.AddChild(this.overlayNode);
            this.overlayNode.Position = new CGPoint(0f, height);

            // The Max icon.
            var characterNode = SKSpriteNode.FromImageNamed("Overlays/MaxIcon.png");
            var menuButton = new Button(characterNode)
            {
                Position = new CGPoint(50f, -50f),
                XScale = 0.5f,
                YScale = 0.5f,
            };

            this.overlayNode.AddChild(menuButton);
            menuButton.SetClickedTarget(this.ToggleMenu);

            // The Gems
            for (int i = 0; i < 1; i++)
            {
                var gemNode = SKSpriteNode.FromImageNamed("Overlays/collectableBIG_empty.png");
                gemNode.Position = new CGPoint(125f + i * 80f, -50f);
                gemNode.XScale = 0.25f;
                gemNode.YScale = 0.25f;

                this.overlayNode.AddChild(gemNode);
                this.collectedGemsSprites.Add(gemNode);
            }

            // The key
            this.collectedKeySprite = SKSpriteNode.FromImageNamed("Overlays/key_empty.png");
            this.collectedKeySprite.Position = new CGPoint(195f, -50f);
            this.collectedKeySprite.XScale = 0.4f;
            this.collectedKeySprite.YScale = 0.4f;
            this.overlayNode.AddChild(this.collectedKeySprite);

            // The virtual D-pad
#if __IOS__
            this.ControlOverlay = new ControlOverlay(new CGRect(0f, 0f, width, height));
            this.ControlOverlay.LeftPad.Delegate = controller;
            this.ControlOverlay.RightPad.Delegate = controller;
            this.ControlOverlay.ButtonA.Delegate = controller;
            this.ControlOverlay.ButtonB.Delegate = controller;
            this.AddChild(this.ControlOverlay);
#endif
            // the demo UI
            this.demoMenu = new Menu(size)
            {
                Hidden = true,
                Delegate = controller,
            };

            this.overlayNode.AddChild(this.demoMenu);

            // Assign the SpriteKit overlay to the SceneKit view.
            base.UserInteractionEnabled = false;
        }

        public Overlay(NSCoder coder)
        {
            throw new NotImplementedException("init(coder:) has not been implemented");
        }

#if __IOS__
        public ControlOverlay ControlOverlay { get; set; }
#endif

        public int CollectedGemsCount
        {
            get
            {
                return this.collectedGemsCount;
            }

            set
            {
                this.collectedGemsCount = value;

                this.collectedGemsSprites[this.collectedGemsCount - 1].Texture = SKTexture.FromImageNamed("Overlays/collectableBIG_full.png");
                this.collectedGemsSprites[this.collectedGemsCount - 1].RunAction(SKAction.Sequence(new SKAction[] { SKAction.WaitForDuration(0.5f),
                                                                                                                    SKAction.ScaleBy(1.5f, 0.2f),
                                                                                                                    SKAction.ScaleBy(1f / 1.5f, 0.2f) }));
            }
        }

        public void Layout2DOverlay()
        {
            this.overlayNode.Position = new CGPoint(0f, this.Size.Height);

            if (this.congratulationsGroupNode != null)
            {
                this.congratulationsGroupNode.Position = new CGPoint(this.Size.Width * 0.5f, this.Size.Height * 0.5f);
                this.congratulationsGroupNode.XScale = 1f;
                this.congratulationsGroupNode.YScale = 1f;
                var currentBbox = this.congratulationsGroupNode.CalculateAccumulatedFrame();

                var margin = 25f;
                var bounds = new CGRect(0f, 0f, this.Size.Width, this.Size.Height);
                var maximumAllowedBbox = bounds.Inset(margin, margin);

                var top = currentBbox.GetMaxY() - this.congratulationsGroupNode.Position.Y;
                var bottom = this.congratulationsGroupNode.Position.Y - currentBbox.GetMinY();
                var maxTopAllowed = maximumAllowedBbox.GetMaxY() - this.congratulationsGroupNode.Position.Y;
                var maxBottomAllowed = this.congratulationsGroupNode.Position.Y - maximumAllowedBbox.GetMinY();

                var left = this.congratulationsGroupNode.Position.X - currentBbox.GetMinX();
                var right = currentBbox.GetMaxX() - this.congratulationsGroupNode.Position.X;
                var maxLeftAllowed = this.congratulationsGroupNode.Position.X - maximumAllowedBbox.GetMinX();
                var maxRightAllowed = maximumAllowedBbox.GetMaxX() - this.congratulationsGroupNode.Position.X;

                var topScale = top > maxTopAllowed ? maxTopAllowed / top : 1;
                var bottomScale = bottom > maxBottomAllowed ? maxBottomAllowed / bottom : 1;
                var leftScale = left > maxLeftAllowed ? maxLeftAllowed / left : 1; ;
                var rightScale = right > maxRightAllowed ? maxRightAllowed / right : 1;

                var scale = NMath.Min(topScale, NMath.Min(bottomScale, NMath.Min(leftScale, rightScale)));

                this.congratulationsGroupNode.XScale = scale;
                this.congratulationsGroupNode.YScale = scale;
            }
        }

        public void DidCollectKey()
        {
            this.collectedKeySprite.Texture = SKTexture.FromImageNamed("Overlays/key_full.png");
            this.collectedKeySprite.RunAction(SKAction.Sequence(new SKAction[]{ SKAction.WaitForDuration(0.5f),
                                                                                SKAction.ScaleBy(1.5f, 0.2f),
                                                                                SKAction.ScaleBy(1f / 1.5f, 0.2f) }));
        }

#if __IOS__

        public void ShowVirtualPad()
        {
            this.ControlOverlay.Hidden = false;
        }

        public void HideVirtualPad()
        {
            this.ControlOverlay.Hidden = true;
        }

#endif

        #region Congratulate the player

        public void ShowEndScreen()
        {
            // Congratulation title
            var congratulationsNode = SKSpriteNode.FromImageNamed("Overlays/congratulations.png");

            // Max image
            var characterNode = SKSpriteNode.FromImageNamed("Overlays/congratulations_pandaMax.png");
            characterNode.Position = new CGPoint(0f, -220f);
            characterNode.AnchorPoint = new CGPoint(0.5f, 0f);

            this.congratulationsGroupNode = new SKNode();
            this.congratulationsGroupNode.AddChild(characterNode);
            this.congratulationsGroupNode.AddChild(congratulationsNode);
            this.AddChild(this.congratulationsGroupNode);

            // Layout the overlay
            this.Layout2DOverlay();

            // Animate
            congratulationsNode.Alpha = 0f;
            congratulationsNode.XScale = 0f;
            congratulationsNode.YScale = 0f;
            congratulationsNode.RunAction(SKAction.Group(new SKAction[] { SKAction.FadeInWithDuration(0.25),
                                                                          SKAction.Sequence(new SKAction[] { SKAction.ScaleTo(1.22f, 0.25),
                                                                                                             SKAction.ScaleTo(1f, 0.1) }) }));

            characterNode.Alpha = 0f;
            characterNode.XScale = 0f;
            characterNode.YScale = 0f;
            characterNode.RunAction(SKAction.Sequence(new SKAction[] { SKAction.WaitForDuration(0.5),
                                                                       SKAction.Group(new SKAction[] { SKAction.FadeInWithDuration(0.5),
                                                                                                       SKAction.Sequence(new SKAction[] { SKAction.ScaleTo(1.22f, 0.25),
                                                                                                                                          SKAction.ScaleTo(1f, 0.1) }) }) }));
        }

        #endregion

        private void ToggleMenu(Button sender)
        {
            this.demoMenu.Hidden = !this.demoMenu.Hidden;
        }
    }
}