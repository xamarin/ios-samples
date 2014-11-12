using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using System.Data;
using System.IO;
using Vici.CoolStorage;

namespace Xamarin.Screens.ViciCoolStorage
{
	public partial class BasicOperations : UITableViewController
	{
		protected CSList<Person> people;
		protected TableSource tableSource;
		
		#region Constructors

		public BasicOperations (IntPtr handle) : base (handle) { Initialize (); }
		[Export("initWithCoder:")]
		public BasicOperations (NSCoder coder) : base (coder) { Initialize (); }
		public BasicOperations () : base ("DataSample", null) { Initialize (); }

		protected void Initialize ()
		{
			this.Title = "Vici Cool Storage";
			
			// performance timing
			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch ();
			stopwatch.Start ();
			
			string dbName = "db_viciCoolStorage.db3";
			
			// check the database, if it doesn't exist, create it
			this.CheckAndCreateDatabase (dbName);
			
			// performance timing
			Console.WriteLine ("database creation: " + stopwatch.ElapsedMilliseconds.ToString ());
			
			// query a list of people from the db			
			people = Person.List();
				
			// performance timing
			Console.WriteLine ("database query: " + stopwatch.ElapsedMilliseconds.ToString ());
		
			// create a new table source from our people collection
			tableSource = new BasicOperations.TableSource (people);
			
			// initialize the table view and set the source
			this.TableView = new UITableView (){
				Source = tableSource
			};
			
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
		// it and inserts some data. It also sets our database to be the default database
		// connection.
		protected void CheckAndCreateDatabase (string dbName)
		{	
			// determine whether or not the database exists
			bool dbExists = File.Exists (GetDBPath (dbName));
			
			// configure the current database, create if it doesn't exist, and then run the anonymous
			// delegate method after it's created
			CSConfig.SetDB (GetDBPath (dbName), SqliteOption.CreateIfNotExists, () => {
				CSDatabase.ExecuteNonQuery ("CREATE TABLE People (PersonID INTEGER PRIMARY KEY AUTOINCREMENT, FirstName text, LastName text)");
				
				// if the database had to be created, let's populate with initial data
				if (!dbExists) {
					// declare vars
					CSList<Person> people = new CSList<Person> ();
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
					
					// save the people collection to the database
					people.Save ();
				}
			});
		}

		// A simple data source for our table
		protected class TableSource : UITableViewSource
		{
			CSList<Person> items;
			
			public TableSource (CSList<Person> items) : base() { this.items = items; }
			
			public override nint NumberOfSections (UITableView tableView) { return 1; }
			
			public override nint RowsInSection (UITableView tableview, nint section) { return this.items.Count; }
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell;
				cell = tableView.DequeueReusableCell ("item");
				if (cell == null)
					cell = new UITableViewCell (UITableViewCellStyle.Default, "item");
				cell.TextLabel.Text = this.items[indexPath.Row].FirstName + " " + this.items[indexPath.Row].LastName;
				return cell;
			}
			
		}
		
	}
}

