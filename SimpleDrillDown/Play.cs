// 
// Play.cs
//  
// Author:
//       Alan McGovern <alan@xamarin.com>
// 
// Copyright 2011, Xamarin Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;

namespace SimpleDrillDown {

	public class Play {
		public List<string> Characters { get; set; }
		public DateTime Date { get; set; }
		public string Genre { get; set; }
		public string Title { get; set; }
		
		public static List <Play> CreateDemoPlays ()
		{
			var plays = new List<Play> ();
			plays.Add (new Play {
				Title = "Julius Caesar",
				Genre = "Tragedy",
				Date = new DateTime (1599, 1, 1),
				Characters = new List<string> { "Antony", "Artemidorus", "Brutus", "Caesar", "Calpurnia", "Casca", "Cassius", "Cicero", "Cinna", "Cinna the Poet", "Citizens", "Claudius", "Clitus", "Dardanius", "Decius Brutus", "First Citizen", "First Commoner", "First Soldier", "Flavius", "Fourth Citizen", "Lepidus", "Ligarius", "Lucilius", "Lucius", "Marullus", "Messala", "Messenger", "Metellus Cimber", "Octavius", "Pindarus", "Poet", "Popilius", "Portia", "Publius", "Second Citizen", "Second Commoner", "Second Soldier", "Servant", "Soothsayer", "Strato", "Third Citizen", "Third Soldier", "Tintinius", "Trebonius", "Varro", "Volumnius", "Young Cato" },
			});

			plays.Add (new Play {
				Title = "King Lear",
				Genre = "Tragedy",
				Date = new DateTime (1605, 1, 1),
				Characters = new List<string>  { "Captain", "Cordelia", "Curan", "Doctor", "Duke of Albany", "Duke of Burgundy", "Duke of Cornwall", "Earl of Gloucester", "Earl of Kent", "Edgar", "Edmund", "Fool", "Gentleman", "Goneril", "Herald", "King of France", "Knight", "Lear", "Messenger", "Old Man", "Oswald", "Regan", "Servant 1", "Servant 2", "Servant 3" }
			});

			plays.Add (new Play {
				Title = "Othello",
				Genre = "Tragedy",
				Date = new DateTime (1604, 1, 1),
				Characters = new List<string> { "Bianca", "Brabantio", "Cassio", "Clown", "Desdemona", "Duke of Venice", "Emilia", "First Gentleman", "First Musician", "First Officer", "First Senator", "Fourth Gentleman", "Gentleman", "Gratiano", "Herald", "Iago", "Lodovico, Kinsman to Brabantio", "Messenger", "Montano", "Othello", "Roderigo", "Sailor", "Second Gentleman", "Second Senator", "Third Gentleman" }
			});

			plays.Add (new Play {
				Title = "Henry IV, Pt 1",
				Genre = "History",
				Date = new DateTime (1597, 1, 1),
				Characters = new List<string> { "Archbishop Scroop", "Blunt", "Carrier", "Chamberlain", "Earl of Douglas", "Earl of Northumberland", "Earl of Westmoreland", "Earl of Worcester", "Edward Poins", "Falstaff", "First Carrier", "First Traveller", "Francis", "Gadshill", "Glendower", "Henry IV", "Henry V", "Hostess Quickly", "Hotspur (Henry Percy)", "Lady Percy", "Lord Bardolph", "Messenger", "Mortimer", "Ostler", "Peto", "Prince John, of Lancaster", "Second Carrier", "Servant", "Sheriff", "Sir Michael", "Vernon", "Vintner" }
			});
			
			plays.Add (new Play {
				Title = "The Tempest",
				Genre = "Comedy",
				Date = new DateTime (1611, 1, 1),
				Characters = new List<string> { "Adrian", "Alonso", "Antonio", "Ariel", "Boatswain", "Caliban", "Ceres", "Ferdinand", "Francisco", "Gonzalo", "Iris", "Juno", "Master", "Miranda", "Prospero", "Sebastian", "Stephano", "Trinculo" }
			});
			
			return plays;
		}
	}
}
