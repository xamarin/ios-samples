using System;
using MonoTouch.UIKit;

namespace SimpleCollectionView
{
	public interface IAnimal
	{
		string Name { get; }

		UIImage Image { get; }
	}
}