﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.System;
using SFML.Graphics;
using SFML.Audio;

namespace Zmeika
{
	public class Snake : Drawable
	{
		private Direction currentDirection = Direction.Down;
		private Direction lastDirection = Direction.Down;
		public Queue<RectangleShape> Body { get; set; }

		public event Action<int> EatJeppa;
		public event Action<int> LengthChanged;

		public Snake(int mapX, int mapY, int length)
		{
			Body = new Queue<RectangleShape>();
			for (int i = 0; i < length; i++)
				Body.Enqueue(CreateBodyPart(mapX, mapY));
		}

		public void Move()
		{
			var bodyPosition = Body.Last().Position;
			var currentIndexX = Program.GetIndexFromPosition(bodyPosition.X);
			var currentIndexY = Program.GetIndexFromPosition(bodyPosition.Y);
			var (X, Y) = GetNextIndexes(currentDirection, currentIndexX, currentIndexY);
			lastDirection = currentDirection;

			Body.Enqueue(CreateBodyPart(X, Y));

			var food = Program.GetFoodFromPosition(Utils.GetPositionFromIndexes(X, Y));
			if (food == null)
				Body.Dequeue();
			else
			{
				Program.foods.Remove(food);
				LengthChanged(Body.Count);
			}

			var headPosition = Body.Last().Position;
			foreach (var bodyPart in Body.Take(Body.Count - 1))
			{
				if (bodyPart.Position.Equals(headPosition))
				{
					EatJeppa(Body.GetObjectIndex(bodyPart));
					break;
				}
			}

			Body.Last().Position = ReachingBorders(X, Y);
		}

		private Vector2f ReachingBorders(int indexX, int indexY)
		{
			var currentX = Utils.SetInInterval(indexX, 0, Program.mapSize.x);
			var currentY = Utils.SetInInterval(indexY, 0, Program.mapSize.y);

			return Utils.GetPositionFromIndexes(currentX, currentY);
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			foreach (var rectangle in Body)
				target.Draw(rectangle);
		}

		public void ChangeDirection(Direction dir)
		{
			if (!Utils.IsOppositeDirection(dir, lastDirection))
				currentDirection = dir;
		}

		private static RectangleShape CreateBodyPart(int mapX, int mapY)
		{
			return new RectangleShape(Program.SizeOfRectangle)
			{
				Position = Utils.GetPositionFromIndexes(mapX, mapY)
			};
		}

		private static (int X, int Y) GetNextIndexes(Direction direction, int currentX, int currentY)
		{
			switch (direction)
			{
				case Direction.Up: currentY--; break;
				case Direction.Down: currentY++; break;
				case Direction.Left: currentX--; break;
				case Direction.Right: currentX++; break;
			}
			return (currentX, currentY);
		}
	}
}
