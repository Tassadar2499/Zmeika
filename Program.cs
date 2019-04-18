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
		public const float RANGE_BETWEEN_BLOCKS = 2;
		public static Vector2f SizeOfRectangle { get; private set; } = new Vector2f(20, 20);
		public static (int X, int Y) mapSize;

		public static GameMap gameMap = new GameMap();
		public static Random randomizer = new Random();
		public static RenderWindow renderWindow;

		public static bool IsWorldPaused = false;
		public static DeathScreen deathScreen;


		private static RenderWindow BuildRenderWindow((uint X, uint Y) size)
		{
			var window = new RenderWindow(new VideoMode(size.X, size.Y), "game");
			window.SetFramerateLimit(120);
			window.KeyPressed += KeyPressed;
			window.Closed += (obj, arg) => (obj as RenderWindow).Close();

			return window;
		}

		private static Timer BuildUpdateTimer(float timeInSecond)
		{
			var updateTimer = new Timer(Time.FromSeconds(timeInSecond));
			updateTimer.Tick += SnakeMove;
			updateTimer.Tick += GenerateFood;

			return updateTimer;
		}

		private static void Main(string[] args)
		{
			renderWindow = BuildRenderWindow((800, 600));

			mapSize =  ((int)(renderWindow.Size.X / (SizeOfRectangle.X + RANGE_BETWEEN_BLOCKS)),
						(int)(renderWindow.Size.Y / (SizeOfRectangle.Y + RANGE_BETWEEN_BLOCKS)));

			gameMap.Snakes.Add(new Snake(5, 5, 10, Color.Blue));
			gameMap.Snakes.Add(new Snake(mapSize.X - 5, 5, 10, Color.White));
			gameMap.EatJeppas += OnSnakeEatsJeppa;

			deathScreen = new DeathScreen("dead.png");

			var updateTimer = BuildUpdateTimer(0.11f);
			var clock = new Clock();

			while (renderWindow.IsOpen)
			{
				var dt = clock.Restart().AsMicroseconds() * 0.001f;
				renderWindow.DispatchEvents();
				///////////////////

				if (!IsWorldPaused)
					updateTimer.Update(dt);

				if (deathScreen.Show)
					deathScreen.Update(dt);

				///////////////////
				renderWindow.Clear();

				foreach (var snake in gameMap.Snakes)
					renderWindow.Draw(snake);

				foreach (var food in gameMap.Foods)
					renderWindow.Draw(food);

				if (deathScreen.Show)
					renderWindow.Draw(deathScreen);

				renderWindow.Display();
			}
		}

		private static void SnakeMove()
		{
			foreach (var snake in gameMap.Snakes)
				snake.Move();

			gameMap.CheckEatJeppas();
		}

		private static void GenerateFood()
		{
			//сделать проверку на жопу змеи
			if (gameMap.Foods.Count < 5)
			{
				var indexX = randomizer.Next(0, mapSize.X);
				var indexY = randomizer.Next(0, mapSize.Y);
				gameMap.CreateFood(indexX, indexY);
			}
		}

		private static void OnSnakeEatsJeppa(int snakeIndex)
		{
			IsWorldPaused = true;
			deathScreen.Show = true;
			SoundSystem.PlaySoundDeath();
		}

		private static void KeyPressed(object sender, KeyEventArgs e)
		{
			switch (e.Code)
			{
				case Keyboard.Key.W: gameMap.Snakes.First().ChangeDirection(Direction.Up); break;
				case Keyboard.Key.S: gameMap.Snakes.First().ChangeDirection(Direction.Down); break;
				case Keyboard.Key.A: gameMap.Snakes.First().ChangeDirection(Direction.Left); break;
				case Keyboard.Key.D: gameMap.Snakes.First().ChangeDirection(Direction.Right); break;

				case Keyboard.Key.Numpad8: gameMap.Snakes.Last().ChangeDirection(Direction.Up); break;
				case Keyboard.Key.Numpad5: gameMap.Snakes.Last().ChangeDirection(Direction.Down); break;
				case Keyboard.Key.Numpad4: gameMap.Snakes.Last().ChangeDirection(Direction.Left); break;
				case Keyboard.Key.Numpad6: gameMap.Snakes.Last().ChangeDirection(Direction.Right); break;

				case Keyboard.Key.Escape:
					renderWindow.Close();
					break;
			}
		}
	}
}
