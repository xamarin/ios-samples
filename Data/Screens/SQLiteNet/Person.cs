using System;
using SQLite;

namespace Xamarin.Screens.SQLiteNet
{
	/// <summary>
	/// A simple person class to illustrate SQLite-Net mapping
	/// </summary>
	public class Person
	{
		public Person () { }
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}

}

