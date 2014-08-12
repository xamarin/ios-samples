using System;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace HelloGoodbye
{
	public class Person
	{
		private const string PhotoKey = "photo";
		private const string AgeKey = "age";
		private const string HobbiesKey = "hobbies";
		private const string ElevatorPitchKey = "elevatorPitch";

		public UIImage Photo { get; set; }
		public int Age { get; set; }
		public string Hobbies { get; set; }
		public string ElevatorPitch { get; set; }

		public static Person PersonFromDictionary(NSDictionary dict)
		{
			Person person = new Person {
				Photo = UIImage.FromBundle((string)(NSString)dict[PhotoKey]),
				Age = ((NSNumber)dict[AgeKey]).Int32Value,
				Hobbies = (string)(NSString)dict[HobbiesKey],
				ElevatorPitch = (string)(NSString)dict[ElevatorPitchKey],
			};

			return person;
		}
	}
}

