using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using OmidosGameEngine.World;
using OmidosGameEngine.Sounds;
using Microsoft.Xna.Framework.Input;
using OmidosGameEngine.Collision;
using OmidosGameEngine.Entity.Cursor;
using Microsoft.Xna.Framework.Storage;

namespace OmidosGameEngine
{
    public static class OGE
    {
        /// <summary>
        /// graphic device used by Engine
        /// </summary>
        private static GraphicsDevice graphicDevice;
        /// <summary>
        /// Content Manager responsible for loading any game assets
        /// </summary>
        private static ContentManager content;
        /// <summary>
        /// spriteBatch used by engine
        /// </summary>
        private static SpriteBatch spriteBatch;
        /// <summary>
        /// world camera that can be update and moved
        /// </summary>
        private static Camera worldCamera;
        /// <summary>
        /// HUD camera that holds the position of hud and size on screen
        /// </summary>
        private static Camera hudCamera;
        /// <summary>
        /// This is system SpriteSort Mode in rendering stuffs
        /// </summary>
        private static SpriteSortMode spriteSortMode;
        /// <summary>
        /// This is system blend state
        /// </summary>
        private static BlendState blendState;
        /// <summary>
        /// 
        /// </summary>
        private static Vector2 screenResolution = new Vector2(1280, 720);
        private static Texture2D pixelTest;

        /// <summary>
        /// get the world camera where you can change and update it
        /// </summary>
        public static Camera WorldCamera
        {
            get
            {
                return worldCamera;
            }
        }
        /// <summary>
        /// get the fixed camera that represents the viewport of the screen
        /// </summary>
        public static Camera HUDCamera
        {
            get
            {
                return hudCamera;
            }
        }
        /// <summary>
        /// get the graphic device that the engine use
        /// </summary>
        public static GraphicsDevice GraphicDevice
        {
            get
            {
                return graphicDevice;
            }
        }
        /// <summary>
        /// get the content manager that the engine use
        /// </summary>
        public static ContentManager Content
        {
            get
            {
                return content;
            }
        }
        /// <summary>
        /// Get the sprite batch that the engine use
        /// </summary>
        public static SpriteBatch SpriteBatch
        {
            get
            {
                return spriteBatch;
            }
        }
        public static SpriteSortMode SpriteSortMode
        {
            get
            {
                return spriteSortMode;
            }
        }
        public static BlendState BlendState
        {
            get
            {
                return blendState;
            }
        }
        public static Vector2 ScreenResolution
        {
            get
            {
                return screenResolution;
            }
        }
        public static BaseWorld CurrentWorld
        {
            set;
            get;
        }
        public static BaseWorld NextWorld
        {
            set;
            get;
        }
        public static float Friction
        {
            set;
            get;
        }
        public static float PlayerSlowFactor
        {
            set;
            get;
        }
        public static float EnemySlowFactor
        {
            set;
            get;
        }
        public static StorageDevice Storage
        {
            set;
            get;
        }
        public static Random Random
        {
            set;
            get;
        }
        public static Game CleanEmUpApplication
        {
            set;
            get;
        }
        public static int DrawFrameRate
        {
            set;
            get;
        }

        /// <summary>
        /// Intialize the Platform Engine must be called when starting the engine
        /// </summary>
        /// <param name="graphicDevice">graphicDevice intialized by the Game class of XNA</param>
        /// <param name="content">Content Manager that loads graphics</param>
        public static void Intialize(GraphicsDevice graphicDevice, ContentManager content)
        {
            OGE.graphicDevice = graphicDevice;
            OGE.spriteBatch = new SpriteBatch(GraphicDevice);
            OGE.content = content;

            OGE.Friction = 0.1f;

            OGE.spriteSortMode = SpriteSortMode.Immediate;
            OGE.blendState = BlendState.AlphaBlend;

            OGE.PlayerSlowFactor = 1;
            OGE.EnemySlowFactor = 1;
            
            OGE.worldCamera = new Camera(graphicDevice.Viewport.Width, graphicDevice.Viewport.Height);
            OGE.hudCamera = new Camera(graphicDevice.Viewport.Width, graphicDevice.Viewport.Height);

            Input.Intialzie();
            if (OGE.CurrentWorld != null)
            {
                OGE.CurrentWorld.Intialize();
            }

            OGE.pixelTest = Content.Load<Texture2D>(@"Graphics\PixelTest");

            Object stateobj = (Object)"GetDevice for Player One";
            StorageDevice.BeginShowSelector(OGE.StorageReady, stateobj);

            CursorEntity.Intialize();
            OGE.Random = new Random(DateTime.Now.Millisecond);
        }

