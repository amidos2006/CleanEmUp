using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Enemy;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Graphics;
using OmidosGameEngine.Graphics.Particles;
using OmidosGameEngine.Entity.Boss;

namespace OmidosGameEngine.Entity.Player.Bullet
{
    public class GasEffectArea : EffectArea
    {
        private float damage;
        private Color baseColor;
        private Image effectImage;
        private float effectRadius;

        public GasEffectArea(Color color, float damage, float timeToLast)
            : base(color, timeToLast)
        {
            this.damage = damage;
            this.baseColor = color;

            image = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bullets\EffectRange"));
            image.Scale = 0;
            image.TintColor = color;
            image.CenterOrigin();

            effectImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bullets\EffectRangeEffect"));
            effectImage.Scale = 0;
            effectImage.TintColor = color;
            effectImage.CenterOrigin();
            effectRadius = 0;

            maxRadius = image.Width;

            CurrentImages.Add(image);
        }

        protected override void DoEffect(BaseEnemy enemy)
        {
            base.DoEffect(enemy);

            float damagePercent = OGE.GetDistance(enemy.Position, Position) / 150;

            enemy.EnemyHit(damagePercent * damage, 0, 0, true);
        }

        protected override void DoEffect(BaseBoss enemy)
        {
            base.DoEffect(enemy);

            float damagePercent = OGE.GetDistance(enemy.Position, Position) / 150;

            enemy.BossHit(damagePercent * damage, 0, 0, true);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (deltaRadius > 0 && currentRadius == maxRadius)
            {
                effectRadius = (effectRadius + 1.25f * deltaRadius) % maxRadius;
                effectImage.Scale = effectRadius / maxRadius;
                effectImage.TintColor = baseColor * effectImage.ScaleX;
            }
            else
            {
                effectImage.Scale = 0;
            }
        }

        public override void Draw(Camera camera)
        {
            effectImage.Draw(Position, camera);

            base.Draw(camera);
        }
    }
}
