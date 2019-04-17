﻿using SFML.Graphics;
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
		public List<Snake> Snakes { get; set; }
		public List<RectangleShape> Foods { get; set; }

		public GameMap()
		{
			Snakes = new List<Snake>();
			Foods = new List<RectangleShape>();
		}

		public void CreateFood(int indexX, int indexY)
		{
			var food = new RectangleShape(Program.SizeOfRectangle) {
				Position = Utils.GetPositionFromIndexes(indexX, indexY),
				FillColor = Color.Red
			};

			if (IsFreePosition(food.Position))
				Foods.Add(food);
		}

		public bool IsFreePosition(Vector2f position)
		{
			if (IsFoodPosition(position))
				return false;
			foreach (var snake in Snakes)
				foreach (var bodyPart in snake.Body)
					if (position.Equals(bodyPart.Position))
						return false;
			return true;
		}

		public bool IsFoodPosition(Vector2f position)
		{
			foreach (var food in Foods)
				if (position.Equals(food.Position))
					return true;
			return false;
		}

		public RectangleShape GetFoodFromPosition(Vector2f position)
		{
			foreach (var food in Foods)
				if (position.Equals(food.Position))
					return food;
			return null;
		}
	}
}
