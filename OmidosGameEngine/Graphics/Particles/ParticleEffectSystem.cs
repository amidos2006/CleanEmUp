using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OmidosGameEngine.Graphics.Particles
{
    public class ParticleEffectSystem : ILogical
    {
        private List<Particle> particles;
        private List<Particle> removedParticles;
        private Dictionary<ParticlePatternType, ParticlePattern> patterns;
        private Dictionary<ParticleTextureType,Texture2D> particleTextures;
        private RenderTarget2D renderTarget;
        private RenderTarget2D oldRenderTarget;
        private Vector2 oldPosition;
        private BlendState blendState;
        private Random random;
        private bool deleteInRenderLoop;

        public float AlphaOldTarget
        {
            set;
            get;
        }

        public ParticleEffectSystem(BlendState blendState)
        {
            this.AlphaOldTarget = 0.8f;
            this.particles = new List<Particle>();
            this.patterns = new Dictionary<ParticlePatternType,ParticlePattern>();
            this.removedParticles = new List<Particle>();
            this.particleTextures = new Dictionary<ParticleTextureType,Texture2D>();
            this.renderTarget = new RenderTarget2D(OGE.GraphicDevice, OGE.WorldCamera.Width, OGE.WorldCamera.Height);
            this.oldRenderTarget = new RenderTarget2D(OGE.GraphicDevice, OGE.WorldCamera.Width, OGE.WorldCamera.Height);
            this.oldPosition = new Vector2();
            this.blendState = blendState;
            this.random = new Random();
        }

        public void LoadContent()
        {
            #region Load textures

            Texture2D tempParticle = OGE.Content.Load<Texture2D>(@"Graphics\Particles\BlurParticle");
            particleTextures.Add(ParticleTextureType.BlurredCircle, tempParticle);

            tempParticle = OGE.Content.Load<Texture2D>(@"Graphics\Particles\FireParticle");
            particleTextures.Add(ParticleTextureType.BlurredFire, tempParticle);

            tempParticle = OGE.Content.Load<Texture2D>(@"Graphics\Particles\LineParticle");
            particleTextures.Add(ParticleTextureType.BlurredLine, tempParticle);

            tempParticle = OGE.Content.Load<Texture2D>(@"Graphics\Particles\BlurSquare");
            particleTextures.Add(ParticleTextureType.BlurredSquare, tempParticle);

            #endregion

            #region Load Patterns

            ParticlePattern doubleCircleDoubleLayerPattern = new ParticlePattern(100);
            ParticlePattern doubleLayerCirclePattern = new ParticlePattern(100);
            ParticlePattern circlePattern = new ParticlePattern(100);
            ParticlePattern doubleCirclePattern = new ParticlePattern(100);
            Particle temp;
            Particle cloneTemp;
            float direction;

            for (int i = 0; i < 25; i++)
            {
                direction = random.Next(360);

                temp = new Particle();
                temp.Direction = direction;
                temp.Angle = direction;
                temp.DeltaSpeed = -0.5f;
                temp.DeltaScale = -0.02f;

                doubleCircleDoubleLayerPattern.Particles[2 * i] = temp;
                doubleCircleDoubleLayerPattern.Particles[2 * i + 1] = temp;

                doubleLayerCirclePattern.Particles[2 * i] = temp;
                doubleLayerCirclePattern.Particles[2 * i + 1] = temp;

                direction = random.Next(360);

                cloneTemp = temp.Clone();
                cloneTemp.Direction = direction;
                cloneTemp.Angle = direction;

                circlePattern.Particles[2 * i] = temp;
                circlePattern.Particles[2 * i + 1] = cloneTemp;

                doubleCirclePattern.Particles[2 * i] = temp;
                doubleCirclePattern.Particles[2 * i + 1] = cloneTemp;
            }

            for (int i = 25; i < 50; i++)
            {
                direction = random.Next(360);

                temp = new Particle();
                temp.Position.X = (float)(50 * Math.Cos(MathHelper.ToRadians(direction)));
                temp.Position.Y = (float)(50 * Math.Sin(MathHelper.ToRadians(direction)));
                temp.Direction = direction;
                temp.Angle = direction;
                temp.DeltaSpeed = -0.4f;
                temp.DeltaScale = -0.02f;

                doubleCircleDoubleLayerPattern.Particles[2 * i] = temp;
                doubleCircleDoubleLayerPattern.Particles[2 * i + 1] = temp;

                cloneTemp = temp.Clone();
                cloneTemp.Position.X = 0;
                cloneTemp.Position.Y = 0;

                doubleLayerCirclePattern.Particles[2 * i] = cloneTemp;
                doubleLayerCirclePattern.Particles[2 * i + 1] = cloneTemp;

                circlePattern.Particles[2 * i] = cloneTemp;

                direction = random.Next(360);

                cloneTemp = temp.Clone();
                cloneTemp.Direction = direction;
                cloneTemp.Angle = direction;

                doubleCirclePattern.Particles[2 * i] = temp;
                doubleCirclePattern.Particles[2 * i + 1] = cloneTemp;

                cloneTemp = cloneTemp.Clone();
                cloneTemp.Position.X = 0;
                cloneTemp.Position.Y = 0;

                circlePattern.Particles[2 * i + 1] = cloneTemp;
            }

            patterns.Add(ParticlePatternType.DoubleCircleDoubleLayerCirclePattern, doubleCircleDoubleLayerPattern);
            patterns.Add(ParticlePatternType.DoubleLayerCirclePattern, doubleLayerCirclePattern);
            patterns.Add(ParticlePatternType.CirclePattern, circlePattern);
            patterns.Add(ParticlePatternType.DoubleCirclePattern, doubleCirclePattern);

            #endregion
        }

        public void AddParticle(Vector2 position, Particle prototype, ParticleTextureType textureType)
        {
            Particle particle = prototype.Clone();
            particle.Texture = particleTextures[textureType];
            particle.Position = position;
            particle.Angle = particle.Direction;

            particles.Add(particle);
        }

        public void AddFractionalParticle(Vector2 position, Particle prototype)
        {
            Particle particle = prototype.Clone();
            particle.Position = position;
            particle.Angle = particle.Direction;

            particles.Add(particle);
        }

        public void GenerateColoredPattern(Vector2 position, float speed, Color color, ParticlePatternType index, ParticleTextureType texture)
        {
            Particle temp;
            float randomAmount;

            foreach (Particle particle in patterns[index].Particles)
            {
                temp = particle.Clone();
                temp.Position += position;
                temp.ParticleColor = color;
                temp.Speed = speed;
                temp.Texture = particleTextures[texture];

                randomAmount = (float)(1 - 0.1 * random.NextDouble());
                temp.Speed *= randomAmount;
                temp.Scale *= randomAmount;
                temp.DeltaSpeed *= randomAmount;
                temp.DeltaAlpha *= randomAmount;
                temp.DeltaScale *= randomAmount;

                particles.Add(temp);
            }
        }

        public void GeneratePattern(Vector2 position, ParticlePatternType index, ParticleTextureType texture)
        {
            Particle temp;
            float randomAmount;

            foreach (Particle particle in patterns[index].Particles)
            {
                temp = particle.Clone();
                temp.Position += position;
                temp.Texture = particleTextures[texture];

                randomAmount = (float)(1 - 0.1 * random.NextDouble());
                temp.Speed *= randomAmount;
                temp.Scale *= randomAmount;
                temp.DeltaSpeed *= randomAmount;
                temp.DeltaAlpha *= randomAmount;
                temp.DeltaScale *= randomAmount;

                particles.Add(temp);
            }
        }

        private void CleanUpSystem(bool isRender)
        {
            if (deleteInRenderLoop == isRender)
            {
                for (int i = 0; i < removedParticles.Count; i++)
                {
                    particles.Remove(removedParticles[i]);
                }
            }
        }

        public void RemoveAllParticles()
        {
            particles.Clear();
            removedParticles.Clear();
        }

        public void Update(GameTime gameTime)
        {
            removedParticles.Clear();

            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update(gameTime);
                if (particles[i].IsFinish())
                {
                    removedParticles.Add(particles[i]);
                }
            }

            deleteInRenderLoop = gameTime.IsRunningSlowly;

            CleanUpSystem(deleteInRenderLoop);
        }

        public void BeginDraw(Camera camera)
        {
            //CleanUpSystem(true);

            SpriteBatch spriteBatch = OGE.SpriteBatch;
            Vector2 origin;
            Vector2 cameraPosition;
            Rectangle particleRectangle;

            OGE.GraphicDevice.SetRenderTarget(renderTarget);
            OGE.GraphicDevice.Clear(new Color(0, 0, 0, 0));

            spriteBatch.Begin(OGE.SpriteSortMode, blendState);

            spriteBatch.Draw(oldRenderTarget, camera.ConvertToCamera(oldPosition), Color.White * AlphaOldTarget);

            foreach (Particle particle in particles)
            {
                origin = new Vector2(particle.Texture.Width/2,particle.Texture.Height/2);
                particleRectangle = new Rectangle((int)(particle.Position.X - particle.Texture.Width / 2), 
                    (int)(particle.Position.Y - particle.Texture.Height / 2), particle.Texture.Width, particle.Texture.Height);

                if(camera.CheckRectangleInCamera(particleRectangle))
                {
                    cameraPosition = camera.ConvertToCamera(particle.Position);
                    spriteBatch.Draw(particle.Texture, cameraPosition, particle.SourceRectangle, particle.ParticleColor * particle.Alpha, 
                        MathHelper.ToRadians(particle.Angle), origin, particle.Scale, SpriteEffects.None, 0);
                }
            }

            spriteBatch.End();

            OGE.GraphicDevice.SetRenderTarget(oldRenderTarget);
            OGE.GraphicDevice.Clear(new Color(0, 0, 0, 0));
            spriteBatch.Begin(OGE.SpriteSortMode, OGE.BlendState);
            spriteBatch.Draw(renderTarget, new Vector2(), Color.White);
            spriteBatch.End();
            oldPosition.X = camera.X;
            oldPosition.Y = camera.Y;

            OGE.GraphicDevice.SetRenderTarget(null);
        }

        public void Draw()
        {
            SpriteBatch spriteBatch = OGE.SpriteBatch;
            
            spriteBatch.Begin(OGE.SpriteSortMode, OGE.BlendState);
            spriteBatch.Draw(renderTarget, new Vector2(), Color.White);
            spriteBatch.End();
        }
    }
}
