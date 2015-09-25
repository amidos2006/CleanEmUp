using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Tween
{
    public delegate void AlarmFinished();

    public class Alarm : ITween
    {
        private AlarmFinished alarmFinished;
        private bool alarmStart;
        private double totalSeconds;
        private double currentSeconds;
        private TweenType tweenType;

        public float SpeedFactor
        {
            set;
            get;
        }

        public double TotalSeconds
        {
            get
            {
                return totalSeconds;
            }
        }

        public double CurrentSeconds
        {
            get
            {
                return currentSeconds;
            }
        }

        public Alarm(double totalSeconds, TweenType type = TweenType.OneShot, AlarmFinished alarmFinished = null)
        {
            this.alarmStart = false;
            this.currentSeconds = totalSeconds;
            this.totalSeconds = totalSeconds;
            this.alarmFinished = alarmFinished;
            this.tweenType = type;

            this.SpeedFactor = 1;
        }

        public void Start(bool reset = false)
        {
            this.alarmStart = true;
            if (currentSeconds <= 0 || reset)
            {
                currentSeconds = totalSeconds;
            }
        }

        public void Pause()
        {
            this.alarmStart = false;
        }

        public void Stop()
        {
            this.alarmStart = false;
            this.currentSeconds = totalSeconds;
        }

        public void Reset(double newTime)
        {
            this.alarmStart = false;
            this.currentSeconds = newTime;
            this.totalSeconds = newTime;
        }

        public bool IsRunning()
        {
            return alarmStart;
        }

        public double PercentComplete()
        {
            return (totalSeconds - currentSeconds) / totalSeconds;
        }

        public void Update(GameTime gameTime)
        {
            if (!alarmStart)
            {
                return;
            }

            currentSeconds -= gameTime.ElapsedGameTime.TotalSeconds * SpeedFactor;
            if (currentSeconds <= 0)
            {
                currentSeconds = 0;
                switch (tweenType)
                {
                    case TweenType.Looping:
                        currentSeconds = totalSeconds;
                        break;
                    case TweenType.OneShot:
                        alarmStart = false;
                        break;
                }

                if (alarmFinished != null)
                {
                    alarmFinished();
                }
            }
        }
    }
}
