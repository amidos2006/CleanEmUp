using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Graphics
{
    public class NoiseEffect : Backdrop
    {
        private const int NOISE_SIZE = 32;

        private float maxNoiseValue = 0.5f;
        private float minNoiseValue = 0;

        private float alpha;

        public float Alpha
        {
            set
            {
                alpha = value;

                if (alpha > maxNoiseValue)
                {
                    alpha = maxNoiseValue;
                }
                if (alpha < minNoiseValue)
                {
                    alpha = minNoiseValue;
                }
            }
            get
            {
                return alpha;
            }
        }

        public float DeltaAlpha
        {
            set;
            get;
        }

        private Color[] choosingColors;
        private Color[] noiseColor;

        public NoiseEffect(float deltaAlpha)
            : base(new Texture2D(OGE.GraphicDevice, NOISE_SIZE, NOISE_SIZE))
        {
            DeltaAlpha = deltaAlpha;

            noiseColor = new Color[NOISE_SIZE * NOISE_SIZE];
            choosingColors = new Color[2];

            choosingColors[0] = Color.Black;
            choosingColors[1] = Color.White;
        }

        public void UpdateHealth(float health, float maxHealth)
        {
            float value = (0.75f - (health / maxHealth));
            if (value < 0)
            {
                value = 0;
            }

            minNoiseValue = value * 0.2f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Alpha += DeltaAlpha;
            TintColor = Color.White * alpha;

            if (alpha <= 0)
            {
                return;
            }

            for (int i = 0; i < noiseColor.Length; i++)
            {
                if (OGE.Random.NextDouble() < 0.6)
                {
                    noiseColor[i] = choosingColors[0];
                }
                else
                {
                    noiseColor[i] = choosingColors[1];
                }
            }

            texture.SetData(noiseColor);
        }
    }
}
