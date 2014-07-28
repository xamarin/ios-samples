using System;
using UIKit;

namespace SimpleCollectionView
{
	public interface IAnimal
	{
		string Name { get; }

		UIImage Image { get; }
	}
}