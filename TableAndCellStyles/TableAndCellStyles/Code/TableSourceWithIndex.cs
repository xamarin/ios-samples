using System;
using System.Collections.Generic;

namespace Example_TableAndCellStyles.Code
{
	/// <summary>
	/// A sample table source implementation that supports a table index
	/// </summary>
	public class TableSourceWithIndex : TableSource
	{
		Dictionary<string, int> indexSectionMap = null;
		
		/// <summary>
		/// 
		/// </summary>
		public TableSourceWithIndex (List<TableItemGroup> items, Dictionary<string, int> indexSectionMap)
		{
			tableItems = items;
			this.indexSectionMap = indexSectionMap;
		}
		
		/// <summary>
		/// called by the table view to get a list of the index section titles
		/// </summary>
		public override string[] SectionIndexTitles (MonoTouch.UIKit.UITableView tableView)
		{
			return new List<string> (indexSectionMap.Keys).ToArray ();
		}
	
		/// <summary>
		/// called by the table view when a user clicks on an index section. used to retrieve the 
		/// appropriate section in the table for that particular index.
		/// </summary>
		public override int SectionFor (MonoTouch.UIKit.UITableView tableView, string title, int atIndex)
		{
			return indexSectionMap[title];
		}
	}
}

