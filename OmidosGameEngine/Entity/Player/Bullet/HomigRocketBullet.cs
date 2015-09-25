using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Entity.ParticleGenerator;
using OmidosGameEngine.Graphics.Particles;
using OmidosGameEngine.Entity.Explosion;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Tween;

namespace OmidosGameEngine.Entity.Player.Bullet
{
    public class HomingRocketBullet : RocketBullet
    {
        private const float MAX_SPEED = 20;

        protected float protectionDistance;
        protected BaseEntity followingEnemy;
        protected float rotationSpeed;

        public float ProtectionDistance
        {
            set
            {
                protectionDistance = value;
            }
            get
            {
                return protectionDistance;
            }
        }

        public HomingRocketBullet(Vector2 startingPoint, float speed, float direction, float maxDistance)
            : base(startingPoint, speed, direction, maxDistance)
        {
            this.damage = 400;
            this.originalSpeed = speed;

            this.protectionDistance = 50;
            this.rotationSpeed = 5;
            this.followingEnemy = null;

            this.trailParticleGenerator.TintColor = new Color(255, 180, 50);

            this.ExplosionColor = new Color(255, 180, 50);
            this.ExplosionRadius = 180;
            this.ExplosionPower = 3;
        }

        protected override void ApplyBullet(BaseEnemy enemy)
        {
            DestroyBulletCollision(enemy);
        }

        protected override void ApplyBullet(Boss.BaseBoss enemy)
        {
            DestroyBulletCollision(enemy);
        }

        public override void DestroyBulletMaxRange()
        {
            base.DestroyBulletMaxRange();

            if (followingEnemy != null && followingEnemy is BaseEnemy)
            {
                (followingEnemy as BaseEnemy).IsFollowed = false;
            }
        }

        public override void DestroyBulletCollision(BaseEntity entity)
        {
            base.DestroyBulletCollision(entity);

            if (followingEnemy != null && followingEnemy is BaseEnemy)
            {
                (followingEnemy as BaseEnemy).IsFollowed = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (followingEnemy == null)
            {
                protectionDistance -= speed;
                if (protectionDistance <= 0)
                {
                    List<BaseEntity> enemies = OGE.CurrentWorld.GetCollisionEntitiesType(Collision.CollisionType.Boss);
                    foreach (BaseEntity enemy in enemies)
                    {
                        float enemyAngle = OGE.GetAngle(Position, enemy.Position);
                        float diffAngle = Math.Abs(direction - enemyAngle) % 360;

                        if (diffAngle > 60)
                        {
                            continue;
                        }

                        if (followingEnemy == null)
                        {
                            followingEnemy = enemy;
                        }
                        else if (OGE.GetDistance(Position, enemy.Position) < OGE.GetDistance(Position, followingEnemy.Position))
                        {
                            followingEnemy = enemy;
                        }
                    }

                    enemies = OGE.CurrentWorld.GetCollisionEntitiesType(Collision.CollisionType.Enemy);
                    foreach (BaseEntity enemy in enemies)
                    {
                        float enemyAngle = OGE.GetAngle(Position, enemy.Position);
                        float diffAngle = Math.Abs(direction - enemyAngle) % 360;

                        if (diffAngle > 60 || (enemy as BaseEnemy).IsFollowed)
                        {
                            continue;
                        }

                        if (followingEnemy == null)
                        {
                            followingEnemy = enemy;
                        }
                        else if(OGE.GetDistance(Position,enemy.Position) < OGE.GetDistance(Position,followingEnemy.Position))
                        {
                            followingEnemy = enemy;
                        }
                    }

                    if (followingEnemy != null && followingEnemy is BaseEnemy)
                    {
                        (followingEnemy as BaseEnemy).IsFollowed = true;
                    }
                }

                if (followingEnemy != null)
                {
                    direction = OGE.GetAngle(Position, followingEnemy.Position) % 360;
                }
            }

            CurrentImages[0].Angle = direction;

            base.Update(gameTime);
        }
    }
}
