using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OmidosGameEngine.Entity.Object.File
{
    public class FileNotifierEntity : BaseEntity
    {
        private Image image;

        private float alpha;
        private float alphaSpeed;
        private float scale;
        private float scaleSpeed;

        public FileNotifierEntity()
        {
            scale = 0;
            scaleSpeed = 0.05f;
            alpha = 1;
            alphaSpeed = 0.04f;

            image = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Objects\ZipFileNotifier"));
            image.CenterOrigin();
            image.Scale = scale;
            image.TintColor = Color.White * alpha;

            CurrentImages.Add(image);

            EntityCollisionType = Collision.CollisionType.Effect;
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

            image.Scale = scale;
            image.TintColor = Color.White * alpha;
        }
    }
}
