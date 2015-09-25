using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Entity.Player;
using OmidosGameEngine.Tween;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Entity.ParticleGenerator;
using OmidosGameEngine.Graphics.Particles;
using OmidosGameEngine.Collision;

namespace OmidosGameEngine.Entity.Object
{
    public class BaseObject:BaseEntity
    {
        public static double GetTime(Random random)
        {
            return random.NextDouble() * 7 + 8;
        }

        private Alarm removalAlarm;
        private Alarm notifingAlarm;
        protected CircleParticleGenerator circleGenerator;
        protected Color removalColor;

        public BaseObject(double timeTillRemove, Color removalColor)
        {
            this.removalAlarm = new Alarm(timeTillRemove, TweenType.OneShot, new AlarmFinished(RemoveEntity));
            AddTween(removalAlarm, true);
            this.removalColor = removalColor;

            this.notifingAlarm = new Alarm(0.8f, TweenType.Looping, GenerateNotifier);
            AddTween(this.notifingAlarm, true);

            Particle particlePrototype = new Particle();
            particlePrototype.ParticleColor = removalColor;
            particlePrototype.DeltaScale = 0.03f;
            particlePrototype.DeltaAlpha = -0.03f;

            this.circleGenerator = new CircleParticleGenerator(OGE.CurrentWorld.TrailEffectSystem, particlePrototype);
            this.circleGenerator.AngleDisplacement = 30;
            this.circleGenerator.NumberOfCircles = 1;
            this.circleGenerator.ParticleTexture = ParticleTextureType.BlurredCircle;
            this.circleGenerator.Speed = 1f;
            this.circleGenerator.Scale = 0.5f;

            EntityCollisionType = CollisionType.Object;
        }

        public override void Intialize()
        {
            base.Intialize();
            GenerateNotifier();
        }

        private void GenerateNotifier()
        {
            ObjectNotifierEntity notifier = new ObjectNotifierEntity();
            notifier.Position.X = Position.X;
            notifier.Position.Y = Position.Y;

            OGE.CurrentWorld.AddEntity(notifier);
        }

        public virtual void RemoveEntity()
        {
            circleGenerator.GenerateParticles(Position);
            OGE.CurrentWorld.RemoveEntity(this);
        }

        public virtual void ApplyBonus(PlayerEntity player)
        {
            GlobalVariables.Achievements[this.GetType()].CurrentNumber += 1;

            SoundManager.ListnerPosition = Position;
            SoundManager.PlaySFX("pickup");
            RemoveEntity();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            PlayerEntity player = Collide(Collision.CollisionType.Player, Position) as PlayerEntity;
            if (player != null)
            {
                ApplyBonus(player);
            }
        }
    }
}
