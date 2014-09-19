using System;

namespace MetalBasic3D
{
	public interface IGameView
	{
		void Reshape (GameView view);

		void Render (GameView view);
	}
}

