using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Entity.Player.Bullet;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Entity.Explosion;
using OmidosGameEngine.Tween;

namespace OmidosGameEngine.Entity.Player.OverClocking
{
    public class ExplosionEffectArea : EffectArea
    {
        protected Color baseColor;
        protected float baseDamage;
        protected Alarm explosionAlarm;

        public ExplosionEffectArea(Color color, float explosionInterval, float damage, float timeToLast)
            : base(color, timeToLast)
        {
            this.baseColor = color;
            this.baseDamage = damage;

            image = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\Bullets\WaveRange"));
            image.Scale = 0;
            image.TintColor = color;
            image.CenterOrigin();

            maxRadius = image.Width;

            explosionAlarm = new Alarm(explosionInterval, TweenType.Looping, CreateExplosion);
            AddTween(explosionAlarm, true);
        }

        public override void Intialize()
        {
            CreateExplosion();
        }

        protected void CreateExplosion()
        {
            BaseExplosion explosion = new BaseExplosion(Position, baseColor, 300);
            explosion.FriendlyExplosion = true;
            explosion.Damage = baseDamage;
            explosion.DamagePercentage = 0.1f;
            OGE.CurrentWorld.AddEntity(explosion);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            explosionAlarm.SpeedFactor = OGE.PlayerSlowFactor;
        }
    }
}
