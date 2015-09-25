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

namespace OmidosGameEngine.Entity.Enemy
{
    public class Popur2Enemy: BaseEnemy
    {
        private const float MAX_FLICKER_AMOUNT = 0.5f;

        private Alarm explosionAlarm;
        private Alarm flickerAlarm;
        private Text alarmText;
        private float totalTime = 12f;

        public Popur2Enemy()
            :base(new Color(255,150,150))
        {
            maxSpeed = 0.7f * SPEED_UNIT;
            acceleration = 0.25f;
            totalTime = (float)((totalTime - 3) + 3 * OGE.Random.NextDouble());

            direction = random.Next(360);
            rotationSpeed = 5f;

            health = 30f;
            damage = 0f;
            score = 120;

            enemyStatus = EnemyStatus.Attacking;
            
            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Enemies\Popur2")));
            CurrentImages[0].CenterOrigin();

            AddCollisionMask(new HitboxMask(CurrentImages[0].Width, CurrentImages[0].Height, 
                CurrentImages[0].OriginX, CurrentImages[0].OriginY));

            thrusters.Add(new EnemyThrusterData { Direction = 180, Length = 22 });

            explosionAlarm = new Alarm(totalTime, TweenType.OneShot, EnemyDestroy);
            AddTween(explosionAlarm, true);
            flickerAlarm = new Alarm(MAX_FLICKER_AMOUNT, TweenType.OneShot, FlickerColor);
            AddTween(flickerAlarm, true);

            alarmText = new Text(Math.Ceiling((1 - explosionAlarm.PercentComplete()) * totalTime).ToString(), FontSize.Small);
            alarmText.Angle = direction + 90;
            alarmText.OriginX = alarmText.Width / 2;
            alarmText.OriginY = alarmText.Height / 2;
        }

        private void FlickerColor()
        {
            if (lightingPercent > 0.7)
            {
                lightingPercent = 0.6f;
            }
            else
            {
                lightingPercent = 1f;
            }

            flickerAlarm.Reset(MAX_FLICKER_AMOUNT * (1 - explosionAlarm.PercentComplete()) + MAX_FLICKER_AMOUNT / 10);
            flickerAlarm.Start();
        }

        protected override void CheckCollisions()
        {
            PlayerEntity player = Collide(CollisionType.Player, Position) as PlayerEntity;
            if (player != null)
            {
                EnemyDestroy();
            }
        }

        public override void EnemyDestroy()
        {
            base.EnemyDestroy();
            BaseExplosion baseExplosion = new BaseExplosion(Position, enemyColor, 180);
            baseExplosion.Damage = 120;
            baseExplosion.DamagePercentage = 20;
            OGE.CurrentWorld.AddEntity(baseExplosion);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            alarmText.TextContext = Math.Ceiling((1 - explosionAlarm.PercentComplete()) * totalTime).ToString();
            alarmText.Angle = direction + 90;
            alarmText.OriginX = alarmText.Width / 2;
            alarmText.OriginY = alarmText.Height / 2;

            explosionAlarm.SpeedFactor = OGE.EnemySlowFactor;
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            //alarmText.Draw(Position, camera);
        }
    }
}
