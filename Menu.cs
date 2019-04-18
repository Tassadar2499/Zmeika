using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmeika
{
	class Menu : IUpdate, Drawable
	{
		private Font _font;
		private List<(Text Text, Action Func)> _items;
		public bool Show;

		public Menu(IEnumerable<(string Str, Action Func)> items, uint charSize, string fontPath)
		{
			Show = true;
			_font = new Font(fontPath);
			_items = new List<(Text Text, Action Func)>();

			var count = 0;
			foreach (var item in items) 
			{
				var text = new Text(item.Str, _font, charSize);
				var textRect = text.GetGlobalBounds();
				text.Position = new Vector2f(
					Program.renderWindow.Size.X / 2f - textRect.Width / 2f,
					100 + count * textRect.Height * 1.5f
				);

				_items.Add((text, item.Func));
				count++;
			}
		}

		public void OnMouseClick(object sender, MouseButtonEventArgs e)
		{
			if (e.Button != Mouse.Button.Left || !Show)
				return;

			foreach (var item in _items)
			{
				var mousePos = Utils.GetMousePosition();
				if (item.Text.GetGlobalBounds().Contains(e.X, e.Y))
					item.Func();
			}
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			foreach (var item in _items)
				target.Draw(item.Text, states);
		}

		public void Update(float dt)
		{
			foreach(var item in _items)
			{
				var mousePos = Utils.GetMousePosition();
				if (item.Text.GetGlobalBounds().Contains(mousePos.X, mousePos.Y))
					item.Text.Color = new Color(255, 150, 150);
				else
					item.Text.Color = new Color(255, 240, 240);
			}
		}
	}
}
