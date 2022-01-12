
namespace ARKitVision;

/// <summary>
/// Instantiates styled label nodes based on a template node in a scene file.
/// </summary>
public class TemplateLabelNode : SKReferenceNode
{
        private readonly string text;

        private bool isUpdated;

        private SKNode? localParent;

        public TemplateLabelNode (string text) : base ("LabelScene")
        {
                this.text = text;
                UpdateText ();
        }

        public TemplateLabelNode (NSCoder coder) : base (coder) => throw new Exception ("init(coder:) has not been implemented");

        public override void DidLoadReferenceNode (SKNode? node)
        {
                base.DidLoadReferenceNode (node);

                // Apply text to both labels loaded from the template.
                localParent = node?.GetChildNode ("LabelNode");
                if (localParent is null)
                {
                        throw new Exception ("misconfigured SpriteKit template file");
                }

                this.UpdateText ();
        }

        private void UpdateText ()
        {
                if (!isUpdated)
                {
                        isUpdated = !string.IsNullOrEmpty (text);
                        if (localParent is not null)
                        {
                                foreach (SKLabelNode label in localParent.Children)
                                {
                                        label.Name = text;
                                        label.Text = text;
                                }
                        }
                }
        }
}
