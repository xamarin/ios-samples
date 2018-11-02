
namespace XamarinShot.Utils
{
    using ARKit;
    using System.Collections.Generic;
    using System.Linq;
    using XamarinShot.Models;

    public static class ARWorldMapExtensions
    {
        public static BoardAnchor BoardAnchor(this ARWorldMap self)
        {
            return self.Anchors.FirstOrDefault(anchor => anchor is BoardAnchor) as BoardAnchor;
        }

        public static List<KeyPositionAnchor> KeyPositionAnchors(this ARWorldMap self)
        {
            return self.Anchors.Where(anchor => anchor is KeyPositionAnchor).Cast<KeyPositionAnchor>().ToList();
        }
    }
}