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

namespace OmidosGameEngine.Entity.Player.Bullet
{
    public class RocketBullet : PlayerBullet
    {
        protected TrailParticleGenerator trailParticleGenerator;
        protected float originalSpeed;

        public Color ExplosionColor
        {
            set;
            get;
        }

        public float ExplosionRadius
        {
            set;
            get;
        }

        public float ExplosionPower
        {
            set;
            get;
        }

        public RocketBullet(Vector2 startingPoint, float speed, float direction, float maxDistance)
            : base(startingPoint, speed, direction, maxDistance)
        {
            this.damage = 400;
            this.originalSpeed = speed;

            Particle particlePrototype = new Particle();
            particlePrototype.ParticleColor = new Color(255, 60, 50);
            particlePrototype.DeltaScale = -0.06f;
            particlePrototype.DeltaAlpha = -0.1f;

            this.trailParticleGenerator = new TrailParticleGenerator(OGE.CurrentWorld.TrailEffectSystem, particlePrototype);
            this.trailParticleGenerator.Angle = direction + 180;
            this.trailParticleGenerator.AngleDisplacement = 15;
            this.trailParticleGenerator.Speed = 2;
            this.trailParticleGenerator.Scale = 0.5f;
            this.trailParticleGenerator.ParticleTexture = ParticleTextureType.BlurredCircle;

            this.ExplosionColor = new Color(255, 60, 50);
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

        public override void DestroyBulletCollision(BaseEntity entity)
        {
            BaseExplosion baseExplosion = new BaseExplosion(Position, ExplosionColor, ExplosionRadius);
            baseExplosion.FriendlyExplosion = true;
            baseExplosion.Damage = damage;
            baseExplosion.DamagePercentage = ExplosionPower;
            OGE.CurrentWorld.AddEntity(baseExplosion);

            base.DestroyBulletCollision(entity);
        }

        public override void DestroyBulletOutsideScreen()
        {
            BaseExplosion baseExplosion = new BaseExplosion(Position, ExplosionColor, ExplosionRadius);
            baseExplosion.FriendlyExplosion = true;
            baseExplosion.Damage = damage;
            baseExplosion.DamagePercentage = ExplosionPower;
            OGE.CurrentWorld.AddEntity(baseExplosion);

            base.DestroyBulletOutsideScreen();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            speed = originalSpeed * (5 * (maxDistance - distance) / maxDistance + 0.1f);
            trailParticleGenerator.GenerateParticles(Position + OGE.GetProjection(CurrentImages[0].Width, direction + 180));
        }
    }
}
