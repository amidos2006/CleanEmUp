using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class TitleTimeAnnouncerEntity : TimeAnnouncerEntity
    {
        public TitleTimeAnnouncerEntity(AnnouncerEnded endFunction, string text)
            : base(endFunction, 100, 2, FontSize.Large)
        {
            TextContext = text;
            this.speed = 4;
            this.text.OriginX = this.text.Width / 2;
            this.text.OriginY = this.text.Height / 2;
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            if (status == AnnouncerStatus.Steady)
            {
                text.Draw(new Vector2(camera.Width / 2, camera.Height / 2), camera);
            }
        }
    }
}
