using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Entity.ParticleGenerator;
using OmidosGameEngine.Graphics.Particles;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Entity.Explosion;
using OmidosGameEngine.Entity.Enemy;

namespace OmidosGameEngine.Entity.Player.Bullet
{
    public class GrenadeBullet : PlayerBullet
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

        public GrenadeBullet(Vector2 startingPoint, float speed, float direction, float maxDistance)
            : base(startingPoint, speed, direction, maxDistance)
        {
            this.damage = 400;
            this.originalSpeed = speed;

            Particle particlePrototype = new Particle();
            particlePrototype.ParticleColor = new Color(255, 60, 50);
            particlePrototype.DeltaScale = -0.1f;
            particlePrototype.DeltaSpeed = -0.1f;

            this.trailParticleGenerator = new TrailParticleGenerator(OGE.CurrentWorld.TrailEffectSystem, particlePrototype);
            this.trailParticleGenerator.Angle = direction + 180;
            this.trailParticleGenerator.ParticleTexture = ParticleTextureType.BlurredCircle;
            this.trailParticleGenerator.Speed = 2f;
            this.trailParticleGenerator.Scale = 0.5f;

            this.ExplosionColor = new Color(255, 60, 50);
            this.ExplosionRadius = 180;
            this.ExplosionPower = 20;
        }

        protected override void ApplyBullet(BaseEnemy enemy)
        {
            
        }

        protected override void ApplyBullet(Boss.BaseBoss enemy)
        {

        }

        public override void DestroyBulletMaxRange()
        {
            BaseExplosion baseExplosion = new BaseExplosion(Position, ExplosionColor, ExplosionRadius);
            baseExplosion.FriendlyExplosion = true;
            baseExplosion.Damage = damage;
            baseExplosion.DamagePercentage = ExplosionPower;
            OGE.CurrentWorld.AddEntity(baseExplosion);
            base.DestroyBulletMaxRange();
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 speedVector = OGE.GetProjection(speed * speedFactor, direction);
            if (Position.X + speedVector.X > OGE.CurrentWorld.Dimensions.X || Position.X + speedVector.X < 0)
            {
                speedVector.X *= -1;
            }
            if (Position.Y + speedVector.Y > OGE.CurrentWorld.Dimensions.Y || Position.Y + speedVector.Y < 0)
            {
                speedVector.Y *= -1;
            }

            direction = OGE.GetAngle(Vector2.Zero, speedVector);

            base.Update(gameTime);

            speed = originalSpeed * distance / maxDistance + 1;
            trailParticleGenerator.GenerateParticles(Position);
        }
    }
}
