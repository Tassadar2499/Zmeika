using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmeika
{
	class GameMap
	{
		public static List<Snake> snakes;
		public static List<RectangleShape> foods;

		private void CreateFood()
		{
			if (foods.Count < 5)
			{
				var indexX = Program.randomizer.Next(0, (int)(Program.renderWindow.Size.X / (Program.SizeOfRectangle.X + Program.RANGE_BETWEEN_BLOCKS * 2)));
				var indexY = Program.randomizer.Next(0, (int)(Program.renderWindow.Size.Y / (Program.SizeOfRectangle.Y + Program.RANGE_BETWEEN_BLOCKS * 2)));
				var food = new RectangleShape(Program.SizeOfRectangle);
				food.Position = Utils.GetPositionFromIndexes(indexX, indexY);
				food.FillColor = Color.Red;
				if (IsFreePosition(food.Position))
					foods.Add(food);
			}
		}

		private bool IsFreePosition(Vector2f position)
		{
			if (IsFoodPosition(position))
				return false;
			foreach (var snake in snakes)
				foreach (var bodyPart in snake.Body)
					if (position.Equals(bodyPart.Position))
						return false;
			return true;
		}

		public bool IsFoodPosition(Vector2f position)
		{
			foreach (var food in foods)
				if (position.Equals(food.Position))
					return true;
			return false;
		}

		public RectangleShape GetFoodFromPosition(Vector2f position)
		{
			foreach (var food in foods)
				if (position.Equals(food.Position))
					return food;
			return null;
		}
	}
}
