using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Tween;
using OmidosGameEngine.Entity.ParticleGenerator;
using OmidosGameEngine.Entity.Player;
using OmidosGameEngine.Graphics.Particles;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Entity.Object;

namespace OmidosGameEngine.Entity.Enemy
{
    public delegate void AIEnemyFunction();

    public class BaseEnemy : BaseEntity
    {
        private const float RAGE_FACTOR = 0.125f;
        private const float RAGE_CONST_INC = 2.5f;
        public const float SPEED_UNIT = 2f;

        protected TrailParticleGenerator trailGenerator;
        protected FractionalParticleGenerator fractionalGenerator;

        protected Random random;

        protected bool isHit;
        private Alarm hitAlarm;
        private Image freezeImage;

        protected float speed;
        protected float acceleration;
        protected float maxSpeed;
        protected float direction;
        protected float destinationDirection;
        protected float rotationSpeed;
        protected List<EnemyThrusterData> thrusters;

        protected float health;
        protected float damage;
        protected float damagePercentage;
        protected Color enemyColor;
        protected float freezeResistance;
        protected float freezeReviving;
        protected int score;
        protected float lightingPercent;

        protected Dictionary<EnemyStatus, AIEnemyFunction> aiFunctions;
        protected EnemyStatus enemyStatus;

        public float SlowFactor
        {
            set;
            get;
        }

        public bool IsFollowed
        {
            set;
            get;
        }

        public float MaxSpeed
        {
            set
            {
                maxSpeed = value;
            }
            get
            {
                return maxSpeed;
            }
        }

        public float Health
        {
            set
            {
                health = value;
            }
            get
            {
                return health;
            }
        }

        public Color MainColor
        {
            set;
            get;
        }

        public float AddedRage
        {
            get
            {
                return RAGE_CONST_INC;
            }
        }

        public float Damage
        {
            get
            {
                return damage;
            }
        }

        private void IntializeParticleGenerators()
        {
            Particle particlePrototype = new Particle();
            particlePrototype.ParticleColor = enemyColor;
            particlePrototype.DeltaScale = -0.06f;
            particlePrototype.DeltaAlpha = -0.1f;

            this.trailGenerator = new TrailParticleGenerator(OGE.CurrentWorld.TrailEffectSystem, particlePrototype);
            this.trailGenerator.AngleDisplacement = 15;
            this.trailGenerator.Speed = 2;
            this.trailGenerator.Scale = 0.5f;

            Particle prototype = new Particle();
            prototype.DeltaScale = -0.03f;
            prototype.DeltaSpeed = -0.02f;
            prototype.DeltaAngle = 5f;

            this.fractionalGenerator = new FractionalParticleGenerator(OGE.CurrentWorld.ExplosionEffectSystem, prototype, 4);
            this.fractionalGenerator.Speed = 5;
            this.fractionalGenerator.NumberOfCircles = 1;
        }

        public BaseEnemy(Color enemyColor)
        {
            this.IsFollowed = false;

            this.enemyColor = enemyColor;
            this.MainColor = Color.White;
            this.speed = 0;
            this.damagePercentage = 0.2f;
            this.freezeResistance = 0.1f;
            this.freezeReviving = 0.005f;
            this.lightingPercent = 1f;

            this.random = OGE.Random;

            IntializeParticleGenerators();

            this.isHit = false;
            this.hitAlarm = new Alarm(0.2f, TweenType.OneShot, new AlarmFinished(HitEnded));
            AddTween(hitAlarm);

            this.enemyStatus = EnemyStatus.Standing;
            this.aiFunctions = new Dictionary<EnemyStatus, AIEnemyFunction>();
            this.aiFunctions.Add(EnemyStatus.Enterance, new AIEnemyFunction(EnteranceAI));
            this.aiFunctions.Add(EnemyStatus.Standing, new AIEnemyFunction(StandingAI));
            this.aiFunctions.Add(EnemyStatus.Moving, new AIEnemyFunction(MovingAI));
            this.aiFunctions.Add(EnemyStatus.Attacking, new AIEnemyFunction(AttackingAI));

            this.thrusters = new List<EnemyThrusterData>();
            this.SlowFactor = 1;

            this.freezeImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Enemies\Freeze"));
            this.freezeImage.CenterOrigin();
            this.freezeImage.Angle = random.Next(360);

            this.EntityCollisionType = CollisionType.Enemy;
        }

        private void HitEnded()
        {
            isHit = false;
        }

        protected virtual void DropBonus()
        {
            int select = random.Next(5);
            BaseObject o;
            double time = BaseObject.GetTime(random);

            switch (select)
            {
                case 0:
                    o = new MediumKitObject(time);
                    o.Position = Position;
                    OGE.CurrentWorld.AddEntity(o);
                    break;
                case 1:
                    o = new MosiacObject(time);
                    o.Position = Position;
                    OGE.CurrentWorld.AddEntity(o);
                    break;
                case 2:
                    o = new QuarantineObject(time);
                    o.Position = Position;
                    OGE.CurrentWorld.AddEntity(o);
                    break;
                case 3:
                    o = new PlasticExplosionObject(time);
                    o.Position = Position;
                    OGE.CurrentWorld.AddEntity(o);
                    break;
                case 4:
                    o = new ShieldObject(time);
                    o.Position = Position;
                    OGE.CurrentWorld.AddEntity(o);
                    break;
            }
        }

