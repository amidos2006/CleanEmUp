using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Entity;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Collision
{
    public class Mask : IMask
    {
        internal Dictionary<MaskType, CollideFunction> collideFunctions;

        public BaseEntity Parent
        {
            get;
            set;
        }

        public MaskType Type
        {
            get;
            set;
        }
        
        public Mask()
        {
            collideFunctions = new Dictionary<MaskType, CollideFunction>();
        }

        public BaseEntity Collide(Vector2 parentPosition, IMask mask)
        {
            return collideFunctions[mask.Type](parentPosition, mask);
        }


        public virtual IMask Clone()
        {
            Mask clonedMask = new Mask();
            clonedMask.Type = Type;

            return clonedMask;
        }
    }
}
