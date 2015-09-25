using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Tween;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class LogoEntity : BaseEntity
    {
        private AnnouncerStatus status;
        private Image logoBackgroundImage;
        private Image oImage;
        private Image omidosImage;
        private float currentValue;
        private float currentSpeed;
        private Alarm waitingAlarm;
        private Action endFunction;

        public LogoEntity(Action endFunction)
        {
            Position.X = OGE.HUDCamera.Width / 2.0f;
            Position.Y = OGE.HUDCamera.Height / 2.0f;

            currentSpeed = 0.03f;
            currentValue = 0;
            status = AnnouncerStatus.Appearing;
            this.endFunction = endFunction;

            logoBackgroundImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Intro\LogoBack"));
            logoBackgroundImage.CenterOrigin();
            logoBackgroundImage.TintColor = Color.White * currentValue;

            oImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Intro\OLogo"));
            oImage.CenterOrigin();
            oImage.TintColor = Color.White * currentValue;

            omidosImage = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Intro\OmidosLogo"));
            omidosImage.CenterOrigin();
            omidosImage.TintColor = Color.White * currentValue;

            waitingAlarm = new Alarm(2f, TweenType.OneShot, Disappear);
            AddTween(waitingAlarm);

            CurrentImages.Add(logoBackgroundImage);
            CurrentImages.Add(oImage);
            CurrentImages.Add(omidosImage);
        }

        private void Disappear()
        {
            status = AnnouncerStatus.Disappearing;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (status)
            {
                case AnnouncerStatus.Appearing:
                    currentValue += currentSpeed;

                    logoBackgroundImage.TintColor = Color.White * currentValue;
                    oImage.TintColor = Color.White * currentValue;
                    omidosImage.TintColor = Color.White * currentValue;

                    if(currentValue >= 1)
                    {
                        currentValue = 1;
                        status = AnnouncerStatus.Steady;
                        waitingAlarm.Start();
                    }
                    break;
                case AnnouncerStatus.Disappearing:
                    currentValue -= currentSpeed;

                    logoBackgroundImage.TintColor = Color.White * currentValue;
                    oImage.TintColor = Color.White * currentValue;
                    omidosImage.TintColor = Color.White * currentValue;

                    if (currentValue <= 0)
                    {
                        currentValue = 0;
                        if (endFunction != null)
                        {
                            endFunction();
                        }
                    }
                    break;
            }
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);
        }
    }
}
