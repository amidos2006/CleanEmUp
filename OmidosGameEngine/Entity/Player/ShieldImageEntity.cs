using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Entity.Player
{
    public class ShieldImageEntity : BaseEntity
    {
        private Image image;

        public Color TintColor
        {
            set
            {
                image.TintColor = value;
            }
            get
            {
                return image.TintColor;
            }
        }

        public ShieldImageEntity()
        {
            image = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Players\Shield"));
            image.CenterOrigin();

            CurrentImages.Add(image);

            EntityCollisionType = Collision.CollisionType.Shield;
        }
    }
}
