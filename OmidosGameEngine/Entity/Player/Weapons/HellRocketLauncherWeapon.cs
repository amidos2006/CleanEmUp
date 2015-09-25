﻿using System;
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
    public class HellRocketLauncherWeapon : BaseWeapon
    {
        public HellRocketLauncherWeapon() :
            base(1.5f)
        {
            numberOfBullets = 3;
            bulletSpeed = 15;
            processorTime = 40;
            accuracy = 10;
            maxDistance = 2f * OGE.WorldCamera.Width;

            GunName = GlobalVariables.Data.Weapons[16].Name;
        }

        public override void LoadContent()
        {
            texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bullets\OrangeRocket");
            baseMask = new HitboxMask(texture.Height, texture.Height, texture.Height / 2, texture.Height / 2);
            TurnetImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Players\Weapons\HellLauncher"));
            TurnetImage.OriginX = 0;
            TurnetImage.OriginY = TurnetImage.Height / 2;

            base.LoadContent();
        }

        public override bool GenerateBullet(Vector2 position, float direction, float bonusAccuracy)
        {
            bool bulletGenerated = base.GenerateBullet(position, direction, bonusAccuracy);
            HomingRocketBullet bullet;
            float currentDirection;

            if (bulletGenerated)
            {
                for (int i = 0; i < numberOfBullets; i++)
                {
                    currentDirection = (float)(direction + (i - numberOfBullets / 2) * accuracy);

                    bullet = new HomingRocketBullet(position, (float)(bulletSpeed * (1 - 0.1 * random.NextDouble())),
                        currentDirection, (float)(maxDistance * (1 - 0.1 * random.NextDouble())));
                    bullet.ProtectionDistance = 200;
                    
                    bullet.CurrentImages.Add(new Image(texture));
                    bullet.CurrentImages[0].OriginX = bullet.CurrentImages[0].Width / 2;
                    bullet.CurrentImages[0].OriginY = bullet.CurrentImages[0].Height / 2;
                    bullet.CurrentImages[0].Angle = currentDirection;
                    bullet.AddCollisionMask(baseMask.Clone());

                    OGE.CurrentWorld.AddEntity(bullet);
                }

                PlaySound("rocket_launch");
            }

            return bulletGenerated;
        }
    }
}
