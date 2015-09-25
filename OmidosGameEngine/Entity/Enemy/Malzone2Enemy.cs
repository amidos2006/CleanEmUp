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
    public class Malzone2Enemy: BaseEnemy
    {
        public Malzone2Enemy()
            : base(new Color(255, 150, 150))
        {
            InsideScreen = true;

            maxSpeed = SPEED_UNIT;
            acceleration = 0f;

            direction = random.Next(0);
            rotationSpeed = 0f;

            health = 300f;
            damage = 100f;
            score = 300;

            damagePercentage = 0.8f;

            enemyStatus = EnemyStatus.Enterance;
            
            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Enemies\Malzone2")));
            CurrentImages[0].CenterOrigin();
            CurrentImages[0].Scale = 0;

            AddCollisionMask(new HitboxMask(CurrentImages[0].Width, CurrentImages[0].Height, 
                CurrentImages[0].OriginX, CurrentImages[0].OriginY));
        }

        protected override void CheckCollisions()
        {
            PlayerEntity player = Collide(CollisionType.Player, Position) as PlayerEntity;
            if (player != null)
            {
                EnemyDestroy();
            }
        }

        public override void EnemyDestroy()
        {
            base.EnemyDestroy();
            BaseExplosion baseExplosion = new BaseExplosion(Position, enemyColor, 180);
            baseExplosion.Damage = 100;
            baseExplosion.DamagePercentage = 20;
            OGE.CurrentWorld.AddEntity(baseExplosion);
        }

        protected override void EnteranceAI()
        {
            base.EnteranceAI();
            foreach (Image image in CurrentImages)
            {
                if (image.ScaleX < 1)
                {
                    image.Scale = MathHelper.Clamp(image.ScaleX + 0.05f, 0, 1);
                }
                else
                {
                    enemyStatus = EnemyStatus.Moving;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (Image image in CurrentImages)
            {
                image.Angle = 0;
            }
        }
    }
}
