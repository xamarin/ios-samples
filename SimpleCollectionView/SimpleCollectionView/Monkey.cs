using System;
using MonoTouch.UIKit;

namespace SimpleCollectionView
{
	public class Monkey : IAnimal
	{
		public Monkey ()
		{
		}

		public string Name {
			get{
				return "Monkey";
			}
		}

		public UIImage Image{
			get{
				return UIImage.FromBundle("monkey.png");
			}
		}

	}
}

