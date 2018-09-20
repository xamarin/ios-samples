
namespace ARKitVision
{
    using Foundation;
    using SpriteKit;
    using System;

    /// <summary>
    /// Instantiates styled label nodes based on a template node in a scene file.
    /// </summary>
    public class TemplateLabelNode : SKReferenceNode
    {
        private readonly string text;

        private bool isUpdated;

        private SKNode localParent;

        public TemplateLabelNode(string text) : base("LabelScene")
        {
            this.text = text;
            this.UpdateText();
        }

        public TemplateLabelNode(NSCoder coder) : base(coder) => throw new Exception("init(coder:) has not been implemented");

        public override void DidLoadReferenceNode(SKNode node)
        {
            base.DidLoadReferenceNode(node);

            // Apply text to both labels loaded from the template.
            this.localParent = node?.GetChildNode("LabelNode");
            if(this.localParent == null)
            {
                throw new Exception("misconfigured SpriteKit template file");
            }

            this.UpdateText();
        }

        private void UpdateText()
        {
            if (!this.isUpdated)
            {
                this.isUpdated = !string.IsNullOrEmpty(this.text);
                foreach (SKLabelNode label in this.localParent.Children)
                {
                    label.Name = this.text;
                    label.Text = this.text;
                }
            }
        }
    }
}