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
using OmidosGameEngine.Tween;

namespace OmidosGameEngine.Entity.Player.Bullet
{
    public class SpikeBullet : PlayerBullet
    {
        protected TrailParticleGenerator trailParticleGenerator;
        protected float originalSpeed;
        protected Alarm removalAlarm;

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

        public SpikeBullet(Vector2 startingPoint, float speed, float direction, float maxDistance, float timeToRemove = 3)
            : base(startingPoint, speed, direction, maxDistance)
        {
            this.damage = 10;
            this.originalSpeed = speed;

            Particle particlePrototype = new Particle();
            particlePrototype.ParticleColor = new Color(255, 60, 50);
            particlePrototype.DeltaScale = -0.1f;
            particlePrototype.DeltaSpeed = -0.1f;

            this.trailParticleGenerator = new TrailParticleGenerator(OGE.CurrentWorld.TrailEffectSystem, particlePrototype);
            this.trailParticleGenerator.Angle = direction + 180;
            this.trailParticleGenerator.ParticleTexture = ParticleTextureType.BlurredCircle;
            this.trailParticleGenerator.Speed = 2f;
            this.trailParticleGenerator.Scale = 0f;

            this.ExplosionColor = new Color(255, 60, 50);
            this.ExplosionRadius = 20;
            this.ExplosionPower = 1;

            this.removalAlarm = new Alarm(timeToRemove, TweenType.OneShot, () => { OGE.CurrentWorld.RemoveEntity(this); });
            AddTween(removalAlarm, true);
        }

        public override void DestroyBulletCollision(BaseEntity entity)
        {
            Particle prototype = new Particle();
            prototype.DeltaScale = -0.03f;
            prototype.DeltaSpeed = -0.02f;
            prototype.DeltaAngle = 5f;

            FractionalParticleGenerator fractionalGenerator = new FractionalParticleGenerator(OGE.CurrentWorld.ExplosionEffectSystem, 
                prototype, 4);
            fractionalGenerator.Speed = 5;
            fractionalGenerator.NumberOfCircles = 1;
            fractionalGenerator.AngleDisplacement = 30;
            fractionalGenerator.FractionTexture = CurrentImages[0].Texture;
            fractionalGenerator.GenerateParticles(Position);

            base.DestroyBulletCollision(entity);
        }

        public override void DestroyBulletMaxRange()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            if (removalAlarm.PercentComplete() > 0.9)
            {
                CurrentImages[0].TintColor = Color.White * (float)(10 * (1 - removalAlarm.PercentComplete()));
            }

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
