using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Collision;

namespace OmidosGameEngine.Entity.Enemy
{
    public class TroyEnemy: BaseEnemy
    {
        public TroyEnemy()
            :base(new Color(80,120,20))
        {
            maxSpeed = 5 * SPEED_UNIT;
            acceleration = 0;

            direction = random.Next(360);
            rotationSpeed = 5f;

            health = 40f;
            damage = 15f;
            score = 50;

            enemyStatus = EnemyStatus.Enterance;
            
            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Enemies\Troy")));
            CurrentImages[0].CenterOrigin();

            AddCollisionMask(new HitboxMask(CurrentImages[0].Width, CurrentImages[0].Height, 
                CurrentImages[0].OriginX, CurrentImages[0].OriginY));

            thrusters.Add(new EnemyThrusterData { Direction = 180, Length = 22 });
        }

        public void GeneratedTroy()
        {
            enemyStatus = EnemyStatus.Moving;
        }

        protected override void EnteranceAI()
        {
            AttackingAI();
            if (speed <= 0)
            {
                List<BaseEntity> player = OGE.CurrentWorld.GetCollisionEntitiesType(Collision.CollisionType.Player);
                if (player.Count > 0)
                {
                    destinationDirection = OGE.GetAngle(Position, player[0].Position);
                }

                if (Math.Abs(direction - destinationDirection) < Math.Abs(rotationSpeed))
                {
                    speed = maxSpeed * SlowFactor * OGE.EnemySlowFactor;
                }
            }

            Rectangle world = new Rectangle(50, 50, (int)OGE.CurrentWorld.Dimensions.X - 50, (int)OGE.CurrentWorld.Dimensions.Y - 50);
            if (world.Contains(new Point((int)Position.X,(int)Position.Y)))
            {
                enemyStatus = EnemyStatus.Moving;
            }
        }

        protected override void MovingAI()
        {
            base.MovingAI();

            if (speed <= 0)
            {
                List<BaseEntity> player = OGE.CurrentWorld.GetCollisionEntitiesType(Collision.CollisionType.Player);
                if (player.Count > 0)
                {
                    destinationDirection = OGE.GetAngle(Position, player[0].Position);
                }

                if (Math.Abs(direction - destinationDirection) < Math.Abs(rotationSpeed))
                {
                    speed = maxSpeed * SlowFactor * OGE.EnemySlowFactor;
                }
            }
        }

        public void JumpDirection(float direction)
        {
            this.destinationDirection = direction;
            this.direction = direction;
            this.speed = maxSpeed * SlowFactor * OGE.EnemySlowFactor;
        }
    }
}
