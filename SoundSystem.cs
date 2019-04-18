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
		private static List<SoundBuffer> _musicBuffer = new List<SoundBuffer>();
		public static Sound CurrentMusic;

		static SoundSystem()
		{
			var files = Directory.GetFiles("music\\");
			foreach (var file in files)
				_musicBuffer.Add(new SoundBuffer(file));
		}

		public static void ChangeMusic()
		{
			///Делай бля!
		}

		public static void PlayAllMusic()
		{
			var index = Program.randomizer.Next(0, _musicBuffer.Count);
			CurrentMusic = new Sound(_musicBuffer[index])
			{
				Volume = 5f
			};

			CurrentMusic.Play();
		}

		public static void PlaySoundDeath()
		{
			var sound = new Sound(_soundBuffer)
			{
				Volume = 5f
			};

			sound.Play();
		}
	}
}
