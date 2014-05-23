using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using System.Data;
using System.IO;
using SQLite;

namespace Xamarin.Screens.SQLiteNet
{
	public partial class BasicOperations : UITableViewController
	{
		protected List<Person> people = new List<Person> ();
		protected TableSource tableSource;
		
		#region Constructors

		public BasicOperations (IntPtr handle) : base(handle) { Initialize (); }
		[Export("initWithCoder:")]
		public BasicOperations (NSCoder coder) : base(coder) { Initialize (); }
		public BasicOperations () : base("DataSample", null) { Initialize (); }

		
		protected void Initialize ()
		{
			this.Title = "SQLite .NET";
			
			// performance timing
			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch ();
			stopwatch.Start ();
			
			string dbName = "db_sqlite-net.db3";
			
			// check the database, if it doesn't exist, create it
			CheckAndCreateDatabase (dbName);
			
			// performance timing
			Console.WriteLine ("database creation: " + stopwatch.ElapsedMilliseconds.ToString ());
			
			// create a connection to the database
			using (SQLiteConnection db = new SQLiteConnection (GetDBPath (dbName))) {
				// query a list of people from the db
				people = new List<Person> (from p in db.Table<Person> () select p);
				
				// performance timing
				Console.WriteLine ("database query: " + stopwatch.ElapsedMilliseconds.ToString ());
			
				// create a new table source from our people collection
				tableSource = new BasicOperations.TableSource (people);
				
				// initialize the table view and set the source
				TableView = new UITableView () {
					Source = tableSource
				};
			}
		}
		
		#endregion
		
		protected string GetDBPath (string dbName)
		{
			// get a reference to the documents folder
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			
			// create the db path
			string db = Path.Combine (documents, dbName);
			
			return db;
		}
		
		// This method checks to see if the database exists, and if it doesn't, it creates
		// it and inserts some data
		protected void CheckAndCreateDatabase (string dbName)
		{
			// create a connection object. if the database doesn't exist, it will create 
			// a blank database
			using(SQLiteConnection db = new SQLiteConnection (GetDBPath (dbName)))
			{				
				// create the tables
				db.CreateTable<Person> ();
				
				// skip inserting data if it already exists
				if(db.Table<Person>().Count() > 0)
					return;
					
				// declare vars
				List<Person> people = new List<Person> ();
				Person person;
				
				// create a list of people that we're going to insert
				person = new Person () { FirstName = "Peter", LastName = "Gabriel" };
				people.Add (person);
				person = new Person () { FirstName = "Thom", LastName = "Yorke" };
				people.Add (person);
				person = new Person () { FirstName = "J", LastName = "Spaceman" };
				people.Add (person);
				person = new Person () { FirstName = "Benjamin", LastName = "Gibbard" };
				people.Add (person);
				
				// insert our people
				db.InsertAll (people);
				
				// close the connection
				db.Close ();
			}

		}
		
		// A simple data source for our table
		protected class TableSource : UITableViewSource
		{
			List<Person> items;
			
			public TableSource (List<Person> items) : base() { this.items = items; }
			
			public override nint NumberOfSections (UITableView tableView) { return 1; }
			public override nint RowsInSection (UITableView tableview, nint section) { return this.items.Count; }
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell;
				cell = tableView.DequeueReusableCell ("item");
				if(cell == null) 
					cell = new UITableViewCell(UITableViewCellStyle.Default, "item");
				cell.TextLabel.Text = this.items[(int)indexPath.Row].FirstName + " " + this.items[(int)indexPath.Row].LastName;
				return cell;
			}
			
		}
		
	}
}

