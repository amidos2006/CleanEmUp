using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace OmidosGameEngine.Graphics.Particles
{
    public class Particle :ILogical
    {
        public Vector2 Position;
        public Color ParticleColor;
        public float Direction;
        public float Angle;
        public float Speed;
        public float Alpha;
        public float Scale;
        public float DeltaAlpha;
        public float DeltaSpeed;
        public float DeltaScale;
        public float DeltaAngle;
        [ContentSerializerIgnore]
        public Texture2D Texture;
        [ContentSerializerIgnore]
        public Rectangle? SourceRectangle = null;

        public Particle()
        {
            Position = new Vector2();
            ParticleColor = Color.White;
            Direction = 0;
            Angle = 0;
            Speed = 0;
            Alpha = 1;
            Scale = 1;
            DeltaAlpha = 0;
            DeltaScale = 0;
            DeltaSpeed = 0;
            DeltaAngle = 0;
        }

        public Particle(float speed, float direction)
        {
            Position = new Vector2();
            ParticleColor = Color.White;
            Direction = direction;
            Angle = 0;
            Speed = speed;
            Alpha = 1;
            Scale = 1;
            DeltaAlpha = 0;
            DeltaScale = 0;
            DeltaSpeed = 0;
            DeltaAngle = 0;
        }

        public Particle Clone()
        {
            Particle tempParticle = new Particle();
            
            tempParticle.Position.X = Position.X;
            tempParticle.Position.Y = Position.Y;

            tempParticle.Texture = Texture;
            tempParticle.SourceRectangle = SourceRectangle;

            tempParticle.Direction = Direction;
            tempParticle.Alpha = Alpha;
            tempParticle.Speed = Speed;
            tempParticle.Scale = Scale;
            tempParticle.Angle = Angle;

            tempParticle.DeltaSpeed = DeltaSpeed;
            tempParticle.DeltaAlpha = DeltaAlpha;
            tempParticle.DeltaScale = DeltaScale;
            tempParticle.DeltaAngle = DeltaAngle;

            tempParticle.ParticleColor = ParticleColor;

            return tempParticle;
        }

        public bool IsFinish()
        {
            if (DeltaScale < 0 && Scale <= 0)
            {
                return true;
            }

            if (DeltaAlpha < 0 && Alpha <= 0)
            {
                return true;
            }

            if (DeltaSpeed < 0 && Speed <= 0)
            {
                return true;
            }

            return false;
        }

        private Vector2 GetVelocityVector()
        {
            Vector2 velocityVector = new Vector2();

            velocityVector.X = (float)(Speed * Math.Cos(MathHelper.ToRadians(Direction)));
            velocityVector.Y = (float)(Speed * Math.Sin(MathHelper.ToRadians(Direction)));

            return velocityVector;
        }

        public void  Update(GameTime gameTime)
        {
            Vector2 velocityVector = GetVelocityVector();

            Position.X += velocityVector.X;
            Position.Y += velocityVector.Y;

            Alpha += DeltaAlpha;
            Speed += DeltaSpeed;
            Scale += DeltaScale;
            Angle += DeltaAngle;

            if (Speed < 0)
            {
                Speed = 0;
            }
        }
    }
}
