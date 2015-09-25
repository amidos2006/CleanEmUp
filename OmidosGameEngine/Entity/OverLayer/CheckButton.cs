using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Collision;
using Microsoft.Xna.Framework.Graphics;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class CheckButton
    {
        private Image normalImage;
        private Image selectedImage;
        private Color color;
        private Text text;
        private WindowButtonState status;
        private List<ButtonPressed> pressedFunction;
        private Rectangle collision;

        public Vector2 Position;

        public bool Active
        {
            set;
            get;
        }

        public bool Selected
        {
            set;
            get;
        }

        public Color TintColor
        {
            set
            {
                normalImage.TintColor = value;
                selectedImage.TintColor = value;
            }
            get
            {
                return normalImage.TintColor;
            }
        }

        public int Width
        {
            get
            {
                return normalImage.Width;
            }
        }

        public int Height
        {
            get
            {
                return normalImage.Height;
            }
        }

        public CheckButton(Color color, string text, ButtonPressed pressedFunction)
        {
            this.Active = true;

            this.Position = new Vector2();
            this.status = WindowButtonState.Normal;

            Texture2D texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\WindowGraphics\CheckButtons");
            normalImage = new Image(texture, new Rectangle(0, 0, texture.Width, texture.Height / 2));
            selectedImage = new Image(texture, new Rectangle(0, texture.Height / 2, texture.Width, texture.Height / 2));
            normalImage.CenterOrigin();
            selectedImage.CenterOrigin();

            this.color = color;
            this.TintColor = color;
            this.text = new Text(text, FontSize.Medium);
            this.text.OriginX = this.text.Width / 2;
            this.text.OriginY = this.text.Height / 2;

            this.pressedFunction = new List<ButtonPressed>();
            this.pressedFunction.Add(pressedFunction);

            collision = new Rectangle(-normalImage.OriginX, -normalImage.OriginY, normalImage.Width, normalImage.Height);
        }

        public void Update(GameTime gameTime)
        {
            if (!Active)
            {
                status = WindowButtonState.Normal;
                Selected = false;
                return;
            }

            Vector2 mouseVector = Input.GetMousePosition(OGE.HUDCamera);
            Point mousePoint = new Point((int)mouseVector.X, (int)mouseVector.Y);
            Rectangle test = new Rectangle((int)(Position.X + collision.X), (int)(Position.Y + collision.Y), 
                collision.Width, collision.Height);

            if (test.Contains(mousePoint))
            {
                status = WindowButtonState.Over;
                if (Input.CheckLeftMouseButton() == GameButtonState.Pressed)
                {
                    foreach (ButtonPressed pressed in pressedFunction)
                    {
                        if (pressed != null)
                        {
                            pressed();
                        }
                    }
                    ToggleSelection();
                }
            }
            else
            {
                status = WindowButtonState.Normal;
            }

            if (Selected)
            {
                normalImage.Scale = 0.75f;
                selectedImage.Scale = 0.75f;
            }
            else
            {
                normalImage.Scale = 1f;
                selectedImage.Scale = 1f;
            }
        }

        public void ToggleSelection()
        {
            Selected = !Selected;
        }

        public void Draw(Camera camera)
        {
            if (!Active)
            {
                normalImage.TintColor = new Color(100, 100, 100);
                text.TintColor = new Color(100, 100, 100);

                normalImage.Draw(Position, camera);
                text.Draw(Position, camera);

                return;
            }

            normalImage.TintColor = color;
            text.TintColor = Color.White;

            if (status == WindowButtonState.Normal && !Selected)
            {
                normalImage.Draw(Position, camera);
            }
            else
            {
                selectedImage.Draw(Position, camera);
            }

            text.Draw(Position, camera);
        }
    }
}
