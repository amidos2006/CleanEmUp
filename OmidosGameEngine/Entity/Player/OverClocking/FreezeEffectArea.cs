using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Entity.Player.Bullet;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Entity.Boss;

namespace OmidosGameEngine.Entity.Player.OverClocking
{
    public class FreezeEffectArea : EffectArea
    {
        private float freezePercentage;

        public FreezeEffectArea(Color color, float freezePercentage, float timeToLast)
            : base(color, timeToLast)
        {
            this.freezePercentage = freezePercentage;

            image = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bullets\FreezeRange"));
            image.Scale = 0;
            image.TintColor = color;
            image.CenterOrigin();

            maxRadius = image.Width;

            CurrentImages.Add(image);
        }

        protected override void DoEffect(BaseEnemy enemy)
        {
            base.DoEffect(enemy);

            float percent = OGE.GetDistance(enemy.Position, Position) / maxRadius;
            enemy.SlowFactor -= percent * freezePercentage;
        }

        protected override void DoEffect(BaseBoss enemy)
        {
            base.DoEffect(enemy);

            float percent = OGE.GetDistance(enemy.Position, Position) / maxRadius;
            enemy.SlowFactor -= percent * freezePercentage;
        }
    }
}
