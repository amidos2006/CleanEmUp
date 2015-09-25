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
using OmidosGameEngine.Entity.ParticleGenerator;
using OmidosGameEngine.Graphics.Particles;
using OmidosGameEngine.Entity.Generator;

namespace OmidosGameEngine.Entity.Enemy
{
    public class HackintoshEnemy: BaseEnemy
    {
        private Alarm generateAlarm;
        private CircleParticleGenerator circleGenerator;

        public HackintoshEnemy()
            :base(new Color(50,50,50))
        {
            InsideScreen = true;

            maxSpeed = SPEED_UNIT;
            acceleration = 0f;

            direction = random.Next(0);
            rotationSpeed = 0f;

            health = 300f;
            damage = 0f;
            score = 500;

            enemyStatus = EnemyStatus.Enterance;
            
            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Enemies\Hackintosh")));
            CurrentImages[0].CenterOrigin();
            CurrentImages[0].Scale = 0;

            AddCollisionMask(new HitboxMask(CurrentImages[0].Width, CurrentImages[0].Height, 
                CurrentImages[0].OriginX, CurrentImages[0].OriginY));

            Particle particlePrototype = new Particle();
            particlePrototype.ParticleColor = enemyColor;
            particlePrototype.DeltaScale = -0.08f;
            particlePrototype.DeltaAlpha = -0.08f;

            this.circleGenerator = new CircleParticleGenerator(OGE.CurrentWorld.TrailEffectSystem, particlePrototype);
            this.circleGenerator.AngleDisplacement = 45;
            this.circleGenerator.InterDistance = 5;
            this.circleGenerator.StartingDistance = 5;
            this.circleGenerator.NumberOfCircles = 1;
            this.circleGenerator.ParticleTexture = ParticleTextureType.BlurredCircle;
            this.circleGenerator.Speed = 4f;
            this.circleGenerator.Scale = 0.1f;

            this.generateAlarm = new Alarm(3f, TweenType.Looping, new AlarmFinished(GenerateVirus));
            AddTween(generateAlarm, true);
        }

        protected override void EnteranceAI()
        {
            base.EnteranceAI();
            foreach (Image image in CurrentImages)
            {
                if (image.ScaleX < 1)
                {
                    image.Scale = MathHelper.Clamp(image.ScaleX + 0.05f, 0, 1);
                    circleGenerator.Scale = image.ScaleX;
                }
                else
                {
                    enemyStatus = EnemyStatus.Standing;
                }
            }
        }

        private void GenerateVirus()
        {
            if (OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Player).Count <= 0 || 
                OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Enemy).Count > BaseGenerator.MAXIMUM_GENRATION)
            {
                return;
            }

            VirusEnemy temp = new VirusEnemy();
            temp.Position = Position;
            temp.GeneratedVirus();
            OGE.CurrentWorld.AddEntity(temp);
        }

        public override void EnemyHit(float damage, float speed, float direction, bool enableHitAlarm = false)
        {
            base.EnemyHit(damage, speed, direction, enableHitAlarm);
            this.direction = 0;
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

            circleGenerator.AngleDisplacement = random.Next(15) + 30;
            circleGenerator.GenerateParticles(Position);

            generateAlarm.SpeedFactor = OGE.EnemySlowFactor;
        }

        public override void Draw(Camera camera)
        {
            //base.Draw(camera);
        }
    }
}
