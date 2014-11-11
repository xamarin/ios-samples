using System;
using UIKit;

namespace SimpleCollectionView
{
	public interface IAnimal
	{
		string Name { get; }
		UIImage Image { get; }
	}

	public class Monkey : IAnimal
	{
		public string Name { get { return "Monkey"; } }
		public UIImage Image { get { return UIImage.FromBundle ("monkey.png"); } }
	}

	public class Tamarin : IAnimal {
		public string Name { get { return "Tamarin"; }	}
		public UIImage Image { get { return UIImage.FromBundle ("placeholder.png"); } }
	}
}
