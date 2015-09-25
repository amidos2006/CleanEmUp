using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OmidosGameEngine.Entity.OverLayer
{
    public delegate void AnnouncerEnded();

    public class AnnouncerEntity:BaseEntity
    {
        protected AnnouncerEnded endFunction;
        protected Text text;
        protected Text hintText;
        protected Image borderImage;
        protected Image bodyImage;
        protected float maxHeight;
        protected float height;
        protected float speed;
        protected AnnouncerStatus status;

        public string TextContext
        {
            set
            {
                text.TextContext = value;
            }
            get
            {
                return text.TextContext;
            }
        }

        public Action EscapeHandler
        {
            set;
            get;
        }

        public virtual Color TintColor
        {
            set
            {
                borderImage.TintColor = value;
                bodyImage.TintColor = value;
                text.TintColor = value;
            }
            get
            {
                return borderImage.TintColor;
            }
        }

        public AnnouncerEntity(AnnouncerEnded endFunction, float height)
        {
            this.endFunction = endFunction;
            this.maxHeight = height;

            this.EscapeHandler = null;

            this.status = AnnouncerStatus.Appearing;
            this.speed = 10;

            this.borderImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\WindowGraphics\Border"));
            this.bodyImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\WindowGraphics\Body"));

            this.borderImage.ScaleX = OGE.HUDCamera.Width / this.borderImage.Width;
            this.bodyImage.ScaleX = OGE.HUDCamera.Width / this.bodyImage.Width;
        }

        public virtual void ChangePosition(int yShift)
        {
            Position.Y = yShift;
        }

        public void FinishAnnouncer()
        {
            status = AnnouncerStatus.Disappearing;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (status)
            {
                case AnnouncerStatus.Appearing:
                    height += speed;

                    if (Input.CheckKeyboardButton(Keys.Escape) == GameButtonState.Pressed)
                    {
                        height = maxHeight;
                    }

                    if (height >= maxHeight)
                    {
                        height = maxHeight;
                        status = AnnouncerStatus.Steady;
                    }
                    break;
                case AnnouncerStatus.Steady:
                    if (Input.CheckKeyboardButton(Keys.Escape) == GameButtonState.Pressed)
                    {
                        if (EscapeHandler != null)
                        {
                            EscapeHandler();
                        }
                    }
                    break;
                case AnnouncerStatus.Disappearing:
                    height -= speed;
                    if (height <= 0)
                    {
                        height = 0;
                        if (endFunction != null)
                        {
                            endFunction();
                        }
                        OGE.CurrentWorld.RemoveOverLayer(this);
                    }
                    break;
            }

            bodyImage.ScaleY = height;
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            borderImage.Draw(new Vector2(0, Position.Y + camera.Height / 2 - height / 2 - borderImage.Height), OGE.HUDCamera);
            bodyImage.Draw(new Vector2(0, Position.Y + camera.Height / 2 - height / 2), OGE.HUDCamera);
            borderImage.Draw(new Vector2(0, Position.Y + camera.Height / 2 + height / 2), OGE.HUDCamera);
        }
    }
}
