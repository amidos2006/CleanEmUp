using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics.Particles;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Entity.ParticleGenerator
{
    public class CircleParticleGenerator : ParticleGenerator
    {
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

        public int NumberOfCircles
        {
            set;
            get;
        }

        public float InterDistance
        {
            set;
            get;
        }

        public float StartingDistance
        {
            set;
            get;
        }

        public float Scale
        {
            set;
            get;
        }

        public Color ParticleColor
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

        public CircleParticleGenerator(ParticleEffectSystem particleSystem, Particle particlePrototype)
            : base(particleSystem, particlePrototype)
        {
            AngleDisplacement = 15;
            StartingDistance = 0;
            Speed = 5;
            Scale = 1;
            ParticleTexture = ParticleTextureType.BlurredCircle;

            NumberOfCircles = 1;
            InterDistance = 20;
        }

        public override void GenerateParticles(Vector2 position)
        {
            Particle tempParticle;
            Vector2 tempPosition;

            for (int i = 0; i < NumberOfCircles; i++)
            {
                for (int j = 0; j < 360; j += (int)(AngleDisplacement))
                {
                    tempParticle = particlePrototype.Clone();
                    tempParticle.Direction = j + random.Next(10) - 5;
                    tempParticle.Angle = tempParticle.Direction;
                    tempParticle.Scale = Scale;
                    tempParticle.Speed = (float)(Speed + Speed / 2 * random.NextDouble());
                    tempPosition = new Vector2(position.X, position.Y) + OGE.GetProjection(i * InterDistance + StartingDistance, 
                        tempParticle.Direction);
                    particleSystem.AddParticle(tempPosition, tempParticle, ParticleTexture);
                }
            }
        }
    }
}
