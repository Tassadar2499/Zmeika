using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zmeika
{
	class ClockTimer : IUpdate
	{
		private Clock Clock { get; set; } = new Clock();
		public Time Time { get; private set; } = Time.Zero;
		public Time Interval { get; set; }

		public delegate void TimerEventHandler();

		public event TimerEventHandler Tick;

		public bool Ticked { get; private set; }

		public ClockTimer(Time interval)
		{
			if (interval.AsMilliseconds() <= 0)
				throw new ArgumentException("Interval must be > 0");

			Interval = interval;
		}

		public void Update(float dt)
		{
			Time += Clock.Restart();

			if (Time >= Interval)
			{
				Time -= Interval;
				Tick();
				Ticked = true;
			}
			else
				Ticked = false;
		}
	}

	class UpdateTimer : IUpdate
	{
		public Time Time { get; private set; } = Time.Zero;
		public Time Interval { get; set; }

		public delegate void TimerEventHandler();

		public event TimerEventHandler Tick;

		public bool Ticked { get; private set; }

		public UpdateTimer(Time interval)
		{
			if (interval.AsMilliseconds() <= 0)
				throw new ArgumentException("Interval must be > 0");

			Interval = interval;
		}

		public void Update(float dt)
		{
			Time += Time.FromMilliseconds((int)dt);

			if (Time >= Interval)
			{
				Time -= Interval;
				Tick();
				Ticked = true;
			}
			else
				Ticked = false;
		}
	}
}
