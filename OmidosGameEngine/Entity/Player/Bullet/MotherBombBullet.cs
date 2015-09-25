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
    public class MotherBombBullet : PlayerBullet
    {
        protected TrailParticleGenerator trailParticleGenerator;
        protected float originalSpeed;
        protected float baseSpeed;
        protected float grenadeDistance;
        protected Texture2D texture;
        protected Mask baseMask;

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

        public MotherBombBullet(Vector2 startingPoint, float speed, float direction, float maxDistance)
            : base(startingPoint, speed, direction, maxDistance)
        {
            this.damage = 400;
            this.originalSpeed = speed;
            this.baseSpeed = 15;
            this.grenadeDistance = 0.3f * OGE.WorldCamera.Width;
            this.texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bullets\RedGrenade");
            this.baseMask = new HitboxMask(texture.Height, texture.Height, texture.Height / 2, texture.Height / 2);

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

            GrenadeBullet bullet;
            Random random = OGE.Random;
            int startingAngle = OGE.Random.Next(360);
            int numberOfGrenades = 3;

            for (int i = 0; i < numberOfGrenades; i++)
            {
                float currentDirection = (startingAngle + i * 360.0f / numberOfGrenades) + OGE.Random.Next(10) - 5;

                bullet = new GrenadeBullet(Position, (float)(baseSpeed * (1 - 0.1 * random.NextDouble())),
                        currentDirection, (float)(grenadeDistance * (1 - 0.5 * random.NextDouble())));

                bullet.CurrentImages.Add(new Image(texture));
                bullet.CurrentImages[0].OriginX = bullet.CurrentImages[0].Width / 2;
                bullet.CurrentImages[0].OriginY = bullet.CurrentImages[0].Height / 2;
                bullet.CurrentImages[0].Angle = currentDirection;
                bullet.CurrentImages[0].Scale = 0.5f;
                bullet.AddCollisionMask(baseMask.Clone());

                OGE.CurrentWorld.AddEntity(bullet);
            }

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
