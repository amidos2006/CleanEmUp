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

namespace OmidosGameEngine.Entity.Boss
{
    public class TroyBoss:BaseBoss
    {
        private float rotationAcceleration = 1f;
        private float rotationSpeed = 0;
        private float maxRotationSpeed = 40;
        private Alarm waitAlarm;

        public TroyBoss()
            : base(new Vector2(OGE.CurrentWorld.Dimensions.X / 2, -100))
        {
            Texture2D texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bosses\TroyBoss");

            this.images.Add(BossState.Enterance, new Image(texture, new Rectangle(0, 0, 128, 128)));
            this.images.Add(BossState.Move, new Image(texture, new Rectangle(0, 0, 128, 128)));
            this.images.Add(BossState.Rotate, new Image(texture, new Rectangle(0, 0, 128, 128)));
            this.images.Add(BossState.Summon, new Image(texture, new Rectangle(0, 0, 128, 128)));
            this.images.Add(BossState.Wait, new Image(texture, new Rectangle(0, 0, 128, 128)));

            foreach (Image image in this.images.Values)
            {
                image.CenterOrigin();
            }

            this.actions.Add(BossState.Enterance, CheckMoveEnds);
            this.actions.Add(BossState.Summon, Summon);
            this.actions.Add(BossState.Wait, Wait);
            this.actions.Add(BossState.Rotate, Rotate);
            this.actions.Add(BossState.Move, CheckMoveEnds);

            this.maxHealth = 6000;
            this.health = this.maxHealth;

            this.acceleration = 0;
            this.Direction = 90;
            this.maxSpeed = 15 * BaseEnemy.SPEED_UNIT;
            this.speed = this.maxSpeed / 4;
            this.slidingFactor = 0;
            this.freezeResistance = 1f;

            this.damage = 50;
            this.enemyColor = new Color(80, 120, 20);
            this.score = 5000;
            this.CurrentImage = images[status];

            this.waitAlarm = new Alarm(1f, TweenType.OneShot, () => { status = BossState.Rotate; });
            AddTween(this.waitAlarm);

            AddCollisionMask(new HitboxMask(120, 120, 60, 60));
        }

        private void CheckMoveEnds()
        {
            rotationSpeed = speed / maxSpeed * maxRotationSpeed;
            if (speed <= 0)
            {
                status = BossState.Summon;
            }
        }

        private void Summon()
        {
            TroyEnemy troy = new TroyEnemy();
            troy.Position = new Vector2(Position.X, Position.Y);
            troy.JumpDirection(CurrentImage.Angle);
            troy.GeneratedTroy();
            OGE.CurrentWorld.AddEntity(troy);

            troy = new TroyEnemy();
            troy.Position = new Vector2(Position.X, Position.Y);
            troy.JumpDirection(CurrentImage.Angle + 90);
            troy.GeneratedTroy();
            OGE.CurrentWorld.AddEntity(troy);

            troy = new TroyEnemy();
            troy.Position = new Vector2(Position.X, Position.Y);
            troy.JumpDirection(CurrentImage.Angle + 180);
            troy.GeneratedTroy();
            OGE.CurrentWorld.AddEntity(troy);

            troy = new TroyEnemy();
            troy.Position = new Vector2(Position.X, Position.Y);
            troy.JumpDirection(CurrentImage.Angle + 270);
            troy.GeneratedTroy();
            OGE.CurrentWorld.AddEntity(troy);

            status = BossState.Wait;
            waitAlarm.Start();
        }

        private void Wait()
        {
            Direction = OGE.Random.Next(360);

            List<BaseEntity> players = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Player);
            if (players.Count > 0)
            {
                Direction = OGE.GetAngle(Position, players[0].Position);
            }
        }

        private void Rotate()
        {
            rotationSpeed += rotationAcceleration;
            if (rotationSpeed > maxRotationSpeed)
            {
                rotationSpeed = maxRotationSpeed;
                speed = maxSpeed;
                status = BossState.Move;
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Image image in images.Values)
            {
                image.Angle = (image.Angle + rotationSpeed * OGE.EnemySlowFactor) % 360;
            }

            waitAlarm.SpeedFactor = OGE.EnemySlowFactor;

            base.Update(gameTime);
        }
    }
}
