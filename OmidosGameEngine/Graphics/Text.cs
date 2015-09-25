using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OmidosGameEngine.Graphics
{
    public class Text : IGraphics, ILogical
    {

        protected SpriteFont spriteFont;
        protected string text;
        protected AlignType alignType;
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

        public string TextContext
        {
            set
            {
                text = value;

                Align(alignType);
            }
            get
            {
                return text;
            }
        }

        /// <summary>
        /// width of the image
        /// </summary>
        public float Width
        {
            get
            {
                return spriteFont.MeasureString(text).X;
            }
        }

        /// <summary>
        /// height of the image
        /// </summary>
        public float Height
        {
            get
            {
                return spriteFont.MeasureString(text).Y;
            }
        }

        /// <summary>
        /// X origin of the image
        /// </summary>
        public float OriginX
        {
            set
            {
                origin.X = value;
            }
            get
            {
                return origin.X;
            }
        }

        /// <summary>
        /// Y origin of the image
        /// </summary>
        public float OriginY
        {
            set
            {
                origin.Y = value;
            }
            get 
            {
                return origin.Y;
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

        public Text(string text, FontSize size)
        {
            this.text = text;
            this.alignType = AlignType.Left;
            ChangeFont(size);
            this.origin = new Vector2();
            this.scale = new Vector2(1, 1);
            this.angle = 0;
            this.flipping = SpriteEffects.None;
            this.tintColor = Color.White;
        }

        public static string GetText(string text, int numberOfCharacters)
        {
            string separatedString = "";
            int tempNumber = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (tempNumber > numberOfCharacters)
                {
                    tempNumber = 0;
                    separatedString += "\n";
                }
                separatedString += text[i];
            }

            return separatedString;
        }

        public void ChangeFont(FontSize size)
        {
            switch (size)
            {
                case FontSize.Small:
                    spriteFont = OGE.Content.Load<SpriteFont>(@"Fonts\small");
                    break;
                case FontSize.Medium:
                    spriteFont = OGE.Content.Load<SpriteFont>(@"Fonts\medium");
                    break;
                case FontSize.Large:
                    spriteFont = OGE.Content.Load<SpriteFont>(@"Fonts\large");
                    break;
                case FontSize.XLarge:
                    spriteFont = OGE.Content.Load<SpriteFont>(@"Fonts\extralarge");
                    break;
            }

            Align(alignType);
        }

        /// <summary>
        /// make the origin in the center of the image
        /// </summary>
        public void Align(AlignType type)
        {
            alignType = type;

            switch (type)
            {
                case AlignType.Left:
                    OriginX = 0;
                    break;
                case AlignType.Right:
                    OriginX = Width;
                    break;
                case AlignType.Center:
                    OriginX = Width / 2;
                    break;
            }
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

            if (!camera.CheckRectangleInCamera(new Rectangle((int)leftUpperCorner.X, (int)leftUpperCorner.Y, (int)Width, (int)Height)))
            {
                return;
            }

            spriteBatch.Begin(OGE.SpriteSortMode, OGE.BlendState);

            spriteBatch.DrawString(spriteFont, text, camera.ConvertToCamera(position), tintColor, angle, origin, scale, flipping, 0);

            spriteBatch.End();
        }
    }
}
