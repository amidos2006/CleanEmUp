using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Tween
{
    public interface ITween
    {
        void Start(bool reset = false);
        void Pause();
        void Stop();
        void Reset(double newTime);
        bool IsRunning();
        double PercentComplete();
        void Update(GameTime gameTime);
    }
}
