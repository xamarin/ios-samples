using System;
using Vici.CoolStorage;

namespace Xamarin.Screens.ViciCoolStorage
{
	/// <summary>
	/// A simple person class to illustrate Vici.CoolStorage mapping
	/// </summary>
	[MapTo("People")]
	public class Person : CSObject<Person, int>
	{
		public Person () { }

		public int ID { get { return (int)GetField ("PersonID"); } }
		public string FirstName { get { return (string)GetField ("FirstName"); } set { SetField ("FirstName",value); } }
		public string LastName { get { return (string)GetField ("LastName"); } set { SetField ("LastName",value); } }
	}

}

