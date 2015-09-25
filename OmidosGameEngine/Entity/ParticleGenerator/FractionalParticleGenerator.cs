using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OmidosGameEngine.Entity.ParticleGenerator
{
    public class FractionalParticleGenerator : ParticleGenerator
    {
        private int numberOfFractions;

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

        public float Scale
        {
            set;
            get;
        }

        public Texture2D FractionTexture
        {
            set;
            get;
        }

        public FractionalParticleGenerator(ParticleEffectSystem particleSystem, Particle particlePrototype, int numberOfFractions = 20)
            : base(particleSystem, particlePrototype)
        {
            AngleDisplacement = 20;
            Speed = 5;
            Scale = 1;

            NumberOfCircles = 1;
            InterDistance = 20;

            this.numberOfFractions = numberOfFractions;
            
        }

        public override void GenerateParticles(Vector2 position)
        {
            Particle tempParticle;
            Vector2 tempPosition;

            Vector2 fractionSize = new Vector2(FractionTexture.Width / (numberOfFractions / 2), 
                FractionTexture.Height / (numberOfFractions / 2));

            for (int i = 0; i < NumberOfCircles; i++)
            {
                for (int j = 0; j < 360; j += (int)(AngleDisplacement))
                {
                    tempParticle = particlePrototype.Clone();
                    tempParticle.Direction = j + random.Next(10) - 5;
                    tempParticle.Angle = random.Next(360);
                    tempParticle.Scale = Scale;
                    tempParticle.Speed = (float)(Speed + Speed / 2 * random.NextDouble());
                    tempParticle.Texture = FractionTexture;
                    tempParticle.SourceRectangle = new Rectangle(random.Next((int)(tempParticle.Texture.Width - fractionSize.X)), 
                        random.Next((int)(tempParticle.Texture.Height - fractionSize.Y)), (int)fractionSize.X, (int)fractionSize.Y);
                    tempPosition = new Vector2(position.X, position.Y) + OGE.GetProjection(i * InterDistance, tempParticle.Direction);
                    particleSystem.AddFractionalParticle(tempPosition, tempParticle);
                }
            }
        }
    }
}
