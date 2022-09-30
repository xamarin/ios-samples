namespace XamarinShot.Utils;

public static class ARWorldMapExtensions
{
	public static BoardAnchor? BoardAnchor (this ARWorldMap self)
	{
		return self.Anchors.FirstOrDefault (anchor => anchor is BoardAnchor) as BoardAnchor;
	}

	public static List<KeyPositionAnchor> KeyPositionAnchors (this ARWorldMap self)
	{
		return self.Anchors.Where (anchor => anchor is KeyPositionAnchor).Cast<KeyPositionAnchor> ().ToList ();
	}
}
