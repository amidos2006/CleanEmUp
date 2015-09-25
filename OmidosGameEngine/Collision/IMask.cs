using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Entity;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Collision
{
    delegate BaseEntity CollideFunction(Vector2 parentPosition, IMask mask);

    public interface IMask
    {
        BaseEntity Parent
        {
            set;
            get;
        }

        MaskType Type
        {
            set;
            get;
        }

        BaseEntity Collide(Vector2 parentPosition, IMask mask);
        IMask Clone();
    }
}
