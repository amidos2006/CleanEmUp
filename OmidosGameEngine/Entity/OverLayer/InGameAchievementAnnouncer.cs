using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class InGameAchievementAnnouncer:TimeAnnouncerEntity
    {
        public InGameAchievementAnnouncer(AnnouncerEnded endFunction, string achievementString, Color color)
            : base(endFunction, 0, 3.5f, Graphics.FontSize.Medium)
        {
            this.text.TextContext = achievementString;
            this.text.Align(Graphics.AlignType.Center);
            this.TintColor = color;
            this.speed = 5;

            this.maxHeight = this.text.Height + 20;
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            if (status == AnnouncerStatus.Steady)
            {
                text.Draw(new Vector2(camera.Width / 2, Position.Y + camera.Height / 2 - text.Height / 2), camera);
            }
        }
    }
}