        private static void StorageReady(IAsyncResult result)
        {
            OGE.Storage = StorageDevice.EndShowSelector(result);
        }

        /// <summary>
        /// Wrap the input value in the correct range
        /// </summary>
        /// <param name="x">the value required for wrapping</param>
        /// <param name="min">lowest value that the number can't be lower than it (most of time equal zero)</param>
        /// <param name="max">maximum value that the number can't exceed</param>
        /// <returns>a number between min and max that correspont to the remainder of x if it is only between min and max</returns>
        public static int NumberWrap(int x, int min, int max)
        {
            int distance = max - min;
            int number = x - min;

            number = number % distance;
            if (number < 0)
            {
                number += distance;
            }

            return number + min;
        }

        /// <summary>
        /// Get the left upper position of the image providing the position
        /// </summary>
        /// <param name="position">position of the image</param>
        /// <param name="origin">position of the center of image</param>
        /// <returns>left upper position of the image</returns>
        public static Vector2 GetLeftUpperCorner(Vector2 position, Vector2 origin)
        {
            return new Vector2(position.X - origin.X, position.Y - origin.Y);
        }

        /// <summary>
        /// Get Angle in Degrees between two points
        /// </summary>
        /// <param name="originPoint">point represent the origin for the angle</param>
        /// <param name="destinationPoint">point required to measure the angle toward it</param>
        /// <returns>Get Angle in Degrees</returns>
        public static float GetAngle(Vector2 originPoint, Vector2 destinationPoint)
        {
            float angle = MathHelper.ToDegrees((float)Math.Atan2(destinationPoint.Y - originPoint.Y, destinationPoint.X - originPoint.X));

            if (angle < 0)
            {
                angle += 360;
            }

            return angle;
        }

        /// <summary>
        /// Get distance between two points
        /// </summary>
        /// <param name="p1">first point</param>
        /// <param name="p2">second point</param>
        /// <returns>distance between two points <paramref name="p1"/> and <paramref name="p2"/></returns>
        public static float GetDistance(Vector2 p1, Vector2 p2)
        {
            Vector2 temp = p2 - p1;
            return temp.Length();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector2 GetProjection(float speed, float direction)
        {
            Vector2 temp = new Vector2();

            temp.X = (float)(speed * Math.Cos(MathHelper.ToRadians(direction)));
            temp.Y = (float)(speed * Math.Sin(MathHelper.ToRadians(direction)));

            return temp;
        }

        public static void Update(GameTime gameTime)
        {
            Input.Update(gameTime);
            SoundManager.Update();

            if (OGE.CurrentWorld != null)
            {
                OGE.CurrentWorld.Update(gameTime);
            }

            if (NextWorld != null)
            {
                CurrentWorld = NextWorld;
                CurrentWorld.Intialize();
                CurrentWorld.LoadContent();
                NextWorld = null;
            }

            OGE.WorldCamera.Update(gameTime);
            OGE.HUDCamera.Update(gameTime);

            CursorEntity.Update(gameTime);
        }

        public static void Draw(GameTime gameTime)
        {
            if (OGE.CurrentWorld != null)
            {
                OGE.CurrentWorld.Draw(gameTime);
            }

            CursorEntity.Draw(Input.GetMousePosition(OGE.HUDCamera));
        }

        public static void DrawHitbox(Vector2 position, HitboxMask mask, Color borderColor)
        {
            SpriteBatch.Begin();

            Vector2 cameraPosition = WorldCamera.ConvertToCamera(new Vector2(mask.Hitbox.X + position.X, mask.Hitbox.Y + position.Y));

            spriteBatch.Draw(pixelTest, new Rectangle((int)cameraPosition.X, (int)cameraPosition.Y, mask.Hitbox.Width, mask.Hitbox.Height), 
                borderColor);

            SpriteBatch.End();
        }

        public static void PauseGame()
        {
            if (OGE.CurrentWorld != null)
            {
                OGE.CurrentWorld.PauseGame();
            }
        }

        public static void UnPauseGame()
        {
            if (OGE.CurrentWorld != null)
            {
                OGE.CurrentWorld.UnPauseGame();
            }
        }
    }
}
