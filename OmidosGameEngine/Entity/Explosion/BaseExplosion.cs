using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Tween;
using OmidosGameEngine.Entity.ParticleGenerator;
using OmidosGameEngine.Graphics.Particles;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Entity.Player;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Entity.Boss;

namespace OmidosGameEngine.Entity.Explosion
{
    public class BaseExplosion : BaseEntity
    {
        protected Alarm removalAlarm;
        protected CircleParticleGenerator circleGenerator;
        protected float radius;

        public float Damage
        {
            set;
            get;
        }

        public float DamagePercentage
        {
            set;
            get;
        }

        public float AdditiveWhite
        {
            set;
            get;
        }

        public bool FriendlyExplosion
        {
            set;
            get;
        }

        public BaseExplosion(Vector2 position, Color explosionColor, float radius)
        {
            this.Position = position;
            this.radius = radius;
            this.AdditiveWhite = 0.2f;
            this.FriendlyExplosion = false;

            Particle particlePrototype = new Particle();
            particlePrototype.ParticleColor = explosionColor;
            particlePrototype.DeltaScale =  0.03f;
            particlePrototype.DeltaAlpha = -0.03f;

            this.circleGenerator = new CircleParticleGenerator(OGE.CurrentWorld.ExplosionEffectSystem, particlePrototype);
            this.circleGenerator.AngleDisplacement = 20;
            this.circleGenerator.InterDistance = 10;
            this.circleGenerator.NumberOfCircles = (int)MathHelper.Clamp((radius / 100), 1, 4);
            this.circleGenerator.ParticleTexture = ParticleTextureType.BlurredCircle;
            this.circleGenerator.Speed = 4f;

            this.circleGenerator.GenerateParticles(Position);
            OGE.CurrentWorld.LightingEffectSystem.GenerateLightSource(position, new Vector2(2 * radius, 2 * radius), explosionColor, -0.01f);
            OGE.CurrentWorld.AdditiveWhite.Alpha += AdditiveWhite;
            OGE.WorldCamera.ShackCamera(10, 0.25f);

            this.EntityCollisionType = CollisionType.Explosion;

            SoundManager.EmitterPosition = Position;
            SoundManager.PlaySFX("explosion");

            this.removalAlarm = new Alarm(0.08f, TweenType.OneShot, new AlarmFinished(RemoveEntity));
            AddTween(removalAlarm, true);
        }

        private void RemoveEntity()
        {
            OGE.CurrentWorld.RemoveEntity(this);
        }

        public float GetDamageAccordingToPosition(Vector2 position)
        {
            float distance = OGE.GetDistance(position, Position);
            float percentage = MathHelper.Clamp((radius - distance) / radius, 0, 1);

            return Damage * percentage;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            List<BaseEntity> entities = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Player);
            if (!FriendlyExplosion)
            {
                foreach (BaseEntity entity in entities)
                {
                    PlayerEntity player = entity as PlayerEntity;
                    float distance = OGE.GetDistance(entity.Position, Position);
                    if (distance - Math.Max(player.CurrentImages[0].Width, player.CurrentImages[0].Height) <= radius)
                    {
                        float percentage = MathHelper.Clamp((radius - distance) / radius, 0, 1);
                        player.PlayerHit(percentage * Damage, percentage * Damage * DamagePercentage, OGE.GetAngle(Position, player.Position));
                    }
                }
            }

            entities = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Enemy);
            foreach (BaseEntity entity in entities)
            {
                if (OGE.GetDistance(entity.Position, Position) <= radius)
                {
                    BaseEnemy enemy = entity as BaseEnemy;
                    float distance = OGE.GetDistance(entity.Position, Position);
                    if (distance - Math.Max(enemy.CurrentImages[0].Width, enemy.CurrentImages[0].Height) <= radius)
                    {
                        float percentage = MathHelper.Clamp((radius - distance) / radius, 0, 1);
                        enemy.EnemyHit(percentage * Damage, percentage * Damage * DamagePercentage, 
                            OGE.GetAngle(Position, enemy.Position), true);
                    }
                }
            }

            entities = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Boss);
            foreach (BaseEntity entity in entities)
            {
                if (OGE.GetDistance(entity.Position, Position) <= radius)
                {
                    BaseBoss enemy = entity as BaseBoss;
                    float distance = OGE.GetDistance(entity.Position, Position);
                    if (distance - Math.Max(enemy.CurrentImage.Width, enemy.CurrentImage.Height) <= radius)
                    {
                        float percentage = MathHelper.Clamp((radius - distance) / radius, 0, 1);
                        enemy.BossHit(percentage * Damage, percentage * Damage * DamagePercentage, 
                            OGE.GetAngle(Position, enemy.Position), true);
                    }
                }
            }
        }
    }
}
