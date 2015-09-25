using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Collision;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Entity.Boss;

namespace OmidosGameEngine.Entity.Player.Bullet
{
    public class PlayerBullet : BaseBullet
    {
        public PlayerBullet(Vector2 startingPoint, float speed, float direction, float maxDistance)
            : base(startingPoint, speed, direction, maxDistance)
        {
            EntityCollisionType = CollisionType.PlayerBullet;
        }

        protected virtual void ApplyBullet(BaseEnemy enemy)
        {
            enemy.EnemyHit(damage, damage * 0.1f, direction);
            SoundManager.EmitterPosition = enemy.Position;
            SoundManager.PlaySFX("bullet_collision");
            DestroyBulletCollision(enemy);
        }

        protected virtual void ApplyBullet(BaseBoss enemy)
        {
            enemy.BossHit(damage, damage * 0.1f, direction);
            SoundManager.EmitterPosition = enemy.Position;
            SoundManager.PlaySFX("bullet_collision");
            DestroyBulletCollision(enemy);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            speedFactor = OGE.PlayerSlowFactor;

            BaseEnemy enemy = Collide(CollisionType.Enemy, Position) as BaseEnemy;
            if (enemy != null)
            {
                ApplyBullet(enemy);
            }

            BaseBoss boss = Collide(CollisionType.Boss, Position) as BaseBoss;
            if (boss != null)
            {
                ApplyBullet(boss);
            }
        }
    }
}
