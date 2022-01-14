
namespace VisionObjectTrack;
public partial class AssetsCell : UICollectionViewCell
{
	public const string Identifier = "AssetCell";

	public string RepresentedAssetIdentifier { get; set; } = string.Empty;

	public AssetsCell (IntPtr handle) : base (handle) { }
}
