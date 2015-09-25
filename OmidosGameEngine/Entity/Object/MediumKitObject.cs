using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Player;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Collision;

namespace OmidosGameEngine.Entity.Object
{
    public class MediumKitObject:BaseObject
    {
        public MediumKitObject(double timeTillRemoval)
            : base(timeTillRemoval, new Color(20, 50, 80))
        {
            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Objects\MediumKitObject")));
            CurrentImages[0].CenterOrigin();

            AddCollisionMask(new HitboxMask(CurrentImages[0].Width,CurrentImages[0].Height,CurrentImages[0].OriginX,CurrentImages[0].OriginY));
        }

        public override void ApplyBonus(PlayerEntity player)
        {
            base.ApplyBonus(player);
            player.IncreaseHealth(40);
        }
    }
}
