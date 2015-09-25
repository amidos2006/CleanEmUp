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

namespace OmidosGameEngine.Entity.Boss
{
    public class PopurBoss:BaseBoss
    {
        private Image wait1Image;
        private Image wait2Image;

        private float flikerTime = 0.3f;
        private float appearTime = 0.75f;
        private float playerPositionProbability = 1f;

        private Alarm waitAlarm;
        private Alarm flikerAlarm;

        public Color BossColor
        {
            get
            {
                return enemyColor;
            }
        }

        public PopurBoss()
            : base(new Vector2(OGE.CurrentWorld.Dimensions.X / 2 + 40, OGE.CurrentWorld.Dimensions.Y / 2 - 200))
        {
            this.status = BossState.Wait;

            Texture2D texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bosses\WaitBoss");

            this.wait1Image = new Image(texture, new Rectangle(0, 0, 128, 128));
            this.wait2Image = new Image(texture, new Rectangle(128, 0, 128, 128));

            this.images.Add(BossState.Wait, wait1Image);

            wait1Image.CenterOrigin();
            wait2Image.CenterOrigin();

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

            this.damage = 60;
            this.enemyColor = new Color(255, 150, 150);
            this.score = 5000;
            this.CurrentImage = images[status];

            this.waitAlarm = new Alarm(2f, TweenType.OneShot, WaitEnded);
            this.flikerAlarm = new Alarm(flikerTime, TweenType.OneShot, FlickImage);
            AddTween(this.waitAlarm, true);
            AddTween(this.flikerAlarm, true);
            
            AddCollisionMask(new HitboxMask(110, 110, 55, 55));
        }

        public override void Intialize()
        {
            base.Intialize();
            Explode();
        }

        private void WaitEnded()
        {
            flikerAlarm.Stop();
            Disappear();
        }

        private void FlickImage()
        {
            if (images[BossState.Wait] == wait1Image)
            {
                images[BossState.Wait] = wait2Image;
            }
            else
            {
                images[BossState.Wait] = wait1Image;
            }

            flikerAlarm.Reset((1 - this.waitAlarm.PercentComplete()) * flikerTime + 0.05f);
            flikerAlarm.Start();
        }

        public void Appear()
        {
            Explode();
            status = BossState.Wait;
            waitAlarm.Start();
            flikerAlarm.Start();
        }

        private void Disappear()
        {
            Explode();

            List<BaseEntity> player = OGE.CurrentWorld.GetCollisionEntitiesType(CollisionType.Player);
            Vector2 newPosition = new Vector2();
            if (player.Count > 0 && OGE.Random.NextDouble() < playerPositionProbability)
            {
                newPosition.X = player[0].Position.X;
                newPosition.Y = player[0].Position.Y;
            }
            else
            {
                newPosition.X = OGE.Random.Next((int)OGE.CurrentWorld.Dimensions.X - 200) + 100;
                newPosition.Y = OGE.Random.Next((int)OGE.CurrentWorld.Dimensions.Y - 200) + 100;
            }

            PopurBossNewPosition newPositionAppearer = new PopurBossNewPosition(newPosition, 
                appearTime + OGE.Random.NextDouble() * appearTime / 2, this);
            OGE.CurrentWorld.AddEntity(newPositionAppearer);
            OGE.CurrentWorld.RemoveEntity(this);
        }

        private void Explode()
        {
            BaseExplosion baseExplosion = new BaseExplosion(Position, enemyColor, 200);
            baseExplosion.Damage = 200;
            baseExplosion.DamagePercentage = 20;
            baseExplosion.AdditiveWhite = 0.4f;
            BossHit(0, 0, 0, true);
            OGE.CurrentWorld.AddEntity(baseExplosion);
        }

        private void Wait()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            waitAlarm.SpeedFactor = OGE.EnemySlowFactor;
            flikerAlarm.SpeedFactor = OGE.EnemySlowFactor;
        }
    }
}
