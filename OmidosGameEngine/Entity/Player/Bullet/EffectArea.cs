using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Tween;
using OmidosGameEngine.Entity.Object.File;
using OmidosGameEngine.Entity.Boss;

namespace OmidosGameEngine.Entity.Player.Bullet
{
    public class EffectArea : BaseEntity
    {
        protected Image image;
        protected float deltaRadius;
        protected float currentRadius;
        protected float maxRadius;
        protected Alarm alarm;

        public Action EndAction
        {
            set;
            get;
        }

        public double GetRemainingPercent
        {
            get
            {
                return 1 - alarm.PercentComplete();
            }
        }

        public EffectArea(Color color, float timeToLast = 5)
        {
            EndAction = null;
            
            currentRadius = 0;
            deltaRadius = 10;

            alarm = new Alarm(timeToLast, TweenType.OneShot, ChangeRadiusDirection);
            AddTween(alarm, true);

            EntityCollisionType = CollisionType.Effect;
        }

        protected virtual void ChangeRadiusDirection()
        {
            deltaRadius *= -1;
        }

        protected virtual void DoEffect(BaseEnemy enemy)
        {
        }

        protected virtual void DoEffect(BaseBoss boss)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            currentRadius += deltaRadius * OGE.PlayerSlowFactor;
            if (currentRadius > maxRadius)
            {
                currentRadius = maxRadius;
            }

            if (deltaRadius < 0 && currentRadius <= 0)
            {
                if (EndAction == null)
                {
                    OGE.CurrentWorld.RemoveEntity(this);
                }
                else
                {
                    EndAction();
                }

                return;
            }

            image.Scale = currentRadius / maxRadius;

            List<BaseEntity> enemies = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Enemy);
            foreach (BaseEntity entity in enemies)
            {
                if (OGE.GetDistance(entity.Position, Position) < currentRadius/2)
                {
                    DoEffect(entity as BaseEnemy);
                }
            }

            enemies = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Boss);
            foreach (BaseEntity entity in enemies)
            {
                if (OGE.GetDistance(entity.Position, Position) < currentRadius / 2)
                {
                    DoEffect(entity as BaseBoss);
                }
            }

            alarm.SpeedFactor = OGE.PlayerSlowFactor;
        }
    }
}
