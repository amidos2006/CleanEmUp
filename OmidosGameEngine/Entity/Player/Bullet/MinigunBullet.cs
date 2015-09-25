using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Entity.ParticleGenerator;
using OmidosGameEngine.Graphics.Particles;

namespace OmidosGameEngine.Entity.Player.Bullet
{
    public class MinigunBullet : PlayerBullet
    {
        protected TrailParticleGenerator trailParticleGenerator;

        public MinigunBullet(Vector2 startingPoint, float speed, float direction, float maxDistance)
            : base(startingPoint, speed, direction, maxDistance)
        {
            this.damage = 30;

            Particle particlePrototype = new Particle();
            particlePrototype.ParticleColor = new Color(255, 180, 50);
            particlePrototype.DeltaScale = -0.03f;
            particlePrototype.DeltaAlpha = -0.03f;

            this.trailParticleGenerator = new TrailParticleGenerator(OGE.CurrentWorld.TrailEffectSystem, particlePrototype);
            this.trailParticleGenerator.Angle = direction + 180;
            this.trailParticleGenerator.ParticleTexture = ParticleTextureType.BlurredCircle;
            this.trailParticleGenerator.Speed = 10f;
            this.trailParticleGenerator.Scale = 0.25f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            trailParticleGenerator.GenerateParticles(Position);
        }
    }
}
