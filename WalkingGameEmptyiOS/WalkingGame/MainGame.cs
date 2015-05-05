using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WalkingGame
{
	public class MainGame : Game
	{
		GraphicsDeviceManager graphics;
		public MainGame()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.IsFullScreen = true;

			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			base.Initialize();
		} 


		protected override void LoadContent()
		{
			
		} 


		protected override void Update(GameTime gameTime)
		{ 
			base.Update(gameTime);
		} 

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			base.Draw(gameTime);
		} 
	}
}

