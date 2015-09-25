using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Entity.Player;
using OmidosGameEngine.Entity.Explosion;

namespace OmidosGameEngine.Entity.Enemy
{
    public class Troy2Enemy: BaseEnemy
    {
        private const float MIN_ALPHA = 0.1f;
        private const float MAX_ALPHA = 1;

        private float alpha;
        private float alphaFadingSpeed;
        private float appearingRegion;

        public float Alpha
        {
            set
            {
                alpha = value;

                if (alpha < MIN_ALPHA)
                {
                    alpha = MIN_ALPHA;
                }

                if (alpha > MAX_ALPHA)
                {
                    alpha = MAX_ALPHA;
                }

                trailGenerator.TintColor = enemyColor * alpha;
                MainColor = Color.White * alpha;
            }
            get
            {
                return alpha;
            }
        }

        public Troy2Enemy()
            :base(new Color(120,120,20))
        {
            alpha = MIN_ALPHA;
            alphaFadingSpeed = -0.005f;
            appearingRegion = 350;

            maxSpeed = 1.5f * SPEED_UNIT;
            acceleration = 0.25f;

            direction = random.Next(360);
            rotationSpeed = 5f;

            health = 35f;
            damage = 25f;
            score = 130;

            enemyStatus = EnemyStatus.Attacking;
            
            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Enemies\Troy2")));
            CurrentImages[0].CenterOrigin();

            AddCollisionMask(new HitboxMask(CurrentImages[0].Width, CurrentImages[0].Height, 
                CurrentImages[0].OriginX, CurrentImages[0].OriginY));

            thrusters.Add(new EnemyThrusterData { Direction = 180, Length = 22 });
        }

        public override void EnemyHit(float damage, float speed, float direction, bool enableHitAlarm = false)
        {
            base.EnemyHit(damage, speed, direction, enableHitAlarm);

            Alpha = MAX_ALPHA;
        }

        public override void Update(GameTime gameTime)
        {
            List<BaseEntity> playerEntities = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Player);
            if (playerEntities.Count > 0)
            {
                float distance = OGE.GetDistance(playerEntities[0].Position, Position);
                if (distance < appearingRegion)
                {
                    Alpha = MAX_ALPHA;
                }
                else
                {
                    Alpha += alphaFadingSpeed;
                }
            }
            else
            {
                Alpha += alphaFadingSpeed;
            }

            base.Update(gameTime);
        }
    }
}
