using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OmidosGameEngine.Entity.Object
{
    public class ObjectNotifierEntity:BaseEntity
    {
        private Image image;

        private float alpha;
        private float alphaSpeed;
        private float scale;
        private float scaleSpeed;

        public ObjectNotifierEntity()
        {
            alphaSpeed = 0.05f;
            scaleSpeed = 0.05f;

            alpha = 1f;
            scale = 1f;

            image = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Objects\ObjectNotifier"));
            image.CenterOrigin();
            image.TintColor = Color.White * alpha;
            image.Scale = scale;

            CurrentImages.Add(image);

            EntityCollisionType = Collision.CollisionType.Object;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            alpha -= alphaSpeed;
            if (alpha <= 0)
            {
                OGE.CurrentWorld.RemoveEntity(this);
            }

            scale += scaleSpeed;

            image.TintColor = Color.White * alpha;
            image.Scale = scale;
        }
    }
}
