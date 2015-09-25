using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Enemy;

namespace OmidosGameEngine.Entity.Boss
{
    public class HackintoshBossFanController: BaseEntity
    {
        private HackintoshBoss boss;
        private BossState status;
        private List<HackintoshBossFan> fanList;
        private float angle = 0;
        private float rotationSpeed = 5;
        private float speed = 4 * BaseEnemy.SPEED_UNIT;
        private float distance = 100;
        private Vector2 destinationPosition;
        private int fanNumber = 4;

        public HackintoshBossFanController(HackintoshBoss boss, float damage)
        {
            this.boss = boss;
            this.Position = new Vector2(boss.Position.X, boss.Position.Y);
            this.status = BossState.Wait;
            this.angle = OGE.Random.Next(360);
            this.fanList = new List<HackintoshBossFan>();

            for (int i = 0; i < fanNumber; i++)
            {
                HackintoshBossFan fan = new HackintoshBossFan(Position + 
                    OGE.GetProjection(distance, angle + i * 360.0f / fanNumber), damage);
                OGE.CurrentWorld.AddEntity(fan);
                this.fanList.Add(fan);
            }
        }

        public void Destroy()
        {
            for (int i = 0; i < fanList.Count; i++)
            {
                OGE.CurrentWorld.RemoveEntity(fanList[i]);
            }

            OGE.CurrentWorld.RemoveEntity(this);
        }

        public void GoToPosition(Vector2 newPosition)
        {
            status = BossState.Move;
            destinationPosition = newPosition;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            angle = (angle + rotationSpeed * OGE.EnemySlowFactor) % 360;
            for (int i = 0; i < fanList.Count; i++)
            {
                fanList[i].Position = Position + OGE.GetProjection(distance, angle + i * 360.0f / fanList.Count);
            }

            if (status == BossState.Move)
            {
                Position = Position + OGE.GetProjection(speed * OGE.EnemySlowFactor, OGE.GetAngle(Position, destinationPosition));
                if (OGE.GetDistance(Position, destinationPosition) < 1.5 * speed)
                {
                    status = BossState.Wait;
                    boss.Position = new Vector2(Position.X, Position.Y);
                    OGE.CurrentWorld.AddEntity(boss);
                }
            }
        }
    }
}
