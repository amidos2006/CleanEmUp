using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Graphics;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Entity.Player;

namespace OmidosGameEngine.Entity.Enemy
{
    public class SlowEnemy : BaseEnemy
    {
        private float slowRate;

        public SlowEnemy()
            : base(new Color(130, 80, 30))
        {
            maxSpeed = 0.5f * SPEED_UNIT;
            acceleration = 0.25f;

            direction = random.Next(360);
            rotationSpeed = 5f;

            health = 40f;
            damage = 10f;
            score = 120;

            slowRate = -0.25f;

            enemyStatus = EnemyStatus.Attacking;

            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Enemies\Slower")));
            CurrentImages[0].CenterOrigin();

            AddCollisionMask(new HitboxMask(CurrentImages[0].Width, CurrentImages[0].Height,
                CurrentImages[0].OriginX, CurrentImages[0].OriginY));

            thrusters.Add(new EnemyThrusterData { Direction = 180, Length = 22 });
        }

        protected override void CheckCollisions()
        {
            base.CheckCollisions();

            PlayerEntity player = Collide(CollisionType.Player, Position) as PlayerEntity;
            if (player != null)
            {
                player.SpeedFactor += slowRate;
            }
        }
    }
}
