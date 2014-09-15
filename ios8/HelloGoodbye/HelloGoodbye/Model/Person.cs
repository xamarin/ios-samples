using System;

using UIKit;
using Foundation;

namespace HelloGoodbye
{
	public class Person
	{
		const string PhotoKey = "photo";
		const string AgeKey = "age";
		const string HobbiesKey = "hobbies";
		const string ElevatorPitchKey = "elevatorPitch";

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

