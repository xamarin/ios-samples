
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;


namespace Xamarin.Screens.ADONET
{
	public partial class BasicOperations : UITableViewController
	{
		protected List<string> people = new List<string>();
		protected TableSource tableSource;
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code
		public BasicOperations (IntPtr handle) : base (handle) { Initialize (); }
		[Export("initWithCoder:")]
		public BasicOperations (NSCoder coder) : base (coder) { Initialize (); }
		public BasicOperations () : base ("DataSample", null) { Initialize (); }
		
		protected void Initialize ()
		{
			this.Title = "ADO.NET";
			
			// performance timing
			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start ();

			// create a connection to the database, if the db doesn't exist, it'll get created			
	        var connection = GetConnection ("db_adonet.db3");
			
			// performance timing
			Console.WriteLine("database creation: " + stopwatch.ElapsedMilliseconds.ToString());
			
			// create a command
			using (var cmd = connection.CreateCommand ())
			{
				// open the connection
				connection.Open ();
				// create a select statement
				cmd.CommandText = "SELECT * FROM People";
				using (var reader = cmd.ExecuteReader ()) {
					// loop through each record and add the name to our collection
					while (reader.Read ()) { 
						people.Add (reader[1] + " " + reader[2]);
					}
				}
				
				// performance timing
				Console.WriteLine("database query: " + stopwatch.ElapsedMilliseconds.ToString ());
				
				// close the connection
				connection.Close ();
			}
			
			// create a new table source from our people collection
			tableSource = new BasicOperations.TableSource (people);
			
			// initialize the table view and set the source
			TableView = new UITableView ();
			TableView.Source = tableSource;
		}
		
		#endregion
		
		// Creates a connection to a database. if the database doesn't exist, it 
		// creates it.
	    protected SqliteConnection GetConnection(string dbName)
	    {
			// declare vars
			bool needToCreate;
			
			// get a reference to the documents folder
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			
			// create the db path
			string db = Path.Combine (documents, dbName);
			
			// determine whether or not the database exists
			needToCreate = !File.Exists (db);
			
			// if there isn't a db there, create it, we can use the SqliteConnection object
			// to create a new blank database
			if (needToCreate) 
				SqliteConnection.CreateFile (db);
			
			// create a new connection object, from the path to the database
	        var conn = new SqliteConnection("Data Source=" + db);
			
			// now that we have a connnection to the database, let's actually create our table structure
	        if (needToCreate) 
				CreateDBSchema(conn);
			
	        return conn;
	    }
		
		/// <summary>
		/// Creates a People table and inserts some data
		/// </summary>
		protected void CreateDBSchema(SqliteConnection connection)
		{
			// create a an array of commands
			var commands = new[]
			{
				"CREATE TABLE People (PersonID INTEGER PRIMARY KEY AUTOINCREMENT, FirstName ntext, LastName ntext)",
				"INSERT INTO People (FirstName, LastName) VALUES ('Peter', 'Gabriel')",
				"INSERT INTO People (FirstName, LastName) VALUES ('Thom', 'Yorke')",
				"INSERT INTO People (FirstName, LastName) VALUES ('J', 'Spaceman')",
				"INSERT INTO People (FirstName, LastName) VALUES ('Benjamin', 'Gibbard')"
			};
			
			// execute each command, using standard ADO.NET calls
			foreach (var cmd in commands) {
				using (var c = connection.CreateCommand()) {
					c.CommandText = cmd;
					c.CommandType = CommandType.Text;
					connection.Open ();
					c.ExecuteNonQuery ();
					connection.Close ();
				}
			}
			
		}
		
		/// <summary>
		/// A simple data source for our table
		/// </summary>
		protected class TableSource : UITableViewSource
		{
			List<string> items;
			
			public TableSource(List<string> items) : base() { this.items = items; }
			
			public override nint NumberOfSections (UITableView tableView) { return 1; }
			public override nint RowsInSection (UITableView tableview, nint section) { return this.items.Count; }
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell;
				cell = tableView.DequeueReusableCell("item");
				if(cell == null) 
					cell = new UITableViewCell(UITableViewCellStyle.Default, "item");
				cell.TextLabel.Text = this.items[(int)indexPath.Row];
				return cell;
			}
			
		}
		
	}
}

