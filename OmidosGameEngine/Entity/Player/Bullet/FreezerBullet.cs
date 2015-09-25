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
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Entity.Boss;

namespace OmidosGameEngine.Entity.Player.Bullet
{
    public class FreezerBullet : PlayerBullet
    {
        public Color TintColor
        {
            set;
            get;
        }
        public float StartingScale
        {
            set;
            get;
        }

        public Rectangle OriginalMask;

        public FreezerBullet(Vector2 startingPoint, float speed, float direction, float maxDistance)
            : base(startingPoint, speed, direction, maxDistance)
        {
            this.damage = 0.1f;
            this.TintColor = Color.White;
            this.StartingScale = 0.75f;
            this.OriginalMask = new Rectangle();
        }

        protected override void ApplyBullet(BaseEnemy enemy)
        {
            float percentage = distance / maxDistance;
            enemy.SlowFactor -= percentage * damage;
        }

        protected override void ApplyBullet(BaseBoss enemy)
        {
            float percentage = distance / maxDistance;
            enemy.SlowFactor -= percentage * damage;
        }

        public override void AddCollisionMask(IMask mask)
        {
            base.AddCollisionMask(mask);

            OriginalMask.X = (mask as HitboxMask).Hitbox.X;
            OriginalMask.Y = (mask as HitboxMask).Hitbox.Y;
            OriginalMask.Width = (mask as HitboxMask).Hitbox.Width;
            OriginalMask.Height = (mask as HitboxMask).Hitbox.Height;

            (CollisionMasks[0] as HitboxMask).Hitbox.X = (int)(StartingScale * OriginalMask.X);
            (CollisionMasks[0] as HitboxMask).Hitbox.Y = (int)(StartingScale * OriginalMask.Y);
            (CollisionMasks[0] as HitboxMask).Hitbox.Width = (int)(StartingScale * OriginalMask.Width);
            (CollisionMasks[0] as HitboxMask).Hitbox.Height = (int)(StartingScale * OriginalMask.Height);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CurrentImages[0].TintColor = TintColor * (distance / maxDistance);
            CurrentImages[0].Scale = (maxDistance - distance) / maxDistance + StartingScale;
            
            (CollisionMasks[0] as HitboxMask).Hitbox.X = (int)(CurrentImages[0].ScaleX * OriginalMask.X);
            (CollisionMasks[0] as HitboxMask).Hitbox.Y = (int)(CurrentImages[0].ScaleY * OriginalMask.Y);
            (CollisionMasks[0] as HitboxMask).Hitbox.Width = (int)(CurrentImages[0].ScaleX * OriginalMask.Width);
            (CollisionMasks[0] as HitboxMask).Hitbox.Height = (int)(CurrentImages[0].ScaleY * OriginalMask.Height);
        }
    }
}
