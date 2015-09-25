using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OmidosGameEngine.Graphics
{
    public class AdditiveWhiteImage : Image
    {
        private float alpha;

        public float Alpha
        {
            set
            {
                alpha = value;

                if (alpha > 0.75f)
                {
                    alpha = 0.75f;
                }
                if (alpha < 0)
                {
                    alpha = 0;
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

        public AdditiveWhiteImage(float deltaAlpha)
            :base(new Texture2D(OGE.GraphicDevice,OGE.HUDCamera.Width,OGE.HUDCamera.Height))
        {
            Alpha = 0;
            DeltaAlpha = deltaAlpha;

            Color[] whiteColor = new Color[Width * Height];
            for (int i = 0; i < whiteColor.Length; i++)
            {
                whiteColor[i] = Color.White;
            }

            texture.SetData(whiteColor);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Alpha += DeltaAlpha;
        }

        public override void Draw(Vector2 position, Camera camera)
        {
            SpriteBatch spriteBatch = OGE.SpriteBatch;

            if (!camera.CheckRectangleInCamera(new Rectangle((int)position.X, (int)position.Y, Width, Height)))
            {
                return;
            }

            spriteBatch.Begin(OGE.SpriteSortMode, BlendState.Additive);

            spriteBatch.Draw(texture, camera.ConvertToCamera(position), sourceRectangle, tintColor * Alpha, angle, origin, scale, flipping, 0);

            spriteBatch.End();
        }
    }
}
