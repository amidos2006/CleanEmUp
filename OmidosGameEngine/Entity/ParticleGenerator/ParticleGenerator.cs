using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics.Particles;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Entity.ParticleGenerator
{
    public class ParticleGenerator : IParticleGenerator
    {
        protected Particle particlePrototype;
        protected ParticleEffectSystem particleSystem;
        protected Random random;

        public ParticleGenerator(ParticleEffectSystem particleSystem, Particle particlePrototype)
        {
            this.particleSystem = particleSystem;
            this.particlePrototype = particlePrototype;
            this.random = new Random();
        }

        public virtual void GenerateParticles(Vector2 position)
        {
            
        }
    }
}
