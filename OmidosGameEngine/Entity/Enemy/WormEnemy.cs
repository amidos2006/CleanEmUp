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
using OmidosGameEngine.Tween;
using OmidosGameEngine.Entity.Generator;

namespace OmidosGameEngine.Entity.Enemy
{
    public class WormEnemy: BaseEnemy
    {
        private Alarm replicateAlarm;
        private double alarmTime;

        public WormEnemy()
            :base(new Color(70,20,90))
        {
            maxSpeed = 1 * SPEED_UNIT;
            acceleration = 0.25f;

            direction = random.Next(360);
            rotationSpeed = 5f;

            health = 30f;
            damage = 10f;
            score = 30;

            enemyStatus = EnemyStatus.Attacking;

            alarmTime = 2f + 2 * (float)random.NextDouble();
            replicateAlarm = new Alarm(alarmTime, TweenType.OneShot, new AlarmFinished(SlowDown));
            AddTween(replicateAlarm, true);
            
            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Enemies\Worm")));
            CurrentImages[0].CenterOrigin();

            AddCollisionMask(new HitboxMask(CurrentImages[0].Width, CurrentImages[0].Height, 
                CurrentImages[0].OriginX, CurrentImages[0].OriginY));

            thrusters.Add(new EnemyThrusterData { Direction = 180, Length = 22 });
        }

        public void ReplicatePosition(float health, float randomAmount, double alarmAmount)
        {
            this.health = health;
            this.Position.X += (float)((randomAmount - 0.5) * 100);
            this.Position.Y += (float)((randomAmount - 0.5) * 100);

            RemoveTween(replicateAlarm);
            this.alarmTime = alarmAmount + alarmAmount * randomAmount;
            this.replicateAlarm = new Alarm(alarmTime, TweenType.OneShot, new AlarmFinished(SlowDown));
            AddTween(replicateAlarm, true);
        }

        private void SlowDown()
        {
            List<BaseEntity> enemies = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Enemy);

            if (enemies.Count < BaseGenerator.MAXIMUM_GENRATION)
            {
                acceleration = 0;
            }
            else
            {
                replicateAlarm.Start(true);
            }
        }

        private void ReturnScoreRage()
        {
            GlobalVariables.LevelScore -= score;
            GlobalVariables.Achievements[this.GetType()].CurrentNumber -= 1;

            List<BaseEntity> player = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Player);
            if (player.Count > 0)
            {
                (player[0] as PlayerEntity).Rage -= AddedRage;
            }
        }

        private void Replicate()
        {
            IList<BaseEntity> entityList = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Player);

            if (entityList.Count == 0)
            {
                acceleration = 0.25f;
                return;
            }

            EnemyDestroy();
            ReturnScoreRage();
            WormEnemy temp = new WormEnemy();
            temp.Position.X = Position.X;
            temp.Position.Y = Position.Y;
            temp.ReplicatePosition(health, (float)random.NextDouble(), alarmTime);
            OGE.CurrentWorld.AddEntity(temp);

            temp = new WormEnemy();
            temp.Position.X = Position.X;
            temp.Position.Y = Position.Y;
            temp.ReplicatePosition(health, (float)random.NextDouble(), alarmTime);
            OGE.CurrentWorld.AddEntity(temp);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (speed <= 0 && !replicateAlarm.IsRunning())
            {
                Replicate();
            }

            replicateAlarm.SpeedFactor = OGE.EnemySlowFactor;
        }
    }
}
