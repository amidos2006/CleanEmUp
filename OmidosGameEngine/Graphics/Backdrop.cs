using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Graphics
{
    public class Backdrop : Image
    {
        /// <summary>
        /// the relative velocity that background move with respect to camera value (0,1)
        /// </summary>
        private float relativeSpeed;
        /// <summary>
        /// the image will repeat in x direction
        /// </summary>
        private bool repeatX;
        /// <summary>
        /// the image will repeat in y direction
        /// </summary>
        private bool repeatY;

        /// <summary>
        /// the relative velocity that background move with respect to camera value (0,1)
        /// </summary>
        public float RelativeSpeed
        {
            set
            {
                relativeSpeed = value;
                if (relativeSpeed <= 0)
                {
                    relativeSpeed = 0;
                }
                else if (relativeSpeed >= 1)
                {
                    relativeSpeed = 1;
                }

            }
            get
            {
                return relativeSpeed;
            }
        }

        /// <summary>
        /// Constructor for Backdrop class
        /// </summary>
        /// <param name="texture">backdrop texture used in drawing</param>
        /// <param name="repeatX">the backdrop is repeated in x direction</param>
        /// <param name="repeatY">the backdrop is repeated in y direction</param>
        public Backdrop(Texture2D texture, bool repeatX = true, bool repeatY = true)
            : base(texture, new Rectangle(0, 0, texture.Width, texture.Height))
        {
            this.tintColor = Color.White;
            this.relativeSpeed = 1;
            this.repeatX = repeatX;
            this.repeatY = repeatY;
        }

        /// <summary>
        /// Draw the backdrop on the screen
        /// </summary>
        /// <param name="position"> starting position of the background</param>
        /// <param name="camera">camera object</param>
        public override void Draw(Vector2 position, Camera camera)
        {
            SpriteBatch spriteBatch = OGE.SpriteBatch;

            Vector2 tempPosition = new Vector2();
            Vector2 startingPosition = new Vector2();
            Point textureDimension = sourceRectangle == null ? new Point(texture.Width, texture.Height) : 
                new Point(sourceRectangle.Value.Width, sourceRectangle.Value.Height);
            int xLoop = 1;
            int yLoop = 1;

            startingPosition = camera.ConvertToCamera(position, relativeSpeed);

            if (repeatX)
            {
                xLoop = (camera.Width) / textureDimension.X + 2;
                startingPosition.X = OGE.NumberWrap((int)(startingPosition.X + textureDimension.X), 0, textureDimension.X)
                    - textureDimension.X;
            }

            if (repeatY)
            {
                yLoop = (camera.Height) / textureDimension.Y + 2;
                startingPosition.Y = OGE.NumberWrap((int)(startingPosition.Y + textureDimension.Y), 0, textureDimension.Y)
                    - textureDimension.Y;
            }

            tempPosition.X = startingPosition.X;
            tempPosition.Y = startingPosition.Y;

            spriteBatch.Begin(OGE.SpriteSortMode, OGE.BlendState);

            for (int y = 0; y < yLoop; y++)
            {
                for (int x = 0; x < xLoop; x++)
                {
                    spriteBatch.Draw(texture, tempPosition, sourceRectangle, tintColor);
                    tempPosition.X += textureDimension.X;
                }
                tempPosition.Y += textureDimension.Y;
                tempPosition.X = startingPosition.X;
            }

            spriteBatch.End();
        }
    }
}
