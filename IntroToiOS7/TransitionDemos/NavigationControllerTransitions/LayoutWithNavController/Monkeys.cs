using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Foundation;

namespace LayoutWithNavController
{
	// Loads a collection of monkeys containing file paths of monkey images
	public class Monkeys : List<Monkeys.Monkey>
	{
		static readonly Monkeys monkeys = new Monkeys ();

		Monkeys ()
		{
			Regex pattern = new Regex (@"^.*\.(jpg|jpeg|png)$", RegexOptions.IgnoreCase);
			string path = Path.Combine (NSBundle.MainBundle.BundlePath, "Images");

			Directory.GetFiles (path).Where (f => pattern.IsMatch (f)).ToList ().ForEach (p => {
				Monkey s = new Monkey{ImageFile = "Images/" + Path.GetFileName(p)};
				this.Add (s);
			});
		}

		public static Monkeys Instance {
			get{
				return monkeys;
			}
		}

		public class Monkey
		{
			public string ImageFile { get; set; }
		}
	}
}