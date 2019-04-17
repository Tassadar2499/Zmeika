using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace Zmeika
{
	public static class MusicPlayer
	{
		public static void PlaySoundDeath()
		{
			var sp = new SoundPlayer("pain.wav");
			sp.Play();
		}
	}
}
