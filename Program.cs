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

		public static bool IsWorldPaused = true;
		public static Menu startMenu;
		public static Menu pauseMenu;
		public static DeathScreen deathScreen;


		private static RenderWindow BuildRenderWindow((uint X, uint Y) size)
		{
			var window = new RenderWindow(new VideoMode(size.X, size.Y), "game");
			window.SetFramerateLimit(120);
			window.KeyPressed += KeyPressed;
			window.Closed += (obj, arg) => (obj as RenderWindow).Close();

			return window;
		}

		private static UpdateTimer BuildUpdateTimer(float timeInSecond)
		{
			var updateTimer = new UpdateTimer(Time.FromSeconds(timeInSecond));
			updateTimer.Tick += SnakeMove;
			updateTimer.Tick += GenerateFood;

			return updateTimer;
		}

		private static void Main(string[] args)
		{
			renderWindow = BuildRenderWindow((800, 600));

			mapSize = ((int)(renderWindow.Size.X / (SizeOfRectangle.X + RANGE_BETWEEN_BLOCKS)),
						(int)(renderWindow.Size.Y / (SizeOfRectangle.Y + RANGE_BETWEEN_BLOCKS)));

			gameMap.Snakes.Add(new Snake(5, 5, 10, Color.Blue));
			gameMap.Snakes.Add(new Snake(mapSize.X - 5, 5, 10, Color.White));
			gameMap.EatJeppas += OnSnakeEatsJeppa;
			gameMap.Show = false;

			deathScreen = new DeathScreen("dead.png");
			startMenu = BuildStartMenu();
			pauseMenu = BuildPauseMenu();

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

				if (startMenu.Show)
					startMenu.Update(dt);

				///////////////////
				renderWindow.Clear();

				if (gameMap.Show)
				{
					foreach (var snake in gameMap.Snakes)
						renderWindow.Draw(snake);

					foreach (var food in gameMap.Foods)
						renderWindow.Draw(food);
				}

				if (deathScreen.Show)
					renderWindow.Draw(deathScreen);

				if (startMenu.Show)
					renderWindow.Draw(startMenu);

				renderWindow.Display();
			}
		}

		private static Menu BuildStartMenu()
		{
			var menu = new Menu(new (string, Action)[]{
				("Начать игру", StartGame),
				("Настройки", ShowSettingMenu),
				(" ", () => { }),
				("Выход", () => renderWindow.Close())
			}, 30, "font.ttf");
			renderWindow.MouseButtonReleased += menu.OnMouseClick;
			menu.Show = true;

			return menu;
		}

		private static Menu BuildPauseMenu()
		{
			var menu = new Menu(new (string, Action)[]{
				("Продолжить игру", StartGame),
				(" ", () => { }),
				("Выход", () => renderWindow.Close())
			}, 30, "font.ttf");
			renderWindow.MouseButtonReleased += menu.OnMouseClick;
			menu.Show = false;

			return menu;
		}

		private static void ShowSettingMenu()
		{
			throw new NotImplementedException();
		}

		private static void StartGame()
		{
			IsWorldPaused = false;
			startMenu.Show = false;
			gameMap.Show = true;
		}

		private static void PauseGame()
		{
			IsWorldPaused = true;
			pauseMenu.Show = true;
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
					PauseGame();
					break;
			}
		}
	}
}
