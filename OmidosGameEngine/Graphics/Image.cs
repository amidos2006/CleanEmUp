using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OmidosGameEngine.Graphics
{
    public class Image : IGraphics, ILogical
    {
        /// <summary>
        /// texture to be drawn in the Draw
        /// </summary>
        protected Texture2D texture;
        /// <summary>
        /// the sourceRectangle that is used in drawing
        /// </summary>
        protected Rectangle? sourceRectangle;
        /// <summary>
        /// the origin of the image
        /// </summary>
        protected Vector2 origin;
        /// <summary>
        /// the angle at which image appear
        /// </summary>
        protected float angle;
        /// <summary>
        /// the image scale
        /// </summary>
        protected Vector2 scale;
        /// <summary>
        /// more efficient image flipping
        /// </summary>
        protected SpriteEffects flipping;
        /// <summary>
        /// Tint color that affects the texture
        /// </summary>
        protected Color tintColor;

        /// <summary>
        /// width of the image
        /// </summary>
        public int Width
        {
            get
            {
                if (sourceRectangle != null)
                {
                    return ((Rectangle)sourceRectangle).Width;
                }

                return texture.Width;
            }
        }

        /// <summary>
        /// height of the image
        /// </summary>
        public int Height
        {
            get
            {
                if (sourceRectangle != null)
                {
                    return ((Rectangle)sourceRectangle).Height;
                }

                return texture.Height;
            }
        }

        /// <summary>
        /// X origin of the image
        /// </summary>
        public int OriginX
        {
            set
            {
                origin.X = value;
            }
            get
            {
                return (int)origin.X;
            }
        }

        /// <summary>
        /// Y origin of the image
        /// </summary>
        public int OriginY
        {
            set
            {
                origin.Y = value;
            }
            get 
            {
                return (int)origin.Y;
            }
        }

        /// <summary>
        /// set the scale of the object
        /// </summary>
        public float Scale
        {
            set
            {
                scale.X = value;
                scale.Y = value;
            }
        }

        /// <summary>
        /// X scale factor of the image
        /// </summary>
        public float ScaleX
        {
            set
            {
                scale.X = value;
            }
            get
            {
                return scale.X;
            }
        }

        /// <summary>
        /// Y scale factor of the image
        /// </summary>
        public float ScaleY
        {
            set
            {
                scale.Y = value;
            }
            get
            {
                return scale.Y;
            }
        }

        /// <summary>
        /// Angle of image in degrees
        /// </summary>
        public float Angle
        {
            set
            {
                angle = MathHelper.ToRadians(value);
            }
            get
            {
                return (int)MathHelper.ToDegrees(angle);
            }
        }

        /// <summary>
        /// Flip the image vertically if true
        /// </summary>
        public bool Flipped
        {
            set
            {
                flipping = SpriteEffects.None;
                if (value)
                {
                    flipping = SpriteEffects.FlipVertically;
                }
            }
            get
            {
                return flipping == SpriteEffects.FlipVertically;
            }
        }

        /// <summary>
        /// Tint color affecting the color applied to the texture
        /// </summary>
        public Color TintColor
        {
            set
            {
                tintColor = value;
            }
            get
            {
                return tintColor;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return texture;
            }
        }

        public Rectangle? SourceRectangle
        {
            set
            {
                sourceRectangle = value;
            }
            get
            {
                return sourceRectangle;
            }
        }

        /// <summary>
        /// Constructor for image class
        /// </summary>
        /// <param name="sourceRectangle">the source rectangle for the image null means all the texture</param>
        public Image(Texture2D texture, Rectangle? sourceRectangle = null)
        {
            this.texture = texture;
            this.sourceRectangle = sourceRectangle;
            this.origin = new Vector2();
            this.scale = new Vector2(1, 1);
            this.angle = 0;
            this.flipping = SpriteEffects.None;
            this.tintColor = Color.White;
        }

        /// <summary>
        /// make the origin in the center of the image
        /// </summary>
        public void CenterOrigin()
        {
            origin.X = Width / 2;
            origin.Y = Height / 2;
        }

        /// <summary>
        /// Updates the image parameters
        /// </summary>
        /// <param name="gameTime">GameTime object supported by XNA</param>
        public virtual void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Draw the texture in the correct position
        /// </summary>
        /// <param name="position">position of the image to be drawn at</param>
        /// <param name="camera">camera position in the scene</param>
        public virtual void Draw(Vector2 position, Camera camera)
        {
            SpriteBatch spriteBatch = OGE.SpriteBatch;

            Vector2 leftUpperCorner = OGE.GetLeftUpperCorner(position, origin);

            if (!camera.CheckRectangleInCamera(new Rectangle((int)leftUpperCorner.X, (int)leftUpperCorner.Y, Width, Height)))
            {
                return;
            }

            spriteBatch.Begin(OGE.SpriteSortMode, OGE.BlendState);

            spriteBatch.Draw(texture, camera.ConvertToCamera(position), sourceRectangle, tintColor, angle, origin, scale, flipping, 0);

            spriteBatch.End();
        }
    }
}
