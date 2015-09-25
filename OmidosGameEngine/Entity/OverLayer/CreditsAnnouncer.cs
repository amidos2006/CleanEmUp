using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using OmidosGameEngine.Data;

namespace OmidosGameEngine.Entity.OverLayer
{
    class CreditsAnnouncer : AnnouncerEntity
    {
        private List<Text> creditsText;
        private List<int> yPosition;

        public CreditsAnnouncer(AnnouncerEnded endFunction, Color color, string title, List<CreditData> credits)
            : base(endFunction, 0)
        {
            this.text = new Text(title, FontSize.Large);
            this.text.Align(AlignType.Center);

            this.maxHeight += this.text.Height + 10;

            this.hintText = new Text("Click to return to Main Console", FontSize.Small);
            this.hintText.Align(AlignType.Center);
            this.hintText.TintColor = color;

            this.maxHeight += this.hintText.Height + 20;

            this.creditsText = new List<Text>();
            this.yPosition = new List<int>();

            int oldMaxHeight = (int)this.maxHeight;
            for (int i = 0; i < credits.Count; i++)
            {

                this.yPosition.Add((int)(this.maxHeight - oldMaxHeight));

                this.creditsText.Add(new Text(credits[i].Title, FontSize.Small));
                this.creditsText[this.creditsText.Count - 1].Align(AlignType.Center);
                this.creditsText[this.creditsText.Count - 1].TintColor = color;

                this.maxHeight += this.creditsText[this.creditsText.Count - 1].Height + 3;

                for (int j = 0; j < credits[i].Names.Count; j++)
                {
                    this.yPosition.Add((int)(this.maxHeight - oldMaxHeight));
                    this.creditsText.Add(new Text(credits[i].Names[j], FontSize.Medium));
                    this.creditsText[this.creditsText.Count - 1].Align(AlignType.Center);
                    this.creditsText[this.creditsText.Count - 1].TintColor = color;

                    this.maxHeight += this.creditsText[this.creditsText.Count - 1].Height + 15;
                }
            }

            this.maxHeight += 15;
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
                for (int i = 0; i < creditsText.Count; i++)
                {
                    creditsText[i].Draw(tempPosition + new Vector2(0, yPosition[i]), camera);
                }

                hintText.Draw(new Vector2(camera.Width / 2, camera.Height / 2 + 
                    Position.Y + maxHeight / 2 - 5 - hintText.Height), camera);
            }
        }
    }
}
