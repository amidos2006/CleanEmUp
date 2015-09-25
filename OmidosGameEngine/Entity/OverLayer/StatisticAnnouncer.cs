using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;

namespace OmidosGameEngine.Entity.OverLayer
{
    class StatisticAnnouncer : AnnouncerEntity
    {
        private List<Text> data;

        public StatisticAnnouncer(AnnouncerEnded endFunction, Color color, string title, List<string> names, 
            List<int> scores, List<string> units)
            : base(endFunction, 0)
        {
            this.text = new Text(title, FontSize.Large);
            this.text.Align(AlignType.Center);

            this.maxHeight += this.text.Height + 10;

            this.hintText = new Text("Click to return to Survival Console", FontSize.Small);
            this.hintText.Align(AlignType.Center);
            this.hintText.TintColor = color;

            this.maxHeight += this.hintText.Height + 20;

            this.data = new List<Text>();

            for (int i = 0; i < names.Count; i++)
            {
                this.data.Add(new Text(names[i] + ": " + scores[i] + " " + units[i], FontSize.Medium));

                this.data[i].Align(AlignType.Center);
                this.data[i].TintColor = color;

                this.maxHeight += this.data[i].Height + 10;
            }

            this.TintColor = color;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (status == AnnouncerStatus.Steady)
            {
                if (Input.CheckLeftMouseButton() == GameButtonState.Pressed)
                {
                    if (endFunction != null)
                    {
                        endFunction();
                    }
                }
            }
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            Vector2 tempPosition = new Vector2(camera.Width / 2, camera.Height / 2 + Position.Y - maxHeight / 2);

            if (status == AnnouncerStatus.Steady)
            {
                tempPosition += new Vector2(0,10);

                text.Draw(tempPosition, camera);

                tempPosition += new Vector2(0, text.Height + 15);
                for (int i = 0; i < data.Count; i++)
                {
                    data[i].Draw(tempPosition, camera);
                    tempPosition += new Vector2(0, data[i].Height + 5);
                }

                hintText.Draw(new Vector2(camera.Width / 2, camera.Height / 2 + Position.Y + maxHeight / 2 - 5 - hintText.Height), camera);
            }
        }
    }
}
