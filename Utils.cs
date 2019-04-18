using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmeika
{
	public enum Direction
	{
		Up,
		Down,
		Left,
		Right
	}

	public static class Utils
	{
		public static Vector2f GetMousePosition()
			=> Program.renderWindow.MapPixelToCoords(Mouse.GetPosition(Program.renderWindow));


		public static Vector2f GetPositionFromIndexes(int indexX, int indexY)
		{
			return new Vector2f(indexX * (Program.SizeOfRectangle.X + Program.RANGE_BETWEEN_BLOCKS), indexY *
				(Program.SizeOfRectangle.Y + Program.RANGE_BETWEEN_BLOCKS));
		}

		public static int GetIndexFromPosition(float position)
		{
			return (int)(position / (Program.SizeOfRectangle.X + Program.RANGE_BETWEEN_BLOCKS));
		}

		public static bool IsOppositeDirection(this Direction dirA, Direction dirB)
		{
			return (dirA == Direction.Down && dirB == Direction.Up) ||
			(dirA == Direction.Up && dirB == Direction.Down) ||
			(dirA == Direction.Left && dirB == Direction.Right) ||
			(dirA == Direction.Right && dirB == Direction.Left);
		}

		public static int GetObjectIndex<T>(this IEnumerable<T> array, T element)
		{
			int index = 0;
			foreach (var obj in array)
			{
				if (obj.Equals(element))
					return index;
				index++;
			}

			return -1;
		}

		public static int SetInInterval(int current, int min, int max)
		{
			if (current > max)
				return min;
			if (current < min)
				return max;
			return current;
		}
	}
}
