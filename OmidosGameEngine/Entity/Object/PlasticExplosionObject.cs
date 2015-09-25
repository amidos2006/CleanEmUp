using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Player;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Entity.Explosion;

namespace OmidosGameEngine.Entity.Object
{
    public class PlasticExplosionObject : BaseObject
    {
        public PlasticExplosionObject(double timeTillRemoval)
            : base(timeTillRemoval, new Color(20, 50, 80))
        {
            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Objects\PlasticExplosionObject")));
            CurrentImages[0].CenterOrigin();

            AddCollisionMask(new HitboxMask(CurrentImages[0].Width,CurrentImages[0].Height,CurrentImages[0].OriginX,CurrentImages[0].OriginY));
        }

        public override void ApplyBonus(PlayerEntity player)
        {
            base.ApplyBonus(player);

            BaseExplosion explosion = new BaseExplosion(Position, new Color(20, 80, 140), 300);
            explosion.FriendlyExplosion = true;
            explosion.Damage = 100;
            explosion.DamagePercentage = 0.1f;
            OGE.CurrentWorld.AddEntity(explosion);
        }
    }
}
