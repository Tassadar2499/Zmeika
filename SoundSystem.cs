using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using SFML.Audio;
using System.IO;

namespace Zmeika
{
	public static class SoundSystem
	{
		private static SoundBuffer _soundBuffer = new SoundBuffer("dead.ogg");
		private static List<Music> _musicBuffer = new List<Music>();
		public static Music CurrentMusic;
		private static Sound _deathSound;

		static SoundSystem()
		{
			var files = Directory.GetFiles("music\\");
			foreach (var file in files)
				_musicBuffer.Add(new Music(file));
		}

		public static void ChangeMusic()//быдло код
		{
			CurrentMusic.Stop();
			var index = _musicBuffer.IndexOf(CurrentMusic);
			if (index == _musicBuffer.Count - 1)
				CurrentMusic = _musicBuffer[0];
			else
				CurrentMusic = _musicBuffer[index + 1];
			CurrentMusic.Volume = 5f;
			CurrentMusic.Play();
		}

		public static void StartMusic()
		{
			var index = Program.randomizer.Next(0, _musicBuffer.Count);
			CurrentMusic = _musicBuffer[index];
			CurrentMusic.Volume = 5f;

			CurrentMusic.Play();
		}

		public static void StopDeathSound()
		{
			_deathSound = null;
		}

		public static void PlayDeathSound()
		{
			_deathSound = new Sound(_soundBuffer)
			{
				Volume = 8f
			};

			_deathSound.Play();
		}
	}
}
