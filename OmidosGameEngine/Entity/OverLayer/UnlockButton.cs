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
    public enum UnlockButtonType
    {
        TopCharacter,
        TopWeapon,
        BottomCharacter,
        BottomWeapon
    }

    public delegate void UnlockButtonPressed(int number, UnlockButtonType type);

    public class UnlockButton
    {
        private Image normalImage;
        private Image selectedImage;
        private Color unlockColor;
        private Color affordableColor;
        private Color lockColor;
        private Image frontImage;
        private WindowButtonState status;
        private List<UnlockButtonPressed> pressFunctions;
        private List<UnlockButtonPressed> lockFunctions;
        private List<UnlockButtonPressed> overFunctions;
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

        public bool Locked
        {
            set;
            get;
        }

        public bool Affordable
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

        public int ButtonNumber
        {
            set;
            get;
        }

        public UnlockButtonType ButtonType
        {
            set;
            get;
        }

        public UnlockButton(Color unlockColor, Color affordableColor, Color lockColor, Image image, UnlockButtonPressed pressFunction, UnlockButtonPressed unlockFunction, UnlockButtonPressed overFunction)
        {
            this.Active = true;

            this.Position = new Vector2();
            this.status = WindowButtonState.Normal;

            Texture2D texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\WindowGraphics\UnlockButtons");
            normalImage = new Image(texture, new Rectangle(0, 0, texture.Width, texture.Height / 2));
            selectedImage = new Image(texture, new Rectangle(0, texture.Height / 2, texture.Width, texture.Height / 2));
            normalImage.CenterOrigin();
            selectedImage.CenterOrigin();

            this.unlockColor = unlockColor;
            this.affordableColor = affordableColor;
            this.lockColor = lockColor;

            this.frontImage = image;
            this.frontImage.CenterOrigin();

            this.pressFunctions = new List<UnlockButtonPressed>();
            this.pressFunctions.Add(pressFunction);

            this.lockFunctions = new List<UnlockButtonPressed>();
            this.lockFunctions.Add(unlockFunction);

            this.overFunctions = new List<UnlockButtonPressed>();
            this.overFunctions.Add(overFunction);

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
                foreach (UnlockButtonPressed pressed in overFunctions)
                {
                    if (pressed != null)
                    {
                        pressed(ButtonNumber, ButtonType);
                    }
                }

                if (Input.CheckLeftMouseButton() == GameButtonState.Pressed)
                {
                    if (!Locked)
                    {
                        foreach (UnlockButtonPressed pressed in pressFunctions)
                        {
                            if (pressed != null)
                            {
                                pressed(ButtonNumber, ButtonType);
                            }
                        }
                    }
                    else
                    {
                        foreach (UnlockButtonPressed pressed in lockFunctions)
                        {
                            if (pressed != null)
                            {
                                pressed(ButtonNumber, ButtonType);
                            }
                        }
                    }
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
                frontImage.Scale = 0.75f;
            }
            else
            {
                normalImage.Scale = 1f;
                selectedImage.Scale = 1f;
                frontImage.Scale = 1f;
            }

            if (Locked)
            {
                if (!Affordable)
                {
                    normalImage.TintColor = lockColor;
                    selectedImage.TintColor = lockColor;
                    frontImage.TintColor = lockColor;
                }
                else
                {
                    normalImage.TintColor = affordableColor;
                    selectedImage.TintColor = affordableColor;
                    frontImage.TintColor = affordableColor;
                }
            }
            else
            {
                normalImage.TintColor = unlockColor;
                selectedImage.TintColor = unlockColor;
                frontImage.TintColor = unlockColor;
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
                frontImage.TintColor = new Color(100, 100, 100);

                normalImage.Draw(Position, camera);
                frontImage.Draw(Position, camera);

                return;
            }

            if (status == WindowButtonState.Normal && !Selected)
            {
                normalImage.Draw(Position, camera);
            }
            else
            {
                selectedImage.Draw(Position, camera);
            }

            frontImage.Draw(Position, camera);
        }
    }
}
