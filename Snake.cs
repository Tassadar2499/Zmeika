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
		private Direction currentDirection = Direction.Down;
		private Direction lastDirection = Direction.Down;
		public Queue<RectangleShape> Body { get; set; }
		public RectangleShape Head => Body.Last();
		public Color Color { get; private set; }

		public event Action<int> EatJeppa;
		public event Action<int> LengthChanged;

		public Snake(int mapX, int mapY, int length, Color color)
		{
			Color = color;
			Body = new Queue<RectangleShape>();
			for (int i = 0; i < length; i++)
				Body.Enqueue(CreateBodyPart((mapX, mapY), Color));
		}

		public void Move()
		{
			var bodyPosition = Body.Last().Position;
			var currentIndex = (
				Utils.GetIndexFromPosition(bodyPosition.X),
				Utils.GetIndexFromPosition(bodyPosition.Y));
			var (X, Y) = GetNextIndexes(currentDirection, currentIndex);
			lastDirection = currentDirection;

			Body.Enqueue(CreateBodyPart((X, Y), Color));

			var food = Program.gameMap.GetFoodFromPosition(Utils.GetPositionFromIndexes(X, Y));
			if (food == null)
			{
				Body.Dequeue();
			}
			else
			{
				Program.gameMap.Foods.Remove(food);
				LengthChanged?.Invoke(Body.Count);
			}

			CheckSelfEatJeppa();

			Body.Last().Position = ReachingBorders((X, Y));
		}

		private void CheckSelfEatJeppa()
		{
			var headPosition = Body.Last().Position;
			foreach (var bodyPart in Body.Take(Body.Count - 1))
			{
				if (bodyPart.Position.Equals(headPosition))
				{
					EatJeppa?.Invoke(Body.GetObjectIndex(bodyPart));
					return;
				}
			}
		}

		public static bool CheckEatJeppa(Snake first, Snake second)
		{
			var headPosition = first.Head.Position;

			if (first.Equals(second))
			{
				foreach (var bodyPart in second.Body.Take(second.Body.Count - 1))
					if (bodyPart.Position.Equals(headPosition))
						return true;
			}
			else
			{
				foreach (var bodyPart in second.Body)
					if (bodyPart.Position.Equals(headPosition))
						return true;
			}

			return false;
		}

		private Vector2f ReachingBorders((int X, int Y) mapIndex)
		{
			var currentX = Utils.SetInInterval(mapIndex.X, 0, Program.mapSize.X);
			var currentY = Utils.SetInInterval(mapIndex.Y, 0, Program.mapSize.Y);

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

		private static RectangleShape CreateBodyPart((int X, int Y) mapIndex, Color color)
		{
			return new RectangleShape(Program.SizeOfRectangle)
			{
				Position = Utils.GetPositionFromIndexes(mapIndex.X, mapIndex.Y),
				FillColor = color
			};
		}

		private static (int X, int Y) GetNextIndexes(Direction direction, (int X, int Y) current)
		{
			switch (direction)
			{
				case Direction.Up: current.Y--; break;
				case Direction.Down: current.Y++; break;
				case Direction.Left: current.X--; break;
				case Direction.Right: current.X++; break;
			}
			return current;
		}
	}
}
