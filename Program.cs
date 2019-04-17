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
		public static GameMap gameMap = new GameMap();
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
			renderWindow.Closed += (obj, arg) => (obj as RenderWindow).Close();
			randomizer = new Random();

			mapSize = ((int)(renderWindow.Size.X / (SizeOfRectangle.X + RANGE_BETWEEN_BLOCKS * 2)),
					(int)(renderWindow.Size.Y / (SizeOfRectangle.Y + RANGE_BETWEEN_BLOCKS * 2)));

			gameMap.Snakes.Add(CreateSnake(5, 5, 20, Color.Blue));
			gameMap.Snakes.Add(CreateSnake(15, 5, 20, Color.White));

			var currentFont = new Font("font.ttf");
			textLength = new Text("Длина - " + gameMap.Snakes.First().Body.Count, currentFont);
			textRecord = new Text("Рекорд - " + ChangeCurrentRecord(gameMap.Snakes.First().Body.Count), currentFont)
			{
				Position = new Vector2f(0, 30),
				Color = Color.Green
			};

			timer = new Timer(Time.FromSeconds(0.1f));
			timer.Tick += SnakeMove;
			timer.Tick += GenerateFood;
			clock = new Clock();

			while (renderWindow.IsOpen)
			{
				var dt = clock.Restart().AsMicroseconds() * 0.001f;
				renderWindow.DispatchEvents();
				timer.Update(dt);
				renderWindow.Clear();

				foreach (var snake in gameMap.Snakes)
					renderWindow.Draw(snake);

				foreach (var food in gameMap.Foods)
					renderWindow.Draw(food);

				renderWindow.Draw(textLength);
				renderWindow.Draw(textRecord);
				renderWindow.Display();
			}
		}

		private static void SnakeMove()
		{
			foreach (var snake in gameMap.Snakes)
				snake.Move();
		}

		private static void GenerateFood()
		{
			//сделать проверку на жопу змеи
			if (gameMap.Foods.Count < 5)
			{
				var indexX = randomizer.Next(0, (int)(renderWindow.Size.X /
					(SizeOfRectangle.X + RANGE_BETWEEN_BLOCKS * 2)));

				var indexY = randomizer.Next(0, (int)(renderWindow.Size.Y /
					(SizeOfRectangle.Y + RANGE_BETWEEN_BLOCKS * 2)));

				gameMap.CreateFood(indexX, indexY);
			}
		}

		private static Snake CreateSnake(int indexX, int indexY, int lenght, Color color)
		{
			var snake = new Snake(indexX, indexY, lenght, color);

			snake.EatJeppa += OnSnakeEatsJeppa;
			snake.LengthChanged += ChangeText;

			return snake;
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

		private static void OnSnakeEatsJeppa(int index)
		{
			//for (int i = 0; i <= index; i++)
			//	snake.Body.Dequeue();

			MusicPlayer.PlaySoundDeath();
			//ChangeText(snake.Body.Count);
		}

		private static void KeyPressed(object sender, KeyEventArgs e)
		{
			switch (e.Code)
			{
				case Keyboard.Key.W: gameMap.Snakes.First().ChangeDirection(Direction.Up); break;
				case Keyboard.Key.S: gameMap.Snakes.First().ChangeDirection(Direction.Down); break;
				case Keyboard.Key.A: gameMap.Snakes.First().ChangeDirection(Direction.Left); break;
				case Keyboard.Key.D: gameMap.Snakes.First().ChangeDirection(Direction.Right); break;

				case Keyboard.Key.Num8: gameMap.Snakes.Last().ChangeDirection(Direction.Up); break;
				case Keyboard.Key.Num5: gameMap.Snakes.Last().ChangeDirection(Direction.Down); break;
				case Keyboard.Key.Num4: gameMap.Snakes.Last().ChangeDirection(Direction.Left); break;
				case Keyboard.Key.Num6: gameMap.Snakes.Last().ChangeDirection(Direction.Right); break;

				case Keyboard.Key.Escape:
					renderWindow.Close();
					break;
			}
		}
	}
}
