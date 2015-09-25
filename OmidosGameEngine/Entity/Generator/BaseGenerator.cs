using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Tween;
using OmidosGameEngine.Entity.Player;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Entity.Enemy;

namespace OmidosGameEngine.Entity.Generator
{
    public class BaseGenerator : BaseEntity
    {
        public const int MAXIMUM_GENRATION = 200;
        private const int SAFE_RANGE = 200;

        private Alarm generatorAlarm;
        private Random random;
        private Type classType;
        private int numberOfEnemies;
        private double interGenerationTime;
        private double randomInterGenerationTime;
        private bool infinite;

        public BaseGenerator(Type classType, int numberOfEnemies, double startingTime, double interGenerationTime, double randomStartingTime = 0, double randomInterGenerationTime = 0)
        {
            this.classType = classType;
            this.numberOfEnemies = numberOfEnemies;
            this.infinite = false;
            if (numberOfEnemies <= 0)
            {
                this.numberOfEnemies = 0;
                this.infinite = true;
            }
            this.interGenerationTime = interGenerationTime;
            this.randomInterGenerationTime = randomInterGenerationTime;

            this.random = OGE.Random;
            this.generatorAlarm = new Alarm(startingTime + randomStartingTime * random.NextDouble(), TweenType.OneShot, Generate);
            AddTween(this.generatorAlarm, true);

            EntityCollisionType = Collision.CollisionType.Generator;
        }

        private bool CheckNearHackintosh(BaseEntity e, List<HackintoshEnemy> hackList)
        {
            foreach (HackintoshEnemy hack in hackList)
            {
                if (OGE.GetDistance(e.Position, hack.Position) < SAFE_RANGE)
                {
                    return true;
                }
            }

            return false;
        }

        private void ModifyPosition(BaseEntity e)
        {
            if (e.InsideScreen)
            {
                List<BaseEntity> list = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Player);
                List<BaseEntity> enemyList = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Enemy);
                List<HackintoshEnemy> hackintoshList = new List<HackintoshEnemy>();

                foreach (BaseEntity enemy in enemyList)
                {
                    if (enemy is HackintoshEnemy)
                    {
                        hackintoshList.Add(enemy as HackintoshEnemy);
                    }
                }

                if (list.Count > 0)
                {
                    PlayerEntity p = list[0] as PlayerEntity;
                    do
                    {
                        e.Position.X = random.Next((int)OGE.CurrentWorld.Dimensions.X - SAFE_RANGE) + SAFE_RANGE / 2;
                        e.Position.Y = random.Next((int)OGE.CurrentWorld.Dimensions.Y - SAFE_RANGE) + SAFE_RANGE / 2;
                    } while (OGE.GetDistance(e.Position, p.Position) < SAFE_RANGE || CheckNearHackintosh(e, hackintoshList));
                }
                else
                {
                    e.Position.X = random.Next((int)OGE.CurrentWorld.Dimensions.X - SAFE_RANGE) + SAFE_RANGE / 2;
                    e.Position.Y = random.Next((int)OGE.CurrentWorld.Dimensions.Y - SAFE_RANGE) + SAFE_RANGE / 2;
                }
            }
            else
            {
                int position = random.Next(4);

                switch (position)
                {
                    case 0:
                        e.Position.X = random.Next((int)OGE.CurrentWorld.Dimensions.X - SAFE_RANGE) + SAFE_RANGE / 2;
                        e.Position.Y = -SAFE_RANGE / 2;
                        break;
                    case 1:
                        e.Position.X = random.Next((int)OGE.CurrentWorld.Dimensions.X - SAFE_RANGE) + SAFE_RANGE / 2;
                        e.Position.Y = OGE.CurrentWorld.Dimensions.Y + SAFE_RANGE / 2;
                        break;
                    case 2:
                        e.Position.X = -SAFE_RANGE / 2;;
                        e.Position.Y = random.Next((int)OGE.CurrentWorld.Dimensions.Y - SAFE_RANGE) + SAFE_RANGE / 2;
                        break;
                    case 3:
                        e.Position.X = OGE.CurrentWorld.Dimensions.X + SAFE_RANGE / 2;
                        e.Position.Y = random.Next((int)OGE.CurrentWorld.Dimensions.Y - SAFE_RANGE) + SAFE_RANGE / 2;
                        break;
                }
            }
        }

        private void Generate()
        {
            if (OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Player).Count <= 0)
            {
                return;
            }

            List<BaseEntity> enemies = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Enemy);
            if (enemies.Count > MAXIMUM_GENRATION)
            {
                generatorAlarm.Reset(interGenerationTime + random.NextDouble() * randomInterGenerationTime);
                generatorAlarm.Start();
                return;
            }

            if (numberOfEnemies > 0 || infinite)
            {
                numberOfEnemies -= 1;
                BaseEntity temp = Activator.CreateInstance(classType, false) as BaseEntity;
                ModifyPosition(temp);
                OGE.CurrentWorld.AddEntity(temp);

                if (numberOfEnemies > 0 || infinite)
                {
                    generatorAlarm.Reset(interGenerationTime + random.NextDouble() * randomInterGenerationTime);
                    generatorAlarm.Start();
                }
                else
                {
                    OGE.CurrentWorld.RemoveEntity(this);
                    return;
                }
            }
            else
            {
                OGE.CurrentWorld.RemoveEntity(this);
                return;
            }
        }
    }
}
