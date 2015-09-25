using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity;

namespace OmidosGameEngine.Collision
{
    public class HitboxMask : Mask
    {
        public Rectangle Hitbox;

        public HitboxMask(int width, int height, int originX = 0, int originY = 0)
        {
            Type = MaskType.HitBox;
            collideFunctions[MaskType.HitBox] = new CollideFunction(HitboxCollide);

            Hitbox = new Rectangle(-originX, -originY, width, height);
        }

        protected BaseEntity HitboxCollide(Vector2 parentPosition, IMask mask)
        {
            HitboxMask hitboxMask = mask as HitboxMask;

            if (Collision.HitBoxCollision(new Rectangle(Hitbox.X, Hitbox.Y, Hitbox.Width, Hitbox.Height),
                new Rectangle(hitboxMask.Hitbox.X, hitboxMask.Hitbox.Y, hitboxMask.Hitbox.Width, hitboxMask.Hitbox.Height),
                parentPosition, hitboxMask.Parent.Position))
            {
                return mask.Parent;
            }

            return null;
        }

        public override IMask Clone()
        {
            return new HitboxMask(Hitbox.Width, Hitbox.Height, -Hitbox.X, -Hitbox.Y);
        }
    }
}
