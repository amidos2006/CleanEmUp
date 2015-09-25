using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics.Particles;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Entity.ParticleGenerator
{
    public class TrailParticleGenerator : ParticleGenerator
    {
        public float Angle
        {
            set;
            get;
        }

        public float Speed
        {
            set;
            get;
        }

        public float AngleDisplacement
        {
            set;
            get;
        }

        public ParticleTextureType ParticleTexture
        {
            set;
            get;
        }

        public float Scale
        {
            set;
            get;
        }

        public Color TintColor
        {
            set
            {
                particlePrototype.ParticleColor = value;
            }
            get
            {
                return particlePrototype.ParticleColor;
            }
        }

        public Particle ParticlePrototype
        {
            set
            {
                particlePrototype = value;
            }
            get
            {
                return particlePrototype;
            }
        }

        public int NumberOfParticles
        {
            set;
            get;
        }

        public TrailParticleGenerator(ParticleEffectSystem particleSystem, Particle particlePrototype)
            : base(particleSystem, particlePrototype)
        {
            Angle = 0;
            AngleDisplacement = 15;
            NumberOfParticles = 2;
            Speed = 5;
            Scale = 1;
            ParticleTexture = ParticleTextureType.BlurredCircle;
        }

        public override void GenerateParticles(Vector2 position)
        {
            Particle tempParticle;
            for (int i = 0; i < NumberOfParticles; i++)
            {
                tempParticle = particlePrototype.Clone();
                tempParticle.Direction = Angle + random.Next(10) - 5;
                tempParticle.Angle = tempParticle.Direction;
                tempParticle.Scale = Scale;
                tempParticle.Speed = (float)(Speed + Speed / 2 * random.NextDouble());
                particleSystem.AddParticle(position, tempParticle, ParticleTexture);
            }
        }
    }
}
