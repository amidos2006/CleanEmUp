using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Entity.Player;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Entity.ParticleGenerator;
using OmidosGameEngine.Graphics.Particles;
using OmidosGameEngine.Entity.Player.Bullet;
using OmidosGameEngine.Tween;
using OmidosGameEngine.Entity.Explosion;

namespace OmidosGameEngine.Entity.Object.File
{
    public class BaseFile:BaseEntity
    {
        protected float scale;
        protected float scaleSpeed;
        protected FractionalParticleGenerator fractionalGenerator;

        protected FileStatus status;

        protected Image normalImage;
        protected Image infectedImage;

        public BaseFile()
        {
            this.InsideScreen = true;
            this.scale = 0;
            this.scaleSpeed = 0.1f;

            Particle prototype = new Particle();
            prototype.DeltaScale = -0.03f;
            prototype.DeltaSpeed = -0.02f;
            prototype.DeltaAngle = 5f;

            this.fractionalGenerator = new FractionalParticleGenerator(OGE.CurrentWorld.ExplosionEffectSystem, prototype, 4);
            this.fractionalGenerator.Speed = 5;
            this.fractionalGenerator.NumberOfCircles = 1;

            this.status = FileStatus.Normal;
            this.EntityCollisionType = CollisionType.File;
        }

        private Image GetCurrentImage()
        {
            switch (status)
            {
                case FileStatus.Normal:
                    return normalImage;
                case FileStatus.Infected:
                    return infectedImage;
            }

            return null;
        }

        protected virtual void DestroyFile()
        {
            GenerateExplosion();

            OGE.CurrentWorld.RemoveEntity(this);
        }

        protected virtual void GenerateExplosion()
        {
            fractionalGenerator.FractionTexture = GetCurrentImage().Texture;
            fractionalGenerator.GenerateParticles(Position);

            SoundManager.EmitterPosition = Position;
            SoundManager.PlaySFX("enemy_destroy");
        }

        protected virtual void EnemyCollide(BaseEnemy e)
        {

        }

        protected virtual void PlayerCollide(PlayerEntity p)
        {
            
        }

        protected virtual void BulletCollide(PlayerBullet b)
        {
            
        }

        protected virtual void ExplosionCollision(BaseExplosion e)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            scale += scaleSpeed;
            if (scale > 1)
            {
                scale = 1;
            }

            GetCurrentImage().Scale = scale;

            BaseEntity entity = Collide(CollisionType.Player, Position);
            if(entity != null)
            {
                PlayerCollide(entity as PlayerEntity);
            }

            entity = Collide(CollisionType.Enemy, Position);
            if (entity != null)
            {
                EnemyCollide(entity as BaseEnemy);
            }

            entity = Collide(CollisionType.Explosion, Position);
            if (entity != null)
            {
                ExplosionCollision(entity as BaseExplosion);
            }

            entity = Collide(CollisionType.PlayerBullet, Position);
            if (entity != null)
            {
                BulletCollide(entity as PlayerBullet);
            }

            GetCurrentImage().Update(gameTime);
        }

        public override void Draw(Camera camera)
        {
            GetCurrentImage().Draw(Position, camera);
        }
    }
}
