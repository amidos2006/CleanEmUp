using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Graphics.Lighting
{
    public class LightSource : ILogical
    {
        public Vector2 Position;
        public float Scale;
        public Color TintColor;
        public float Alpha;
        public float DeltaAlpha;

        public LightSource()
        {
            Position = new Vector2();
            Scale = 1;
            TintColor = Color.White;
            Alpha = 1;
            DeltaAlpha = 0;
        }

        public LightSource(Vector2 position, float scale, Color tintColor)
        {
            Position = position;
            Scale = scale;
            TintColor = tintColor;
            Alpha = 1;
            DeltaAlpha = 0;
        }

        public bool IsFinish()
        {
            if (Alpha <= 0 && DeltaAlpha < 0)
            {
                return true;
            }

            return false;
        }

        public void Update(GameTime gameTime)
        {
            Alpha += DeltaAlpha;
        }
    }
}
