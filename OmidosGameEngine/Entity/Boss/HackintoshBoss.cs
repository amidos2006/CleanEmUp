using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Tween;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Entity.Explosion;
using OmidosGameEngine.Entity.ParticleGenerator;
using OmidosGameEngine.Graphics.Particles;
using OmidosGameEngine.Entity.Generator;

namespace OmidosGameEngine.Entity.Boss
{
    public class HackintoshBoss:BaseBoss
    {
        private List<Type> enemyTypes;
        private CircleParticleGenerator circleGenerator;
        private HackintoshBossFanController hackintoshFanController;

        private float generatingTime = 0.75f;
        private float waitingTime = 3.5f;

        private Alarm waitAlarm;
        private Alarm generatingAlarm;

        public Color BossColor
        {
            get
            {
                return enemyColor;
            }
        }

        public HackintoshBoss()
            : base(new Vector2(OGE.CurrentWorld.Dimensions.X / 2, OGE.CurrentWorld.Dimensions.Y / 2 - 200))
        {
            this.status = BossState.Wait;

            Texture2D texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bosses\HackintoshBoss");

            this.images.Add(BossState.Wait, new Image(texture));

            foreach (Image image in this.images.Values)
            {
                image.CenterOrigin();
            }

            this.actions.Add(BossState.Wait, Wait);

            this.maxHealth = 15000;
            this.health = this.maxHealth;

            this.acceleration = 0;
            this.Direction = 90;
            this.maxSpeed = 0;
            this.speed = 0;
            this.slidingFactor = 0;
            this.freezeResistance = 1f;

            this.damage = 0;
            this.enemyColor = new Color(255, 255, 80);
            this.score = 5000;
            this.CurrentImage = images[status];

            Particle particlePrototype = new Particle();
            particlePrototype.ParticleColor = enemyColor;
            particlePrototype.DeltaScale = -0.08f;
            particlePrototype.DeltaAlpha = -0.08f;

            this.circleGenerator = new CircleParticleGenerator(OGE.CurrentWorld.TrailEffectSystem, particlePrototype);
            this.circleGenerator.AngleDisplacement = 45;
            this.circleGenerator.InterDistance = 5;
            this.circleGenerator.StartingDistance = 15;
            this.circleGenerator.NumberOfCircles = 1;
            this.circleGenerator.ParticleTexture = ParticleTextureType.BlurredCircle;
            this.circleGenerator.Speed = 4f;
            this.circleGenerator.Scale = 1f;

            this.waitAlarm = new Alarm(waitingTime, TweenType.Looping, Disappear);
            this.generatingAlarm = new Alarm(generatingTime, TweenType.Looping, Generate);
            AddTween(this.waitAlarm, true);
            AddTween(this.generatingAlarm, true);

            this.enemyTypes = new List<Type>();
            this.enemyTypes.Add(typeof(VirusEnemy));
            this.enemyTypes.Add(typeof(TroyEnemy));
            this.enemyTypes.Add(typeof(Troy2Enemy));
            this.enemyTypes.Add(typeof(DOSEnemy));
            this.enemyTypes.Add(typeof(DOS2Enemy));
            this.enemyTypes.Add(typeof(WormEnemy));
            this.enemyTypes.Add(typeof(SlowEnemy));

            this.hackintoshFanController = new HackintoshBossFanController(this, 40);
            OGE.CurrentWorld.AddEntity(hackintoshFanController);
            
            AddCollisionMask(new HitboxMask(110, 110, 55, 55));
        }

        private void Generate()
        {
            if (OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Player).Count <= 0 ||
                OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Enemy).Count > BaseGenerator.MAXIMUM_GENRATION)
            {
                return;
            }


            BaseEntity temp = Activator.CreateInstance(enemyTypes[OGE.Random.Next(enemyTypes.Count)], false) as BaseEntity;
            temp.Position = new Vector2(Position.X, Position.Y);
            OGE.CurrentWorld.AddEntity(temp);
        }

        public override void DestroyBoss()
        {
            base.DestroyBoss();
            hackintoshFanController.Destroy();
        }

        private void Disappear()
        {
            Vector2 newPosition = new Vector2();
            newPosition.X = OGE.Random.Next((int)OGE.CurrentWorld.Dimensions.X - 200) + 100;
            newPosition.Y = OGE.Random.Next((int)OGE.CurrentWorld.Dimensions.Y - 200) + 100;

            OGE.CurrentWorld.RemoveEntity(this);
            hackintoshFanController.GoToPosition(newPosition);
        }

        private void Wait()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (isHit)
            {
                circleGenerator.ParticleColor = new Color(255, 150, 150);
            }
            else
            {
                circleGenerator.ParticleColor = enemyColor;
            }

            circleGenerator.AngleDisplacement = OGE.Random.Next(15) + 30;
            circleGenerator.GenerateParticles(Position);

            waitAlarm.SpeedFactor = OGE.EnemySlowFactor;
            generatingAlarm.SpeedFactor = OGE.EnemySlowFactor;
        }
    }
}
