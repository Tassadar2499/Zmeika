using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using SFML.Audio;

namespace Zmeika
{
	public static class SoundSystem
	{
		private static SoundBuffer _soundBuffer = new SoundBuffer("dead.wav");

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
