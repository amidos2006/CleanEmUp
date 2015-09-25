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
    public delegate void ButtonPressed();

    public class Button
    {
        private Image normalImage;
        private Image overImage;
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

        public Color TintColor
        {
            set
            {
                normalImage.TintColor = value;
                overImage.TintColor = value;
            }
            get
            {
                return normalImage.TintColor;
            }
        }

        public string TextContext
        {
            set
            {
                text.TextContext = value;
                this.text.OriginX = this.text.Width / 2;
                this.text.OriginY = this.text.Height / 2;
            }
            get
            {
                return text.TextContext;
            }
        }

        public Button(Color color, string text, ButtonPressed pressedFunction)
        {
            this.Active = true;

            this.Position = new Vector2();
            this.status = WindowButtonState.Normal;

            Texture2D texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\WindowGraphics\Buttons");
            normalImage = new Image(texture, new Rectangle(0, 0, texture.Width, texture.Height / 2));
            overImage = new Image(texture, new Rectangle(0, texture.Height / 2, texture.Width, texture.Height / 2));
            normalImage.CenterOrigin();
            overImage.CenterOrigin();

            this.color = color;
            this.TintColor = color;
            this.text = new Text(text, FontSize.Medium);
            this.text.OriginX = this.text.Width / 2;
            this.text.OriginY = this.text.Height / 2;
            this.pressedFunction = new List<ButtonPressed>();
            this.pressedFunction.Add(pressedFunction);

            collision = new Rectangle(-normalImage.OriginX, -normalImage.OriginY, normalImage.Width, normalImage.Height);
        }

        public void AddFunction(ButtonPressed pressed)
        {
            this.pressedFunction.Add(pressed);
        }

        public void Update(GameTime gameTime)
        {
            if (!Active)
            {
                status = WindowButtonState.Normal;
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
                }
            }
            else
            {
                status = WindowButtonState.Normal;
            }
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

            switch (status)
            {
                case WindowButtonState.Normal:
                    normalImage.Draw(Position, camera);
                    break;
                case WindowButtonState.Over:
                    overImage.Draw(Position, camera);
                    break;
            }

            text.Draw(Position, camera);
        }
    }
}
