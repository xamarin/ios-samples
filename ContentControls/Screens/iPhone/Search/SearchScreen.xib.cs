using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.IO;

namespace Example_ContentControls.Screens.iPhone.Search
{
	public partial class SearchScreen : UIViewController
	{
		/// <summary>
		/// the dictionary that will hold our words
		/// </summary>
		List<string> dictionary = null;
		
		/// <summary>
		/// The table source that will hold our matches
		/// </summary>
		WordsTableSource tableSource = null;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public SearchScreen (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public SearchScreen (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public SearchScreen () : base("SearchScreen", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			NavigationItem.Title = "Search Bar Example";
			
			// load our dictonary words
			LoadWords();
			
			// create our table source and bind it to the table
			tableSource = new WordsTableSource();
			tblMain.Source = tableSource;
			
			// wire up the search button clicked handler to hide the keyboard
			srchMain.SearchButtonClicked += (s, e) => { srchMain.ResignFirstResponder(); };
			
			// refine the search results every time the text changes
			srchMain.TextChanged += (s, e) => { RefineSearchItems(); };
		}
		
		/// <summary>
		/// This loads our dictionary of words into our _dictionary object.
		/// </summary>
		protected void LoadWords()
		{
			dictionary = File.ReadAllLines("Content/WordList.txt").ToList();
		}

		/// <summary>
		/// is called when the text in the search box text changes. i use some simple
		/// LINQ to refine the word list in our table source
		/// </summary>
		protected void RefineSearchItems()
		{
			// select our words
			tableSource.Words = dictionary
				.Where(w => w.Contains(srchMain.Text)).ToList();
			
			// refresh the table
			tblMain.ReloadData();
			
		}
		
		/// <summary>
		/// A simple table source that displays a list of strings
		/// </summary>
		protected class WordsTableSource : UITableViewSource
		{
			protected string cellIdentifier = "wordsCell";
			
			/// <summary>
			/// The words to display in the table
			/// </summary>
			public List<string> Words
			{
				get { return words; }
				set { words = value; }
			}
			protected List<string> words = new List<string>();
			
			/// <summary>
			/// called by the table to determine how many rows to create, in our case, it's the number 
			/// of words.
			/// </summary>
			public override int RowsInSection (UITableView tableview, int section)
			{
				return words.Count;
			}
			
			/// <summary>
			/// called by the table to determine how many sections to create, in this case, we just have one
			/// </summary>
			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}
			
			/// <summary>
			/// called by the table to generate the cell to display. in this case, it's very simple
			/// and just displays the word.
			/// </summary>
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				// declare vars
				UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
				string word = words[indexPath.Row];
				
				// if there are no cells to reuse, create a new one
				if (cell == null)
					cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
				
				// set the item text
				cell.TextLabel.Text = word;

				return cell;
			}
		}
		
	}
}

