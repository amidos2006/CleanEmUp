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
    public class PopurEnemy: BaseEnemy
    {
        public PopurEnemy()
            :base(new Color(255,150,150))
        {
            maxSpeed = 0.5f * SPEED_UNIT;
            acceleration = 0.25f;

            direction = random.Next(360);
            rotationSpeed = 5f;

            health = 20f;
            damage = 0f;
            score = 100;

            enemyStatus = EnemyStatus.Attacking;
            
            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Enemies\Popur")));
            CurrentImages[0].CenterOrigin();

            AddCollisionMask(new HitboxMask(CurrentImages[0].Width, CurrentImages[0].Height, 
                CurrentImages[0].OriginX, CurrentImages[0].OriginY));

            thrusters.Add(new EnemyThrusterData { Direction = 180, Length = 22 });
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
    }
}