        public virtual void EnemyDestroy()
        {
            fractionalGenerator.FractionTexture = CurrentImages[0].Texture;
            fractionalGenerator.GenerateParticles(Position);

            GlobalVariables.Achievements[this.GetType()].CurrentNumber += 1;

            SoundManager.EmitterPosition = Position;
            SoundManager.PlaySFX("enemy_destroy");

            if (random.NextDouble() < 0.05)
            {
                DropBonus();
            }

            OGE.CurrentWorld.AdditiveWhite.Alpha += 0.075f;
            OGE.CurrentWorld.LightingEffectSystem.GenerateLightSource(Position, new Vector2(200, 200), enemyColor, -0.005f);

            OGE.CurrentWorld.RemoveEntity(this);

            GlobalVariables.LevelScore += score;

            List<BaseEntity> player = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Player);
            if (player.Count > 0)
            {
                (player[0] as PlayerEntity).Rage += AddedRage;
            }
        }

        public virtual void EnemyHit(float damage, float speed, float direction, bool enableHitAlarm = false)
        {
            if (isHit && enableHitAlarm)
            {
                return;
            }

            isHit = true;
            hitAlarm.Start();
            health -= damage * (2 - SlowFactor);
            if (health <= 0)
            {
                health = 0;
                EnemyDestroy();
                return;
            }

            if (speed > 0)
            {
                Vector2 speedVector = OGE.GetProjection(this.speed, this.direction) + OGE.GetProjection(speed, direction);
                this.speed = speedVector.Length();

                this.direction = OGE.GetAngle(Vector2.Zero, speedVector);
            }
        }

        protected virtual void CheckCollisions()
        {
            PlayerEntity player = Collide(CollisionType.Player, Position) as PlayerEntity;
            if (player != null)
            {
                player.PlayerHit(damage, damagePercentage * damage, OGE.GetAngle(Position, player.Position));
            }
        }

        protected void UpdateMovingParameter()
        {
            //Update the direction
            if (Math.Abs(direction - destinationDirection) > 2 * Math.Abs(rotationSpeed * OGE.EnemySlowFactor * SlowFactor))
            {
                if (Math.Abs(direction - destinationDirection) < Math.Abs(destinationDirection - direction))
                {
                    rotationSpeed = Math.Abs(rotationSpeed) * Math.Sign(direction - destinationDirection);
                }
                else
                {
                    rotationSpeed = Math.Abs(rotationSpeed) * Math.Sign(destinationDirection - direction);
                }

                rotationSpeed = Math.Abs(rotationSpeed);

                //Update the direction
                direction += rotationSpeed * OGE.EnemySlowFactor * SlowFactor;
                if (direction < 0)
                {
                    direction += 360;
                }
                if (direction >= 360)
                {
                    direction -= 360;
                }
            }
            else
            {
                direction = destinationDirection;
            }

            //Update the speed
            speed += acceleration * OGE.EnemySlowFactor * SlowFactor;
            if (speed > maxSpeed * OGE.EnemySlowFactor * SlowFactor)
            {
                speed = maxSpeed * OGE.EnemySlowFactor * SlowFactor;
            }

            speed -= OGE.Friction * OGE.EnemySlowFactor;
            if (speed < 0)
            {
                speed = 0;
            }
            
        }

        protected virtual void EnteranceAI()
        {

        }

        protected virtual void StandingAI()
        {
        }

        protected virtual void MovingAI()
        {
            UpdateMovingParameter();

            Vector2 speedVector = OGE.GetProjection(speed, direction);

            if (Position.X + speedVector.X < 0 || Position.X + speedVector.X > OGE.CurrentWorld.Dimensions.X)
            {
                speedVector.X *= -1;
                direction = OGE.GetAngle(Vector2.Zero, speedVector);
                destinationDirection = direction;
            }

            if (Position.Y + speedVector.Y < 0 || Position.Y + speedVector.Y > OGE.CurrentWorld.Dimensions.Y)
            {
                speedVector.Y *= -1;
                direction = OGE.GetAngle(Vector2.Zero, speedVector);
                destinationDirection = direction;
            }

            Position = Position + speedVector;

            foreach (EnemyThrusterData thrust in thrusters)
            {
                trailGenerator.Angle = direction + thrust.Direction;
                trailGenerator.GenerateParticles(Position + OGE.GetProjection(thrust.Length, direction + thrust.Direction));
            }
        }

        protected virtual void AttackingAI()
        {
            UpdateMovingParameter();

            List<BaseEntity> player = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Player);
            if (player.Count > 0)
            {
                destinationDirection = OGE.GetAngle(Position, player[0].Position);
            }
            else
            {
                enemyStatus = EnemyStatus.Moving;
            }

            Vector2 speedVector = OGE.GetProjection(speed, direction);

            Position = Position + speedVector;

            foreach (EnemyThrusterData thrust in thrusters)
            {
                trailGenerator.Angle = direction + thrust.Direction;
                trailGenerator.GenerateParticles(Position + OGE.GetProjection(thrust.Length, direction + thrust.Direction));
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CheckCollisions();
            aiFunctions[enemyStatus]();

            SlowFactor = (float)MathHelper.Clamp(SlowFactor + freezeReviving, freezeResistance, 1);

            foreach (Image image in CurrentImages)
            {
                image.Angle = direction;
            }
        }

        public override void Draw(Camera camera)
        {
            foreach (Image image in CurrentImages)
            {
                if (isHit)
                {
                    image.TintColor = new Color((int)(255 * lightingPercent), 
                        (int)(150 * lightingPercent), (int)(150 * lightingPercent));
                }
                else
                {
                    Vector3 mainColor = MainColor.ToVector3();
                    mainColor.X *= lightingPercent;
                    mainColor.Y *= lightingPercent;
                    mainColor.Z *= lightingPercent;
                    image.TintColor = new Color(mainColor);
                }
            }

            base.Draw(camera);

            this.freezeImage.TintColor = Color.White * (1 - SlowFactor);
            this.freezeImage.Draw(Position, camera);
        }
    }
}
