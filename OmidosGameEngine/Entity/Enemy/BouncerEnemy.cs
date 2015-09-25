using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Collision;

namespace OmidosGameEngine.Entity.Enemy
{
    public class BouncerEnemy: BaseEnemy
    {
        private float angle = 0;
        private float imageRotationSpeed = 10;

        public BouncerEnemy()
            :base(new Color(130,30,130))
        {
            maxSpeed = 3 * SPEED_UNIT;
            acceleration = 0.25f;

            direction = random.Next(360);
            angle = direction;
            rotationSpeed = 5f;

            health = 30f;
            damage = 30f;
            score = 40;

            enemyStatus = EnemyStatus.Enterance;
            
            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Enemies\Bouncer")));
            CurrentImages[0].CenterOrigin();

            AddCollisionMask(new HitboxMask(CurrentImages[0].Width, CurrentImages[0].Height, 
                CurrentImages[0].OriginX, CurrentImages[0].OriginY));

            thrusters.Add(new EnemyThrusterData { Direction = 180, Length = 0 });
            trailGenerator.Scale = 0.9f;
            trailGenerator.ParticlePrototype.DeltaAlpha /= 1.25f;
        }

        public void GeneratedBouncer()
        {
            enemyStatus = EnemyStatus.Moving;
        }

        protected override void EnteranceAI()
        {
            AttackingAI();

            Rectangle world = new Rectangle(50, 50, (int)OGE.CurrentWorld.Dimensions.X - 50, (int)OGE.CurrentWorld.Dimensions.Y - 50);
            if (world.Contains(new Point((int)Position.X,(int)Position.Y)))
            {
                enemyStatus = EnemyStatus.Moving;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            angle = (angle + imageRotationSpeed) % 360;

            foreach (Image image in CurrentImages)
            {
                image.Angle = angle;
            }
        }
    }
}
