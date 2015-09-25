using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Tween;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace OmidosGameEngine.Entity.Boss
{
    public class PopurBossNewPosition : BaseEntity
    {
        private Alarm appearTimer;
        private PopurBoss boss;

        public PopurBossNewPosition(Vector2 newPosition, double timeToAppear, PopurBoss popurBoss)
        {
            this.boss = popurBoss;

            Position.X = newPosition.X;
            Position.Y = newPosition.Y;

            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bosses\ExplosionPositon")));
            CurrentImages[0].TintColor = popurBoss.BossColor;
            CurrentImages[0].CenterOrigin();

            appearTimer = new Alarm(timeToAppear, TweenType.OneShot, FinishAlarm);
            AddTween(appearTimer, true);

            EntityCollisionType = Collision.CollisionType.Effect;
        }

        private void FinishAlarm()
        {
            boss.Position.X = Position.X;
            boss.Position.Y = Position.Y;

            boss.Appear();

            OGE.CurrentWorld.AddEntity(boss);
            OGE.CurrentWorld.RemoveEntity(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CurrentImages[0].Scale = (float)(1 - appearTimer.PercentComplete());
            appearTimer.SpeedFactor = OGE.EnemySlowFactor;
        }
    }
}
