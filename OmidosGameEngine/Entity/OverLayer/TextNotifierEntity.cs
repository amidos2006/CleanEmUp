using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class TextNotifierEntity : BaseEntity
    {
        private Text text;

        private float scale;
        private float scaleSpeed;
        private float alpha;
        private float alphaSpeed;
        private Color normalColor;

        public TextNotifierEntity(string showingText)
        {
            Position.X = OGE.HUDCamera.Width / 2;
            Position.Y = OGE.HUDCamera.Height / 2;

            scale = 1;
            scaleSpeed = 0.3f;
            alpha = 0.5f;
            alphaSpeed = 0.008f;
            normalColor = new Color(150, 255, 130);

            text = new Text(showingText, FontSize.XLarge);
            text.Align(AlignType.Center);
            text.OriginY = text.Height / 2;
            text.TintColor = normalColor * alpha;
            text.Scale = scale;

            EntityCollisionType = Collision.CollisionType.Explosion;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            alpha -= alphaSpeed;
            if (alpha <= 0)
            {
                OGE.CurrentWorld.RemoveOverLayer(this);
            }

            scale += scaleSpeed;

            text.Scale = scale;
            text.TintColor = normalColor * alpha;
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            text.Draw(Position, camera);
        }
    }
}
