using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Tween;

namespace OmidosGameEngine
{
    /// <summary>
    /// This class is resposible for holding 2D camera data (position, dimension of scene)
    /// Provides Functions to check if a point or rectangle or line in scene
    /// Convert point from world coordinates to camera coordinates
    /// </summary>
    public class Camera : ILogical
    {
        /// <summary>
        /// This is the camera rectangle
        /// </summary>
        private Rectangle camera;
        /// <summary>
        /// if true then the world is shacking
        /// </summary>
        private bool isShacking;
        /// <summary>
        /// the amount of pixel to shack
        /// </summary>
        private float shackingPower;
        /// <summary>
        /// Shack amount in everyframe
        /// </summary>
        private Vector2 shackAmount;
        /// <summary>
        /// alarm responsible for shacking
        /// </summary>
        private Alarm shackingAlarm;
        /// <summary>
        /// Random Object used to shack the scene
        /// </summary>
        private Random random;

        /// <summary>
        /// X position of the camera in the world
        /// </summary>
        public int X
        {
            set
            {
                camera.X = value;
                if (camera.X < 0)
                {
                    camera.X = 0;
                }
                else if (camera.X + camera.Width > WorldDimensions.X)
                {
                    camera.X = (int)(WorldDimensions.X - camera.Width);
                }
            }
            get
            {
                return camera.X;
            }
        }

        /// <summary>
        /// Y position of the camera in the world
        /// </summary>
        public int Y
        {
            set
            {
                camera.Y = value;
                if (camera.Y < 0)
                {
                    camera.Y = 0;
                }
                else if (camera.Y + camera.Height > WorldDimensions.Y)
                {
                    camera.Y = (int)(WorldDimensions.Y - camera.Height);
                }
            }
            get
            {
                return camera.Y;
            }
        }

        /// <summary>
        /// The Width of the Viewport
        /// </summary>
        public int Width
        {
            get
            {
                return camera.Width;
            }
        }

        /// <summary>
        /// The Height of the Viewport
        /// </summary>
        public int Height
        {
            get
            {
                return camera.Height;
            }
        }

        /// <summary>
        /// Dimensions that the camera can't go futher it
        /// </summary>
        public Vector2 WorldDimensions
        {
            set;
            get;
        }

        /// <summary>
        /// Camera Constructor
        /// </summary>
        /// <param name="width">the width of the viewPort</param>
        /// <param name="height">the height of the viewPort</param>
        public Camera(int width, int height)
        {
            camera = new Rectangle(0, 0, width, height);
            WorldDimensions = new Vector2(width, height);

            isShacking = false;
            shackingPower = 0;
            shackAmount = new Vector2();
            shackingAlarm = new Alarm(0, TweenType.OneShot, new AlarmFinished(StopShacking));
            random = new Random();
        }

        /// <summary>
        /// Stop Shacking the camera
        /// </summary>
        private void StopShacking()
        {
            isShacking = false;
            shackingPower = 0;
        }

        /// <summary>
        /// Convert a point in the world to its position with respect to camera
        /// </summary>
        /// <param name="worldPoint">point in world coordinates</param>
        /// <param name="relativeSpeed">speed by which the world point affected by the camera</param>
        /// <returns>position of the point with respect to the camera</returns>
        public Vector2 ConvertToCamera(Vector2 worldPoint, float relativeSpeed = 1)
        {
            Vector2 tempPoint = new Vector2();
            tempPoint.X = worldPoint.X - camera.X * relativeSpeed;
            tempPoint.Y = worldPoint.Y - camera.Y * relativeSpeed;

            if (isShacking)
            {
                tempPoint.X += shackAmount.X;
                tempPoint.Y += shackAmount.Y;
            }

            return tempPoint;
        }

        /// <summary>
        /// Convert a point in the camera to its position in the world
        /// </summary>
        /// <param name="cameraPoint">point in world coordinates</param>
        /// <returns>position of the point with respect to the world</returns>
        public Vector2 ConvertToWorld(Vector2 cameraPoint)
        {
            Vector2 tempPoint = new Vector2();
            tempPoint.X = cameraPoint.X + camera.X;
            tempPoint.Y = cameraPoint.Y + camera.Y;

            return tempPoint;
        }

        /// <summary>
        /// Check if the point lies inside the camera
        /// </summary>
        /// <param name="worldPoint">point in world coordinates</param>
        /// <returns>true when the points inside the camera, false otherwise</returns>
        public bool CheckPointInCamera(Vector2 worldPoint)
        {
            return camera.Intersects(new Rectangle((int)worldPoint.X, (int)worldPoint.Y, 1, 1));
        }

        /// <summary>
        /// Check if the rectangle falls in the camera
        /// </summary>
        /// <param name="worldRectangle">the rectangle where x and y in world coordinates</param>
        /// <returns>true if the rectangle falls inside the camera, false otherwise</returns>
        public bool CheckRectangleInCamera(Rectangle worldRectangle)
        {
            return camera.Intersects(worldRectangle);
        }

        /// <summary>
        /// Check if line falls in the camera
        /// </summary>
        /// <param name="startPoint">starting position of the line in world coordinates</param>
        /// <param name="endPoint">ending position of the line in world coordinates</param>
        /// <returns>true if line falls inside the camera, false otherwise</returns>
        public bool CheckLineInCamera(Vector2 startPoint, Vector2 endPoint)
        {
            Rectangle worldRectangle = new Rectangle();

            worldRectangle.X = (int)Math.Min(startPoint.X, endPoint.X);
            worldRectangle.Y = (int)Math.Min(startPoint.Y, endPoint.Y);
            worldRectangle.Width = (int)Math.Abs(startPoint.X - endPoint.X);
            worldRectangle.Height = (int)Math.Abs(startPoint.Y - endPoint.Y);

            return camera.Intersects(worldRectangle);
        }

        /// <summary>
        /// Shack the camera by amount
        /// </summary>
        /// <param name="shackingPower">amount of shacking</param>
        /// <param name="duration">time of shacking</param>
        public void ShackCamera(float shackingPower, float duration)
        {
            this.shackingPower = Math.Max(shackingPower, this.shackingPower);
            this.isShacking = true;
            shackingAlarm.Reset(duration);
            shackingAlarm.Start();
        }

        /// <summary>
        /// Update the camera if shacking
        /// </summary>
        /// <param name="gameTime">XNA game time object</param>
        public void Update(GameTime gameTime)
        {
            shackingAlarm.Update(gameTime);
            shackAmount.X = (float)((random.NextDouble() - 0.5) * shackingPower);
            shackAmount.Y = (float)((random.NextDouble() - 0.5) * shackingPower);
        }
    }
}
