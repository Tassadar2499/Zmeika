using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using SFML.Audio;

namespace Zmeika
{
	public static class MusicPlayer
	{
		private static SoundBuffer _soundBuffer = new SoundBuffer("pain.wav");

		public static void PlaySoundDeath()
		{
			var sound = new Sound(_soundBuffer)
			{
				Volume = 0.2f
			};

			sound.Play();
		}
	}
}
