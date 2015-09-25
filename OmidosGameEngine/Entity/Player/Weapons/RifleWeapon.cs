using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Player.Bullet;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Collision;

namespace OmidosGameEngine.Entity.Player.Weapons
{
    public class RifleWeapon : BaseWeapon
    {
        public RifleWeapon() :
            base(0.8f)
        {
            numberOfBullets = 1;
            bulletSpeed = 15;
            processorTime = 30;
            accuracy = 0;
            maxDistance = 0.8f * OGE.WorldCamera.Width;

            GunName = GlobalVariables.Data.Weapons[1].Name;
        }

        public override void LoadContent()
        {
            texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bullets\OrangeBullet");
            baseMask = new HitboxMask(texture.Height, texture.Height, texture.Height / 2, texture.Height / 2);
            TurnetImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Players\Weapons\Rifle"));
            TurnetImage.OriginX = 0;
            TurnetImage.OriginY = TurnetImage.Height / 2;

            base.LoadContent();
        }

        public override bool GenerateBullet(Vector2 position, float direction, float bonusAccuracy)
        {
            bool bulletGenerated = base.GenerateBullet(position, direction, bonusAccuracy);
            RifleBullet bullet;
            float currentDirection;

            if (bulletGenerated)
            {
                for (int i = 0; i < numberOfBullets; i++)
                {
                    currentDirection = (float)(direction + (accuracy + bonusAccuracy) * (random.NextDouble() - 0.5));

                    bullet = new RifleBullet(position, (float)(bulletSpeed * (1 - 0.1 * random.NextDouble())),
                        currentDirection, (float)(maxDistance * (1 - 0.1 * random.NextDouble())));
                    
                    bullet.CurrentImages.Add(new Image(texture));
                    bullet.CurrentImages[0].OriginX = bullet.CurrentImages[0].Width / 2;
                    bullet.CurrentImages[0].OriginY = bullet.CurrentImages[0].Height / 2;
                    bullet.CurrentImages[0].Angle = currentDirection;
                    bullet.CurrentImages[0].Scale = 0.5f;
                    bullet.AddCollisionMask(baseMask.Clone());

                    OGE.CurrentWorld.AddEntity(bullet);
                }

                PlaySound("rifle");
                OGE.WorldCamera.ShackCamera(2, 0.1f);
            }

            return bulletGenerated;
        }
    }
}
