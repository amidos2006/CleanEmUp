using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Graphics.Lighting
{
    public class LightingSystem : ILogical
    {
        private List<LightSource> lightingSources;
        private List<LightSource> removedSources;

        private BlendState lightingBlendState;
        private RenderTarget2D renderTarget;
        private Texture2D lightTexture;

        public float AmbientLight
        {
            set;
            get;
        }

        public LightingSystem()
        {
            lightingSources = new List<LightSource>();
            removedSources = new List<LightSource>();

            AmbientLight = 1;

            renderTarget = new RenderTarget2D(OGE.GraphicDevice, OGE.WorldCamera.Width, OGE.WorldCamera.Height);
            lightingBlendState = new BlendState
            {
                ColorSourceBlend = Blend.DestinationColor,
                AlphaSourceBlend = Blend.DestinationAlpha
            };
        }

        public void LoadContent()
        {
            lightTexture = OGE.Content.Load<Texture2D>(@"Graphics\LightSource\lightsource");
        }

        public LightSource GenerateLightSource(Vector2 position, Vector2 size, Color lightingColor, float deltaAlpha)
        {
            float scale = Math.Min(size.X / lightTexture.Width, size.Y / lightTexture.Height);

            LightSource temp;
            temp = new LightSource(position, scale, lightingColor);
            temp.DeltaAlpha = deltaAlpha;

            lightingSources.Add(temp);

            return temp;
        }

        public void RemoveLightSource(LightSource lightSource)
        {
            lightingSources.Remove(lightSource);
        }

        public void  Update(GameTime gameTime)
        {
            removedSources.Clear();

            for (int i = 0; i < lightingSources.Count; i++)
            {
                lightingSources[i].Update(gameTime);
                if (lightingSources[i].IsFinish())
                {
                    removedSources.Add(lightingSources[i]);
                }
            }

            for (int i = 0; i < removedSources.Count; i++)
            {
                lightingSources.Remove(removedSources[i]);
            }
        }

        public void BeginDraw(Camera camera)
        {
            SpriteBatch spriteBatch = OGE.SpriteBatch;
            Vector2 origin = new Vector2(lightTexture.Width / 2, lightTexture.Height / 2);
            Vector2 cameraPosition;
            Rectangle lightRectangle;
            int width, height;

            OGE.GraphicDevice.SetRenderTarget(renderTarget);
            OGE.GraphicDevice.Clear(new Color(AmbientLight, AmbientLight, AmbientLight));

            spriteBatch.Begin(OGE.SpriteSortMode, OGE.BlendState);

            foreach (LightSource light in lightingSources)
            {
                width = (int)(lightTexture.Width * light.Scale);
                height = (int)(lightTexture.Height * light.Scale);
                lightRectangle = new Rectangle((int)(light.Position.X - width / 2), (int)(light.Position.Y - height / 2), width, height);

                if (camera.CheckRectangleInCamera(lightRectangle))
                {
                    cameraPosition = camera.ConvertToCamera(light.Position);
                    spriteBatch.Draw(lightTexture, cameraPosition, null, light.TintColor * light.Alpha, 0, origin, 
                        light.Scale, SpriteEffects.None, 0);
                }
            }

            spriteBatch.End();

            OGE.GraphicDevice.SetRenderTarget(null);
        }

        public void Draw()
        {
            SpriteBatch spriteBatch = OGE.SpriteBatch;

            spriteBatch.Begin(OGE.SpriteSortMode, lightingBlendState);

            spriteBatch.Draw(renderTarget, new Vector2(), Color.White);

            spriteBatch.End();
        }
    }
}
