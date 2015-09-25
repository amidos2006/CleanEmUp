using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Tween;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.World;
using OmidosGameEngine.Sounds;

namespace OmidosGameEngine.Entity.Boss
{
    public class DosBossController:BaseEntity
    {
        private List<DosBoss> bosses;
        private float playerAngle = 0;
        private float maxDistance = OGE.GetDistance(Vector2.Zero, OGE.CurrentWorld.Dimensions);
        private int totalNumberBosses;
        private Alarm generateAttackAlarm;
        private Color enemyColor;
        private Vector2 attackPosition;
        private bool firstAttack = true;

        public DosBossController(int totalNumberBosses = 8)
        {
            this.totalNumberBosses = totalNumberBosses;

            this.enemyColor = new Color(255, 120, 255);

            this.generateAttackAlarm = new Alarm(3f, TweenType.OneShot, Attack);
            AddTween(generateAttackAlarm, false);
            Attack();

            this.bosses = new List<DosBoss>();
        }

        public override void Intialize()
        {
            base.Intialize();

            Position = new Vector2(OGE.CurrentWorld.Dimensions.X, OGE.CurrentWorld.Dimensions.Y) / 2;

            for (int i = 0; i < totalNumberBosses; i++)
            {
                DosBoss boss = new DosBoss(this);
                boss.Position = Position + OGE.GetProjection(maxDistance, i * 360.0f / totalNumberBosses);
                boss.Direction = i * 360.0f / totalNumberBosses;
                bosses.Add(boss);

                OGE.CurrentWorld.AddEntity(boss);
            }
        }

        private void Attack()
        {
            if (firstAttack)
            {
                firstAttack = false;
                attackPosition = new Vector2(OGE.CurrentWorld.Dimensions.X, OGE.CurrentWorld.Dimensions.Y) / 2;
            }
            else
            {
                attackPosition = new Vector2(OGE.Random.Next((int)(OGE.CurrentWorld.Dimensions.X - 200) + 100),
                    (OGE.Random.Next((int)(OGE.CurrentWorld.Dimensions.Y - 200 + 100))));
            }

            List<BaseEntity> player = OGE.CurrentWorld.GetCollisionEntitiesType(Collision.CollisionType.Player);
            if(player.Count > 0)
            {
                attackPosition = player[0].Position + new Vector2(OGE.Random.Next(100) - 50, OGE.Random.Next(100) - 50);
            }

            DosBossNewPosition dosNewPostion = new DosBossNewPosition(attackPosition, enemyColor, OGE.Random.NextDouble() + 1, GenerateAttack);
            OGE.CurrentWorld.AddEntity(dosNewPostion);
        }

        public void GenerateAttack()
        {
            playerAngle = OGE.Random.Next(360);
            for (int i = 0; i < bosses.Count; i++)
            {
                bosses[i].Position =  attackPosition + OGE.GetProjection(maxDistance, playerAngle + i * 360.0f / bosses.Count);
                bosses[i].Direction = OGE.GetAngle(bosses[i].Position, attackPosition);
            }

            generateAttackAlarm.Start();
        }

        public void RemoveBoss(DosBoss boss)
        {
            OGE.CurrentWorld.RemoveEntity(boss);
            bosses.Remove(boss);

            if (bosses.Count <= 0)
            {
                SoundManager.EmitterPosition = Position;
                SoundManager.PlaySFX("explosion");

                (OGE.CurrentWorld as GameplayWorld).ClearArea();
                OGE.CurrentWorld.RemoveEntity(this);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            generateAttackAlarm.SpeedFactor = OGE.EnemySlowFactor;
        }
    }
}
