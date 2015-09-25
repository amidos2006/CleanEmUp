using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Entity.ParticleGenerator;
using OmidosGameEngine.Graphics.Particles;
using OmidosGameEngine.Collision;

namespace OmidosGameEngine.Entity.Player.Bullet
{
    public class ShotgunBullet : PlayerBullet
    {
        protected TrailParticleGenerator trailParticleGenerator;

        public ShotgunBullet(Vector2 startingPoint, float speed, float direction, float maxDistance)
            : base(startingPoint, speed, direction, maxDistance)
        {
            this.damage = 40;

            Particle particlePrototype = new Particle();
            particlePrototype.ParticleColor = new Color(255, 180, 50);
            particlePrototype.DeltaScale = -0.12f;
            particlePrototype.DeltaAlpha = -0.03f;

            this.trailParticleGenerator = new TrailParticleGenerator(OGE.CurrentWorld.TrailEffectSystem, particlePrototype);
            this.trailParticleGenerator.Angle = direction + 180;
            this.trailParticleGenerator.ParticleTexture = ParticleTextureType.BlurredCircle;
            this.trailParticleGenerator.Speed = 8f;
            this.trailParticleGenerator.Scale = 0.5f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            CurrentImages[0].Scale = distance / maxDistance;
            damage = CurrentImages[0].ScaleX * 100;
            trailParticleGenerator.Scale = 0.5f * CurrentImages[0].ScaleX;
            trailParticleGenerator.GenerateParticles(Position);
        }
    }
}
