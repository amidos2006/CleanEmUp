using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Graphics
{
    public class HackedNoiseEffect : Backdrop
    {
        private const int NOISE_SIZE = 100;

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

        public HackedNoiseEffect(float deltaAlpha)
            : base(new Texture2D(OGE.GraphicDevice, NOISE_SIZE, NOISE_SIZE))
        {
            DeltaAlpha = deltaAlpha;

            texture = OGE.Content.Load<Texture2D>(@"Graphics\Effects\Noise");
            sourceRectangle = new Rectangle(0, 0, NOISE_SIZE, NOISE_SIZE);
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

            Vector2 texturePosition = new Vector2();
            texturePosition.X = OGE.Random.Next(texture.Width - NOISE_SIZE);
            texturePosition.Y = OGE.Random.Next(texture.Height - NOISE_SIZE);

            sourceRectangle = new Rectangle((int)texturePosition.X, (int)texturePosition.Y, NOISE_SIZE, NOISE_SIZE);
        }
    }
}
