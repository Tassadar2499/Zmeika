using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.System;
using SFML.Graphics;
using SFML.Audio;
using System.IO;
using Newtonsoft.Json;

namespace Zmeika
{
	class Program
	{
		public static Snake snake;
		public static List<RectangleShape> foods;
		public static Random randomizer;
		public static RenderWindow renderWindow;
		public static (int x, int y) mapSize;
		public static Text textLength;
		public static Text textRecord;
		public static Timer timer;
		public static Clock clock;
		public static Vector2f SizeOfRectangle { get; private set; } = new Vector2f(20, 20);
		public const float RANGE_BETWEEN_BLOCKS = 1;
		static void Main(string[] args)
		{
			renderWindow = new RenderWindow(new VideoMode(600, 600), "game");
			renderWindow.SetFramerateLimit(120);
			renderWindow.KeyPressed += KeyPressed;
			randomizer = new Random();

			mapSize = ((int)(renderWindow.Size.X / (SizeOfRectangle.X + RANGE_BETWEEN_BLOCKS * 2)),
					(int)(renderWindow.Size.Y / (SizeOfRectangle.Y + RANGE_BETWEEN_BLOCKS * 2)));

			snake = new Snake(5, 5, 3);

			snake.EatJeppa += SnakeEatsJeppa;
			snake.LengthChanged += ChangeText;
			foods = new List<RectangleShape>();

			var currentFont = new Font("font.ttf");
			textLength = new Text("Длина - " + snake.Body.Count, currentFont);
			textRecord = new Text("Рекорд - " + ChangeCurrentRecord(snake.Body.Count), currentFont)
			{
				Position = new Vector2f(0, 30),
				Color = Color.Green
			};

			timer = new Timer(Time.FromSeconds(0.1f));
			timer.Tick += SnakeMove;
			timer.Tick += CreateFood;
			clock = new Clock();

			Parallel.Invoke(PlayGame);
		}

		private static void PlayGame()
		{
			while (renderWindow.IsOpen)
			{
				var dt = clock.Restart().AsMicroseconds() * 0.001f;
				renderWindow.DispatchEvents();
				timer.Update(dt);
				renderWindow.Clear();
				renderWindow.Draw(snake);
				foreach (var food in foods)
					renderWindow.Draw(food);
				renderWindow.Draw(textLength);
				renderWindow.Draw(textRecord);
				renderWindow.Display();
			}
		}

		private static void SnakeMove()
		{
			snake.Move();
		}

		private static int ChangeCurrentRecord(int length)
		{
			var path = "Record.txt";
			var str = JsonConvert.DeserializeObject(File.ReadAllText(path));
			var lengthRecord = int.Parse(str.ToString());
			if (length > lengthRecord)
			{
				str = JsonConvert.SerializeObject(length);
				File.Delete(path);
				File.AppendAllText(path, str.ToString());
				lengthRecord = length;
			}
			return lengthRecord;
		}

		private static void ChangeText(int length)
		{
			textLength.DisplayedString = "Длина - " + length;
			textRecord.DisplayedString = "Рекорд - " + ChangeCurrentRecord(length);
		}

		private static void SnakeEatsJeppa(int index)
		{
			for (int i = 0; i <= index; i++)
				snake.Body.Dequeue();

			MusicPlayer.PlaySoundDeath();
			ChangeText(snake.Body.Count);
		}

		private static void KeyPressed(object sender, KeyEventArgs e)
		{
			switch (e.Code)
			{
				case Keyboard.Key.W:
					snake.ChangeDirection(Direction.Up);
					break;
				case Keyboard.Key.S:
					snake.ChangeDirection(Direction.Down);
					break;
				case Keyboard.Key.A:
					snake.ChangeDirection(Direction.Left);
					break;
				case Keyboard.Key.D:
					snake.ChangeDirection(Direction.Right);
					break;
				case Keyboard.Key.Escape:
					renderWindow.Close();
					break;
			}
		}

		private static void CreateFood()
		{
			if (foods.Count < 5)
			{
				var indexX = randomizer.Next(0, (int)(renderWindow.Size.X / (SizeOfRectangle.X + RANGE_BETWEEN_BLOCKS * 2)));
				var indexY = randomizer.Next(0, (int)(renderWindow.Size.Y / (SizeOfRectangle.Y + RANGE_BETWEEN_BLOCKS * 2)));
				var food = new RectangleShape(SizeOfRectangle);
				food.Position = Utils.GetPositionFromIndexes(indexX, indexY);
				food.FillColor = Color.Red;
				if (IsFreePosition(food.Position))
					foods.Add(food);
			}
		}		

		private static bool IsFreePosition(Vector2f position)
		{
			if (IsFoodPosition(position))
				return false;
			foreach (var bodyPart in snake.Body)
				if (position.Equals(bodyPart.Position))
					return false;
			return true;
		}

		public static bool IsFoodPosition(Vector2f position)
		{
			foreach (var food in foods)
				if (position.Equals(food.Position))
					return true;
			return false;
		}

		public static RectangleShape GetFoodFromPosition(Vector2f position)
		{
			foreach (var food in foods)
				if (position.Equals(food.Position))
					return food;
			return null;
		}
	}
}
