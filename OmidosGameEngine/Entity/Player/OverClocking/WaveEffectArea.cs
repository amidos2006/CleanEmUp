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
    public class WaveEffectArea : EffectArea
    {
        private float powerOfWave;
        protected float totalTime;
        protected Color baseColor;
        protected bool wavesEnded;

        public WaveEffectArea(Color color, float powerOfWave, float timeToLast)
            : base(color, timeToLast)
        {
            this.powerOfWave = powerOfWave;
            this.baseColor = color;
            this.totalTime = timeToLast;
            this.wavesEnded = false;

            image = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bullets\WaveRange"));
            image.Scale = 0;
            image.TintColor = color;
            image.CenterOrigin();

            maxRadius = image.Width;
            deltaRadius *= 2;

            CurrentImages.Add(image);
        }

        protected override void DoEffect(BaseEnemy enemy)
        {
            base.DoEffect(enemy);

            float percent = (maxRadius - currentRadius) / maxRadius;
            enemy.EnemyHit(0, powerOfWave * percent, OGE.GetAngle(Position, enemy.Position));
        }

        protected override void DoEffect(BaseBoss enemy)
        {
            base.DoEffect(enemy);

            float percent = (maxRadius - currentRadius) / maxRadius;
            enemy.BossHit(0, powerOfWave * percent, OGE.GetAngle(Position, enemy.Position));
        }

        protected override void ChangeRadiusDirection()
        {
            wavesEnded = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float percent = (maxRadius - currentRadius) / maxRadius;
            CurrentImages[0].TintColor = baseColor * percent;

            if (percent <= 0)
            {
                if (wavesEnded)
                {
                    if (EndAction != null)
                    {
                        EndAction();
                    }
                    else
                    {
                        OGE.CurrentWorld.RemoveEntity(this);
                    }

                    return;
                }
                else
                {
                    currentRadius = 0;
                }
            }
        }
    }
}
