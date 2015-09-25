using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Tween;

namespace OmidosGameEngine.Entity.Object
{
    public class FreezeEnemiesEntity : BaseEntity
    {
        private List<BaseEnemy> enemyList;
        private Alarm removalAlarm;

        public FreezeEnemiesEntity(double freezingTime)
        {
            enemyList = new List<BaseEnemy>();

            List<BaseEntity> list = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Enemy);
            foreach (BaseEntity entity in list)
            {
                BaseEnemy enemy = entity as BaseEnemy;
                if (enemy != null)
                {
                    enemyList.Add(enemy);
                }
            }

            removalAlarm = new Alarm(freezingTime, TweenType.OneShot, new AlarmFinished(RemoveEntity));
            AddTween(removalAlarm, true);

            EntityCollisionType = CollisionType.Object;
        }

        private void RemoveEntity()
        {
            OGE.CurrentWorld.RemoveEntity(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (BaseEnemy enemy in enemyList)
            {
                enemy.SlowFactor = 0;
            }

            removalAlarm.SpeedFactor = OGE.PlayerSlowFactor;
        }
    }
}
