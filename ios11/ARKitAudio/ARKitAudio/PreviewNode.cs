
namespace ARKitAudio
{
    using ARKit;
    using Foundation;
    using SceneKit;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// SceneKit node wrapper that estimates an object's final placement
    /// </summary>
    public class PreviewNode : SCNNode
    {
        // Use average of recent positions to avoid jitter.
        private readonly List<SCNVector3> recentPreviewNodePositions = new List<SCNVector3>();

        // Saved positions that help smooth the movement of the preview
        private SCNVector3 lastPositionOnPlane;

        // Saved positions that help smooth the movement of the preview
        private SCNVector3 lastPosition;

        public PreviewNode(IntPtr handle) : base(handle) { }

        public PreviewNode(NSCoder coder)
        {
            throw new NotImplementedException("init(coder:) has not been implemented");
        }

        public PreviewNode(SCNNode node) : base()
        {
            this.Opacity = 0.5f;
            this.AddChildNode(node);
        }

        // Appearance

        public void Update(SCNVector3 position, ARPlaneAnchor planeAnchor, ARCamera camera)
        {
            this.lastPosition = position;

            if (planeAnchor != null)
            {
                this.lastPositionOnPlane = position;
            }

            this.UpdateTransform(position, camera);
        }

        private void UpdateTransform(SCNVector3 position, ARCamera camera)
        {
            // Add to the list of recent positions.
            this.recentPreviewNodePositions.Add(position);

            // Remove anything older than the last 8 positions.
            this.recentPreviewNodePositions.KeepLast(8);

            // Move to average of recent positions to avoid jitter.
            var average = this.recentPreviewNodePositions.GetAverage();
            if (average.HasValue)
            {
                this.Position = average.Value;
            }
        }
    }
}