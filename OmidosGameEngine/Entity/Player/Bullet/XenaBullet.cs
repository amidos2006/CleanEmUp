using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Entity.ParticleGenerator;
using OmidosGameEngine.Graphics.Particles;
using OmidosGameEngine.Sounds;

namespace OmidosGameEngine.Entity.Player.Bullet
{
    public class XenaBullet : PlayerBullet
    {
        protected TrailParticleGenerator trailParticleGenerator;

        public XenaBullet(Vector2 startingPoint, float speed, float direction, float maxDistance)
            : base(startingPoint, speed, direction, maxDistance)
        {
            this.damage = 25;

            Particle particlePrototype = new Particle();
            particlePrototype.ParticleColor = new Color(150, 255, 130);
            particlePrototype.DeltaScale = -0.005f;
            particlePrototype.DeltaAlpha = -0.01f;

            this.trailParticleGenerator = new TrailParticleGenerator(OGE.CurrentWorld.TrailEffectSystem, particlePrototype);
            this.trailParticleGenerator.Angle = direction + 180;
            this.trailParticleGenerator.AngleDisplacement = 1;
            this.trailParticleGenerator.ParticleTexture = ParticleTextureType.BlurredCircle;
            this.trailParticleGenerator.Speed = 5f;
            this.trailParticleGenerator.Scale = 0.1f;
        }

        private void JumpToNextEnemy(BaseEntity enemy)
        {
            List<BaseEntity> enemies = OGE.CurrentWorld.GetCollisionEntitiesType(Collision.CollisionType.Enemy);
            List<BaseEntity> bosses = OGE.CurrentWorld.GetCollisionEntitiesType(Collision.CollisionType.Boss);
            enemies.AddRange(bosses);
            BaseEntity nextEnemy = null;

            foreach (BaseEntity temp in enemies)
            {
                float enemyAngle = OGE.GetAngle(Position, temp.Position);
                float diffAngle = Math.Abs(direction - enemyAngle) % 360;

                if (temp == enemy)
                {
                    continue;
                }

                if (nextEnemy == null)
                {
                    nextEnemy = temp;
                }
                else if (OGE.GetDistance(Position, temp.Position) < OGE.GetDistance(Position, nextEnemy.Position))
                {
                    nextEnemy = temp;
                }
            }

            if (nextEnemy != null)
            {
                direction = OGE.GetAngle(Position, nextEnemy.Position);
            }
        }

        protected override void ApplyBullet(Enemy.BaseEnemy enemy)
        {
            enemy.EnemyHit(damage, damage * 1f, direction, true);
            SoundManager.PlaySFX("bullet_collision");

            JumpToNextEnemy(enemy);
        }

        protected override void ApplyBullet(Boss.BaseBoss enemy)
        {
            enemy.BossHit(damage, damage * 1f, direction, true);
            SoundManager.PlaySFX("bullet_collision");

            JumpToNextEnemy(enemy);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector2 speedVector = OGE.GetProjection(speed, direction);
            if (Position.X + speedVector.X > OGE.CurrentWorld.Dimensions.X || Position.X + speedVector.X < 0)
            {
                speedVector.X *= -1;
            }
            if (Position.Y + speedVector.Y > OGE.CurrentWorld.Dimensions.Y || Position.Y + speedVector.Y < 0)
            {
                speedVector.Y *= -1;
            }

            direction = OGE.GetAngle(Vector2.Zero, speedVector);

            trailParticleGenerator.Angle = 180 + direction;
            trailParticleGenerator.GenerateParticles(Position + OGE.GetProjection(CurrentImages[0].Width / 4, direction + 180));
            trailParticleGenerator.GenerateParticles(Position + OGE.GetProjection(CurrentImages[0].Width / 4, direction + 90));
            trailParticleGenerator.GenerateParticles(Position + OGE.GetProjection(CurrentImages[0].Width / 4, direction - 90));
        }
    }
}
