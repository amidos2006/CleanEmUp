using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Graphics
{
    public class RedHitEffectImage : Image
    {
        private float alpha;

        public float Alpha
        {
            set
            {
                alpha = value;

                if (alpha > 0.5f)
                {
                    alpha = 0.5f;
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

        public RedHitEffectImage(float deltaAlpha)
            :base(OGE.Content.Load<Texture2D>(@"Graphics\Effects\HitEffect"))
        {
            DeltaAlpha = deltaAlpha;
        }

        public override void  Update(GameTime gameTime)
        {
 	         base.Update(gameTime);
             Alpha += DeltaAlpha;
        }

        public override void  Draw(Vector2 position, Camera camera)
        {
            SpriteBatch spriteBatch = OGE.SpriteBatch;

            if (!camera.CheckRectangleInCamera(new Rectangle((int)position.X, (int)position.Y, Width, Height)))
            {
                return;
            }

            spriteBatch.Begin(OGE.SpriteSortMode, OGE.BlendState);

            spriteBatch.Draw(texture, camera.ConvertToCamera(position + new Vector2(0, 0)), sourceRectangle, tintColor * Alpha, 
                angle, origin, scale, SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(texture, camera.ConvertToCamera(position + new Vector2(camera.Width - Width, 0)), sourceRectangle, tintColor * Alpha,
                angle, origin, scale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, camera.ConvertToCamera(position + new Vector2(0, camera.Height - Height)), sourceRectangle, tintColor * Alpha,
                angle, origin, scale, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0);
            spriteBatch.Draw(texture, camera.ConvertToCamera(position + new Vector2(camera.Width - Width, camera.Height - Height)), sourceRectangle, tintColor * Alpha,
                angle, origin, scale, SpriteEffects.FlipVertically, 0);

            spriteBatch.End();
        }
    }
}
