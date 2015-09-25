using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity;

namespace OmidosGameEngine.Collision
{
    public class PixelCollisionMask : Mask
    {
        public int CollisionIndex
        {
            set;
            get;
        }

        public PixelCollisionMask(int index = 0)
        {
            Type = MaskType.PixelCollision;
            collideFunctions[MaskType.PixelCollision] = new CollideFunction(PixelCollide);

            CollisionIndex = index;
        }

        protected BaseEntity PixelCollide(Vector2 parentPosition, IMask mask)
        {
            PixelCollisionMask pixelMask = mask as PixelCollisionMask;

            if (Collision.PerPixelCollision(Parent.CurrentImages[CollisionIndex],pixelMask.Parent.CurrentImages[pixelMask.CollisionIndex], 
                parentPosition, mask.Parent.Position))
            {
                return mask.Parent;
            }

            return null;
        }
    }
}
