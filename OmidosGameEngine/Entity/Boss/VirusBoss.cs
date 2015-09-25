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
    public class VirusBoss:BaseBoss
    {
        private Alarm waitAlarm;
        private Alarm moveAlarm;
        private List<VirusBossTail> tail;
        private Image fine;
        private float yScale = 0.4f;

        public float Damage
        {
            get
            {
                return damage;
            }
        }

        public VirusBoss()
            : base(new Vector2(OGE.CurrentWorld.Dimensions.X / 2, -100))
        {
            Texture2D texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bosses\VirusBoss");

            this.images.Add(BossState.Enterance, new Image(texture));
            this.images.Add(BossState.Move, new Image(texture));
            this.images.Add(BossState.Summon, new Image(texture));
            this.images.Add(BossState.Wait, new Image(texture));

            foreach (Image image in this.images.Values)
            {
                image.CenterOrigin();
            }

            this.actions.Add(BossState.Enterance, CheckMoveEnds);
            this.actions.Add(BossState.Summon, Summon);
            this.actions.Add(BossState.Wait, Wait);
            this.actions.Add(BossState.Move, CheckMoveEnds);

            this.maxHealth = 15000;
            this.health = this.maxHealth;

            this.acceleration = 0;
            this.Direction = 90;
            this.maxSpeed = 3 * BaseEnemy.SPEED_UNIT;
            this.speed = this.maxSpeed;
            this.slidingFactor = 0;
            this.freezeResistance = 1f;

            this.damage = 60;
            this.enemyColor = new Color(255, 170, 80);
            this.score = 5000;
            this.CurrentImage = images[status];

            this.enableReflection = false;

            this.waitAlarm = new Alarm(0.5f, TweenType.OneShot, () => { status = BossState.Move; moveAlarm.Start(); });
            this.moveAlarm = new Alarm(5f, TweenType.OneShot, () => { status = BossState.Summon; });
            AddTween(this.waitAlarm);
            AddTween(this.moveAlarm, true);

            AddCollisionMask(new HitboxMask(80, 80, 40, 40));

            this.fine = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bosses\VirusBossTail"));
            this.fine.OriginY = this.fine.Height / 2;
            this.fine.ScaleY = yScale;

            this.tail = new List<VirusBossTail>();
            for (int i = 0; i < 8; i++)
            {
                VirusBossTail temp = new VirusBossTail(this, i, 8);
                OGE.CurrentWorld.AddEntity(temp);
                tail.Add(temp);
            }
        }

        public override void DestroyBoss()
        {
            base.DestroyBoss();

            for (int i = 0; i < tail.Count; i++)
            {
                OGE.CurrentWorld.RemoveEntity(tail[i]);
            }
        }

        private void CheckMoveEnds()
        {
            List<BaseEntity> players = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Player);
            if (players.Count > 0)
            {
                Direction = OGE.GetAngle(Position, players[0].Position);
            }

            if (speed <= 0)
            {
                speed = maxSpeed;
            }
        }

        private void Summon()
        {
            status = BossState.Wait;
            waitAlarm.Start();
        }

        private void Wait()
        {
            List<BaseEntity> players = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Player);
            if (players.Count > 0)
            {
                Direction = OGE.GetAngle(Position, players[0].Position);
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Image image in images.Values)
            {
                image.Angle = Direction;
            }

            if(OGE.GetDistance(tail[0].Position, Position) > 70)
            {
                tail[0].Position = tail[0].Position + 0.06f * (Position - tail[0].Position);
            }

            for (int i = 1; i < tail.Count; i++)
            {
                if (OGE.GetDistance(tail[i].Position, tail[i - 1].Position) > 50 - (i - 1) * 5)
                {
                    tail[i].Position = tail[i].Position + 0.06f * (tail[i - 1].Position - tail[i].Position);
                }
            }

            for (int i = 0; i < tail.Count; i++)
            {
                tail[i].IsHit = isHit;
            }

            waitAlarm.SpeedFactor = OGE.EnemySlowFactor;
            moveAlarm.SpeedFactor = OGE.EnemySlowFactor;

            base.Update(gameTime);
        }

        public override void Draw(Camera camera)
        {
            Vector2 direction2 = (Position - tail[0].Position) / 2;
            Vector2 leftOffset = Vector2.Transform(direction2, Matrix.CreateRotationZ((float)(Math.PI / 6)));
            Vector2 rightOffset = Vector2.Transform(direction2, Matrix.CreateRotationZ((float)(-Math.PI / 6)));

            fine.ScaleX = tail[0].Scale;
            fine.ScaleY = yScale * tail[0].Scale;
            fine.TintColor = CurrentImage.TintColor;

            fine.Angle = OGE.GetAngle(Vector2.Zero, direction2) + 180 - (maxSpeed - speed) * 5 - 10;
            fine.Draw(tail[0].Position + leftOffset, camera);

            fine.Angle = OGE.GetAngle(Vector2.Zero, direction2) + 180 + (maxSpeed - speed) * 5 + 10;
            fine.Draw(tail[0].Position + rightOffset, camera);

            for (int i = 1; i < tail.Count; i++)
            {
                direction2 = (tail[i - 1].Position - tail[i].Position) / 2;
                leftOffset = Vector2.Transform(direction2, Matrix.CreateRotationZ((float)(Math.PI / 6)));
                rightOffset = Vector2.Transform(direction2, Matrix.CreateRotationZ((float)(-Math.PI / 6)));

                fine.ScaleX = tail[i].Scale;
                fine.ScaleY = yScale * tail[i].Scale;
                fine.TintColor = CurrentImage.TintColor;

                fine.Angle = OGE.GetAngle(Vector2.Zero, direction2) + 180 - (maxSpeed - speed) * 5 - 10;
                fine.Draw(tail[i].Position + leftOffset, camera);

                fine.Angle = OGE.GetAngle(Vector2.Zero, direction2) + 180 + (maxSpeed - speed) * 5 + 10;
                fine.Draw(tail[i].Position + rightOffset, camera);
            }

            direction2 = (tail[tail.Count - 1].Position - tail[tail.Count - 2].Position) / 4;
            leftOffset = Vector2.Transform(direction2, Matrix.CreateRotationZ((float)(Math.PI / 6)));
            rightOffset = Vector2.Transform(direction2, Matrix.CreateRotationZ((float)(-Math.PI / 6)));

            fine.ScaleX = 0.5f;
            fine.ScaleY = yScale * 0.5f;
            fine.TintColor = CurrentImage.TintColor;

            fine.Angle = OGE.GetAngle(Vector2.Zero, direction2) + 30;
            fine.Draw(tail[tail.Count - 1].Position + leftOffset, camera);

            fine.Angle = OGE.GetAngle(Vector2.Zero, direction2) - 30;
            fine.Draw(tail[tail.Count - 1].Position + rightOffset, camera);

            base.Draw(camera);
        }
    }
}
