using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmeika
{
	class DeathScreen : IUpdate, Drawable
	{
		public bool Show;
		public Sprite Sprite;
		public Timer Timer;

		public DeathScreen(string spritePath)
		{
			Show = false;

			Sprite = new Sprite(new Texture(spritePath));
			Sprite.Scale = new Vector2f(
					Program.renderWindow.Size.X / (float)Sprite.TextureRect.Width,
					Program.renderWindow.Size.Y / (float)Sprite.TextureRect.Height);
			Sprite.Color = new Color(0, 0, 0, 0);

			Timer = new Timer(Time.FromSeconds(0.03f));
			Timer.Tick += () => Sprite.Color =
				new Color(255, 255, 255, (byte)Math.Min(Sprite.Color.A + 2, byte.MaxValue));
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			target.Draw(Sprite, states);
		}

		public void Update(float dt)
		{
			Timer.Update(dt);
		}
	}
}
