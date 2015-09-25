using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Tween;
using OmidosGameEngine.Collision;

namespace OmidosGameEngine.Entity
{
    public class BaseBullet : BaseEntity
    {
        protected float damage;

        protected float speed;
        protected float direction;

        protected float distance;
        protected float maxDistance;
        protected float speedFactor;

        protected Vector2 startingPoint;

        public float BonusDamage
        {
            set;
            get;
        }

        public float Damage
        {
            get
            {
                return damage;
            }
        }

        public BaseBullet(Vector2 startingPoint, float speed, float direction, float maxDistance)
        {
            this.speed = speed;
            this.direction = direction;

            this.distance = maxDistance;
            this.maxDistance = maxDistance;

            this.speedFactor = 1;

            this.startingPoint = new Vector2(startingPoint.X, startingPoint.Y);
            this.Position = new Vector2(startingPoint.X, startingPoint.Y);
        }

        public virtual void DestroyBulletOutsideScreen()
        {
            OGE.CurrentWorld.RemoveEntity(this);
        }

        public virtual void DestroyBulletMaxRange()
        {
            OGE.CurrentWorld.RemoveEntity(this);
        }

        public virtual void DestroyBulletCollision(BaseEntity entity)
        {
            OGE.CurrentWorld.RemoveEntity(this);
        }

        public override void Update(GameTime gameTime)
        {
            distance -= speed * speedFactor;
            if (distance <= 0)
            {
                DestroyBulletMaxRange();
            }

            if (Position.X > OGE.CurrentWorld.Dimensions.X || Position.X < 0)
            {
                DestroyBulletOutsideScreen();
            }
            if (Position.Y > OGE.CurrentWorld.Dimensions.Y || Position.Y < 0)
            {
                DestroyBulletOutsideScreen();
            }

            Vector2 speedVector = OGE.GetProjection(speed * speedFactor, direction);

            Position.X += speedVector.X;
            Position.Y += speedVector.Y;

            base.Update(gameTime);
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);
        }
    }
}
