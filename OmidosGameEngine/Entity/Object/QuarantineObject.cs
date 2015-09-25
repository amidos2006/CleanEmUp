using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Player;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Entity.Enemy;

namespace OmidosGameEngine.Entity.Object
{
    public class QuarantineObject:BaseObject
    {
        public QuarantineObject(double timeTillRemoval)
            : base(timeTillRemoval, new Color(20, 50, 80))
        {
            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Objects\QuarantineObject")));
            CurrentImages[0].CenterOrigin();

            AddCollisionMask(new HitboxMask(CurrentImages[0].Width,CurrentImages[0].Height,CurrentImages[0].OriginX,CurrentImages[0].OriginY));
        }

        public override void ApplyBonus(PlayerEntity player)
        {
            base.ApplyBonus(player);

            FreezeEnemiesEntity fe = new FreezeEnemiesEntity(5);
            OGE.CurrentWorld.AddEntity(fe);
        }
    }
}
