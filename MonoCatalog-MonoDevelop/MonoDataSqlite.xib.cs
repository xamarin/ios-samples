
using System;
using System.Collections.Generic;
using CoreGraphics;
using System.IO;
using Mono.Data.Sqlite;
using Foundation;
using UIKit;

namespace MonoCatalog
{
	public partial class MonoDataSqliteController : UITableViewController
	{
		// The IntPtr and NSCoder constructors are required for controllers that need
		// to be able to be created from a xib rather than from managed code

		public MonoDataSqliteController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public MonoDataSqliteController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public MonoDataSqliteController () : base ("MonoDataSqlite", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}

		class ItemsTableDelegate : UITableViewDelegate
		{
			//
			// Override to provide the sizing of the rows in our table
			//
			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				if (indexPath.Section == 0)
					return 70f;
				return 50f;
			}
		}

		class ItemsDataSource : UITableViewDataSource
		{
			static readonly NSString kAdd = new NSString ("Add");
			static readonly NSString kKey = new NSString ("Key");

			const int kKeyTag = 1;
			const int kValueTag = 2;
			const int kAddTag = 3;

			class SectionInfo {
				public string Title;
				public Func<UITableView, NSIndexPath, UITableViewCell> Creator;
			}

			SectionInfo [] Sections = new[]{
				new SectionInfo { Title = "Add Key/Value Pair", Creator = GetAddKeyValuePairCell },
				new SectionInfo { Title = "Key/Value Pairs",    Creator = GetKeyValuePairCell },
			};

			protected override void Dispose (bool disposing)
			{
				foreach (var cell in cells)
					cell.Dispose ();
				cells = null;
				base.Dispose (disposing);
			}

			public override nint NumberOfSections (UITableView tableView)
			{
				return Sections.Length;
			}

			public override string TitleForHeader (UITableView tableView, nint section)
			{
				return Sections [section].Title;
			}

			// keep a managed reference to the `cell` otherwise the GC can collect it and events
			// like TouchUpInside will crash the application (needs to be static for SectionInfo
			// initialization)
			static List<UITableViewCell> cells = new List<UITableViewCell> ();

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				return Sections [indexPath.Section].Creator (tableView, indexPath);
			}

