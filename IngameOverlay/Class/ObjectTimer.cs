using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace IngameOverlay
{
    public class ObjectTimer
    {
        public int id { get; set; } = 0;
        public int leftTime { get; set; } = -1;
        private DispatcherTimer timer;
        private int respawnTime;
        public event OnTicked onTicked;
        public delegate void OnTicked(ObjectTimer sender, EventArgs args);
        public event OnTimerEnd onTimerEnd;
        public delegate void OnTimerEnd(ObjectTimer sender, EventArgs args);
        public ObjectTimer(int id)
        {
            timer = new DispatcherTimer(DispatcherPriority.Send);
            this.id = id;
            respawnTime = 300;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
        }

        public ObjectTimer(int id, int respawnTime)
        {
            timer = new DispatcherTimer();
            this.id = id;
            this.respawnTime = respawnTime;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (leftTime-- < 0)
                onTimerEnd(this, e);
            else
                onTicked(this, e);
        }

        public void Start()
        {
            leftTime = respawnTime;
            timer.Start();
            onTicked(this, new EventArgs());
        }

        public void Stop()
        {
            leftTime = -1;
            timer.Stop();
        }
    }
}
