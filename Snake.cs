using System;
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
		public enum Direction
		{
			Up,
			Down,
			Left,
			Right
		}
		private Direction currentDirection = Direction.Right;
		private Direction lastDirection = Direction.Right;
		public Queue<RectangleShape> Body { get; set; }

		public delegate void SnakeDeathEventHandler();
		public event SnakeDeathEventHandler Died;

		public Snake(int mapX, int mapY)
		{
			Body = new Queue<RectangleShape>();
			for (int i = 0; i < 3; i++)
				Body.Enqueue(CreateBodyPart(mapX + i, mapY));
		}

		public void Move()
		{
			var bodyPosition = Body.Last().Position;
			var indexX = Program.GetIndexFromPosition(bodyPosition.X);
			var indexY = Program.GetIndexFromPosition(bodyPosition.Y);
			var indexes = GetNextIndexes(currentDirection, indexX, indexY);
			lastDirection = currentDirection;
			var food = Program.GetFoodFromPosition(Program.GetPositionFromIndexes(indexes.X, indexes.Y));
			if (food == null)
				Body.Dequeue();
			else
				Program.foods.Remove(food);
			Body.Enqueue(CreateBodyPart(indexes.X, indexes.Y));
			var headPosition = Body.Last().Position;
			foreach (var bodyPart in Body.Take(Body.Count - 1))
				if (bodyPart.Position.Equals(headPosition))
					Died();
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			foreach (var rectangle in Body)
				target.Draw(rectangle);
		}

		public void ChangeDirection(Direction dir)
		{
			if (!IsOppositeDirection(dir, lastDirection))
				currentDirection = dir;
		}

		private static bool IsOppositeDirection(Direction dirA, Direction dirB)
		{
			return ((dirA == Direction.Down && dirB == Direction.Up) ||
			(dirA == Direction.Up && dirB == Direction.Down)) ||
			((dirA == Direction.Left && dirB == Direction.Right) ||
			(dirA == Direction.Right && dirB == Direction.Left));
		}

		private static RectangleShape CreateBodyPart(int mapX, int mapY)
		{
			var bodyPart = new RectangleShape(Program.SizeOfRectangle);
			bodyPart.Position = Program.GetPositionFromIndexes(mapX, mapY);
			return bodyPart;
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
