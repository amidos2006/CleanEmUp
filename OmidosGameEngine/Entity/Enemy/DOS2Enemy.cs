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
    public class DOS2Enemy: BaseEnemy
    {
        public DOS2Enemy()
            :base(new Color(20,80,40))
        {
            maxSpeed = 0.5f * SPEED_UNIT;
            acceleration = 0.25f;

            direction = random.Next(360);
            rotationSpeed = 5f;

            health = 140f;
            damage = 10f;
            score = 100;

            enemyStatus = EnemyStatus.Attacking;
            
            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Enemies\DOS2")));
            CurrentImages[0].CenterOrigin();

            AddCollisionMask(new HitboxMask(CurrentImages[0].Width, CurrentImages[0].Height, 
                CurrentImages[0].OriginX, CurrentImages[0].OriginY));

            thrusters.Add(new EnemyThrusterData { Direction = 180, Length = 22 });
        }

        protected override void CheckCollisions()
        {
            PlayerEntity player = Collide(CollisionType.Player, Position) as PlayerEntity;
            if (player != null)
            {
                player.PlayerHit(damage, damagePercentage * damage, OGE.GetAngle(Position, player.Position));
                player.OverHeatWeapons();
            }
        }

        public void SmallerDOS(float health, float scale, float speed)
        {
            foreach (Image image in CurrentImages)
            {
                image.Scale = scale * image.ScaleX;
            }

            this.maxSpeed = speed;
            this.health = health;
            this.Position.X += (float)(random.NextDouble() - 0.5) * 20;
            this.Position.Y += (float)(random.NextDouble() - 0.5) * 20;
        }

        public override void EnemyHit(float damage, float speed, float direction, bool enableHitAlarm = false)
        {
            if (health < 50)
            {
                base.EnemyHit(damage, speed, direction, enableHitAlarm);
            }
            else
            {
                EnemyDestroy();
                DOS2Enemy temp = new DOS2Enemy();
                temp.Position.X = Position.X;
                temp.Position.Y = Position.Y;
                temp.SmallerDOS(0.5f * health, 0.75f, maxSpeed * 1.5f);
                OGE.CurrentWorld.AddEntity(temp);

                temp = new DOS2Enemy();
                temp.SmallerDOS(0.5f * health, 0.75f, maxSpeed * 1.5f);
                temp.Position.X = Position.X;
                temp.Position.Y = Position.Y;
                OGE.CurrentWorld.AddEntity(temp);
            }
        }
    }
}
