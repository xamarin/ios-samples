using System;

namespace MetalBasic3D
{
	public interface IGameViewController
	{
		void RenderViewControllerUpdate (GameViewController gameViewController);

		void RenderViewController (GameViewController gameViewController, bool value);
	}

}
