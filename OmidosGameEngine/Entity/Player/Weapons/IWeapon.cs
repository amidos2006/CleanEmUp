using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Entity.Player.Weapons
{
    public interface IWeapon
    {
        void LoadContent();
        bool GenerateBullet(Vector2 position, float direction, float bonusAccuracy);
    }
}
