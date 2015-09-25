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
    public class DosBossNewPosition : BaseEntity
    {
        private Alarm appearTimer;

        public DosBossNewPosition(Vector2 newPosition, Color color, double timeToAppear, AlarmFinished endFunction)
        {
            Position.X = newPosition.X;
            Position.Y = newPosition.Y;

            CurrentImages.Add(new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bosses\ExplosionPositon")));
            CurrentImages[0].TintColor = color;
            CurrentImages[0].CenterOrigin();

            appearTimer = new Alarm(timeToAppear, TweenType.OneShot, endFunction);
            AddTween(appearTimer, true);

            EntityCollisionType = Collision.CollisionType.Effect;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CurrentImages[0].Scale = (float)(1 - appearTimer.PercentComplete());
            appearTimer.SpeedFactor = OGE.EnemySlowFactor;
        }
    }
}