			static UITableViewCell GetAddKeyValuePairCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell (kAdd);
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Default, kAdd);
					cells.Add (cell);
				} else {
					RemoveViewWithTag (cell, kKeyTag   << 1);
					RemoveViewWithTag (cell, kKeyTag);
					RemoveViewWithTag (cell, kValueTag << 1);
					RemoveViewWithTag (cell, kValueTag);
					RemoveViewWithTag (cell, kAddTag);
				}
				var lblKey = new UILabel () {
					BaselineAdjustment = UIBaselineAdjustment.AlignCenters,
					Frame              = new CGRect (10f, 0f, 70f, 31f),
					Tag                = kKeyTag << 1,
					Text               = "Key: ",
					TextAlignment      = UITextAlignment.Right,
				};
				var key = new UITextField () {
					BorderStyle        = UITextBorderStyle.Bezel,
					ClearButtonMode    = UITextFieldViewMode.WhileEditing,
					Frame              = new CGRect (80f, 1f, 170f, 31f),
					Placeholder        = "Key",
					Tag                = kKeyTag,
					AccessibilityLabel = "Key"
				};
				var lblValue = new UILabel () {
					BaselineAdjustment = UIBaselineAdjustment.AlignCenters,
					Frame              = new CGRect (10f, 37f, 70f, 31f),
					Tag                = kValueTag << 1,
					Text               = "Value: ",
					TextAlignment      = UITextAlignment.Right,
				};
				var value = new UITextField () {
					BorderStyle        = UITextBorderStyle.Bezel,
					ClearButtonMode    = UITextFieldViewMode.WhileEditing,
					Frame              = new CGRect (80f, 38f, 170f, 31f),
					Placeholder        = "Value",
					Tag                = kValueTag,
					AccessibilityLabel = "Value"
				};
				var add = UIButton.FromType (UIButtonType.ContactAdd);
				add.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
				add.VerticalAlignment   = UIControlContentVerticalAlignment.Center;
				add.Frame               = new CGRect (255, 0, 40f, 70f);
				add.SetTitle ("Add", UIControlState.Normal);
				add.TouchUpInside += (o, e) => {
					WithCommand (c => {
						c.CommandText = "INSERT INTO [Items] ([Key], [Value]) VALUES (@key, @value)";
						c.Parameters.Add (new SqliteParameter ("@key", key.Text));
						c.Parameters.Add (new SqliteParameter ("@value", value.Text));
						c.ExecuteNonQuery ();
						key.Text      = "";
						value.Text    = "";
						key.ResignFirstResponder ();
						value.ResignFirstResponder ();
						var path = NSIndexPath.FromRowSection (GetItemCount () - 1, 1);
						tableView.InsertRows (new NSIndexPath [] {path}, UITableViewRowAnimation.Bottom);
					});
				};
				cell.ContentView.AddSubview (lblKey);
				cell.ContentView.AddSubview (key);
				cell.ContentView.AddSubview (lblValue);
				cell.ContentView.AddSubview (value);
				cell.ContentView.AddSubview (add);

				return cell;
			}

			static void RemoveViewWithTag (UITableViewCell cell, int tag)
			{
				var u = cell.ContentView.ViewWithTag (tag);
				Console.Error.WriteLine ("# Removing view: {0}", u);
				if (u != null)
					u.RemoveFromSuperview ();
			}

			static UITableViewCell GetKeyValuePairCell (UITableView tableView, NSIndexPath indexPath)
			{
				string query = string.Format ("SELECT [Key], [Value] FROM [Items] LIMIT {0},1", indexPath.Row);
				string key = null, value = null;
				WithCommand (c => {
					c.CommandText = query;
					var r = c.ExecuteReader ();
					while (r.Read ()) {
						key   = r ["Key"].ToString ();
						value = r ["Value"].ToString ();
					}
				});
				var cell = tableView.DequeueReusableCell (kKey);
				if (cell == null){
					cell = new UITableViewCell (UITableViewCellStyle.Default, kKey);
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
					cells.Add (cell);
				}
				else {
					RemoveViewWithTag (cell, kKeyTag);
					RemoveViewWithTag (cell, kValueTag);
				}
				var width = tableView.Bounds.Width / 2;
				Func<string, int, bool, UILabel> createLabel = (v, t, left) => {
					var label = new UILabel ();
					label.Frame = left
						? new CGRect (10f, 1f, width-10, 40)
						: new CGRect (width, 1f, width-30, 40);
					label.Text = v;
					label.TextAlignment = left
						? UITextAlignment.Left
						: UITextAlignment.Right;
					label.Tag = t;
					return label;
				};
				var f = cell.TextLabel.Frame;
				cell.ContentView.AddSubview (createLabel (key, kKeyTag, true));
				cell.ContentView.AddSubview (createLabel (value, kValueTag, false));
				return cell;
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				if (section == 0)
					return 1;
				return GetItemCount ();
			}
		}

		static int GetItemCount ()
		{
			int count = 0;
			WithCommand (c => {
				c.CommandText = "SELECT COUNT(*) FROM [Items]";
				var r = c.ExecuteReader ();
				while (r.Read ()) {
					count = (int) (long) r [0];
				}
			});
			return count;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.DataSource = new ItemsDataSource ();
			TableView.Delegate = new ItemsTableDelegate ();
		}

		static SqliteConnection GetConnection ()
		{
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string db = Path.Combine (documents, "items.db3");
			bool exists = File.Exists (db);
			if (!exists)
				SqliteConnection.CreateFile (db);
			var conn = new SqliteConnection("Data Source=" + db);
			if (!exists) {
				var commands = new[] {
					"CREATE TABLE Items (Key ntext, Value ntext)",
					"INSERT INTO [Items] ([Key], [Value]) VALUES ('sample', 'text')",
				};
				foreach (var cmd in commands)
					WithCommand (c => {
						c.CommandText = cmd;
						c.ExecuteNonQuery ();
					});
			}
			return conn;
		}

		static void WithConnection (Action<SqliteConnection> action)
		{
			var connection = GetConnection ();
			try {
				connection.Open ();
				action (connection);
			}
			finally {
				connection.Close ();
			}
		}

		static void WithCommand (Action<SqliteCommand> command)
		{
			WithConnection (conn => {
				using (var cmd = conn.CreateCommand ())
					command (cmd);
			});
		}
	}
}
