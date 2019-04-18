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
		private static SoundBuffer _soundBuffer = new SoundBuffer("dead.wav");
		private static List<SoundBuffer> _musicBuffer = new List<SoundBuffer>();

		static SoundSystem()
		{
			var files = Directory.GetFiles("music\\");
			foreach (var file in files)
				_musicBuffer.Add(new SoundBuffer(file));
		}
		public static void PlayAllMusic()
		{
			var index = Program.randomizer.Next(0, _musicBuffer.Count);
			var sound = new Sound(_musicBuffer[index])
			{
				Volume = 10f
			};

			sound.Play();
		}

		public static void PlaySoundDeath()
		{
			var sound = new Sound(_soundBuffer)
			{
				Volume = 100f
			};

			sound.Play();
		}
	}
}
