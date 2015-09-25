using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Entity.Player;
using OmidosGameEngine.Entity.Explosion;

namespace OmidosGameEngine.Entity.Enemy
{
    public class VirusEnemy: BaseEnemy
    {
        private bool isGenerated;

        public float DestinationDirection
        {
            set
            {
                destinationDirection = value;
            }
        }

        public VirusEnemy()
            :base(new Color(190,100,20))
        {
            maxSpeed = 1 * SPEED_UNIT;
            acceleration = 0.25f;

            direction = random.Next(360);
            rotationSpeed = 5f;

            health = 30f;
            damage = 20f;
            score = 20;

            isGenerated = false;
            enemyStatus = EnemyStatus.Enterance;
            
            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Enemies\Virus")));
            CurrentImages[0].CenterOrigin();

            AddCollisionMask(new HitboxMask(CurrentImages[0].Width, CurrentImages[0].Height, 
                CurrentImages[0].OriginX, CurrentImages[0].OriginY));

            thrusters.Add(new EnemyThrusterData { Direction = 180, Length = 22 });
        }

        public void GeneratedVirus()
        {
            isGenerated = true;
            foreach (Image image in CurrentImages)
            {
                image.Scale = 0;
            }
        }

        protected override void EnteranceAI()
        {
            base.EnteranceAI();

            if (isGenerated)
            {
                foreach (Image image in CurrentImages)
                {
                    if (image.ScaleX < 1)
                    {
                        image.Scale = MathHelper.Clamp(image.ScaleX + 0.05f, 0, 1);
                    }
                    else
                    {
                        enemyStatus = EnemyStatus.Attacking;
                    }
                }
            }
            else
            {
                enemyStatus = EnemyStatus.Attacking;
            }
        }
    }
}
