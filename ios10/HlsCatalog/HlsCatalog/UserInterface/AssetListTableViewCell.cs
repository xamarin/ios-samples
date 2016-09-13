using System;

using Foundation;
using UIKit;

namespace HlsCatalog
{
	public interface IAssetListTableViewCellDelegate
	{
		void DownloadStateDidChange (AssetListTableViewCell cell, DownloadState newState);
	}

	public partial class AssetListTableViewCell : UITableViewCell
	{
		// TODO: port
		public static readonly string CellReusableId = "";
		public Asset Asset { get; set; }
		public IAssetListTableViewCellDelegate Delegate { get; set; }
		 
		public AssetListTableViewCell (IntPtr handle)
			: base (handle)
		{
		}
	}
}
