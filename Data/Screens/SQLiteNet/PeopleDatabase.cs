using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace Xamarin.Screens.SQLiteNet
{
	// If you're looking for a more encapsulated way to work with SQLite-Net,
	// i recommend creating a database object that inherits from SQLiteConnection
	// and handles it's own creation, as well as data access methods.
	public class PeopleDatabase : SQLiteConnection
	{

		// Creates a new PeopelDatabase. if the database doesn't exist, it will
		// create the database and all the tables
		public PeopleDatabase (string path) : base (path)
		{
			// create the tables
			CreateTable<Person> ();
		}

		public IEnumerable<Person> GetPeople ()
		{
			return (from i in this.Table<Person> () select i);
		}

		public Person GetPerson (int id)
		{
			return (from i in Table<Person> ()
				where i.ID == id
				select i).FirstOrDefault ();
		}

		public int AddPerson (Person item)
		{
			return Insert (item);
		}
	}
}

