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
    public class BulletGrenadeBullet : PlayerBullet
    {
        protected TrailParticleGenerator trailParticleGenerator;
        protected float originalSpeed;
        protected Texture2D texture;
        protected Mask baseMask;
        protected float bulletSpeed;

        public BulletGrenadeBullet(Vector2 startingPoint, float speed, float direction, float maxDistance)
            : base(startingPoint, speed, direction, maxDistance)
        {
            this.damage = 0;
            this.originalSpeed = speed;

            Particle particlePrototype = new Particle();
            particlePrototype.ParticleColor = new Color(255, 250, 50);
            particlePrototype.DeltaScale = -0.1f;
            particlePrototype.DeltaSpeed = -0.1f;

            this.trailParticleGenerator = new TrailParticleGenerator(OGE.CurrentWorld.TrailEffectSystem, particlePrototype);
            this.trailParticleGenerator.Angle = direction + 180;
            this.trailParticleGenerator.ParticleTexture = ParticleTextureType.BlurredCircle;
            this.trailParticleGenerator.Speed = 2f;
            this.trailParticleGenerator.Scale = 0.5f;

            this.texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bullets\YellowBullet");
            this.baseMask = new HitboxMask(texture.Height, texture.Height, texture.Height / 2, texture.Height / 2);
            this.bulletSpeed = 15;
        }

        protected override void ApplyBullet(BaseEnemy enemy)
        {
            
        }

        protected override void ApplyBullet(Boss.BaseBoss enemy)
        {

        }

        public override void DestroyBulletMaxRange()
        {
            PistolBullet bullet;
            Random random = OGE.Random;

            for (int i = 0; i < 360; i += 360 / 15)
            {
                float currentDirection = i;

                bullet = new PistolBullet(Position, bulletSpeed, currentDirection, (float)(maxDistance * (1 - 0.1 * random.NextDouble())));

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
