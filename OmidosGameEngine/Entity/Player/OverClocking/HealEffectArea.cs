using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Entity.Player.Bullet;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Entity.Enemy;

namespace OmidosGameEngine.Entity.Player.OverClocking
{
    public class HealEffectArea : EffectArea
    {
        private float healingRate;

        public HealEffectArea(Color color, float healingRate, float timeToLast)
            : base(color, timeToLast)
        {
            this.healingRate = healingRate;

            image = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bullets\HealingRange"));
            image.Scale = 0;
            image.TintColor = color;
            image.CenterOrigin();

            maxRadius = image.Width;

            CurrentImages.Add(image);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            List<BaseEntity> player = OGE.CurrentWorld.GetCollisionEntitiesType(Collision.CollisionType.Player);
            if (player.Count > 0)
            {
                (player[0] as PlayerEntity).IncreaseHealth(healingRate * OGE.PlayerSlowFactor);
            }
        }
    }
}
