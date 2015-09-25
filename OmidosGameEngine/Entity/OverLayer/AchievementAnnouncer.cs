using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;
using OmidosGameEngine.Data;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class AchievementAnnouncer : AnnouncerEntity
    {
        private List<Text> openBracket;
        private List<Text> achieved;
        private List<Text> achievementName;
        private List<Text> currentNumber;

        public AchievementAnnouncer(AnnouncerEnded endFunction, Color color)
            : base(endFunction, 0)
        {
            this.text = new Text("Achievement Console", FontSize.Large);
            this.text.Align(AlignType.Center);
            this.text.TintColor = color;

            this.TintColor = color;

            this.openBracket = new List<Text>();
            this.achieved = new List<Text>();
            this.achievementName = new List<Text>();
            this.currentNumber = new List<Text>();

            foreach (KeyValuePair<Type,AchievementData> item in GlobalVariables.Achievements)
            {
                this.openBracket.Add(new Text("[ ", FontSize.Medium));

                if (item.Value.Achieved)
                {
                    this.achieved.Add(new Text("X", FontSize.Medium));
                }
                else
                {
                    this.achieved.Add(new Text(" ", FontSize.Medium));
                }

                this.achievementName.Add(new Text(" ] " + item.Value.Name + ": " + item.Value.Description, FontSize.Medium));
                this.currentNumber.Add(new Text(item.Value.CurrentNumber.ToString(), FontSize.Medium));
            }

            for (int i = 0; i < this.openBracket.Count; i++)
            {
                this.openBracket[i].TintColor = color;
                this.achieved[i].TintColor = color;
                this.achieved[i].Align(AlignType.Center);
                this.achievementName[i].TintColor = color;
                this.currentNumber[i].TintColor = color;
                this.currentNumber[i].Align(AlignType.Right);
            }

            this.hintText = new Text("Click to return to Drive Console", FontSize.Small);
            this.hintText.Align(AlignType.Center);
            this.hintText.TintColor = color;

            this.maxHeight = this.hintText.Height + this.text.Height + 
                this.openBracket.Count * (this.openBracket[0].Height + 5) + 40;
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

            Vector2 tempPosition = new Vector2(0, camera.Height / 2 + Position.Y - maxHeight / 2);

            if (status == AnnouncerStatus.Steady)
            {
                tempPosition += new Vector2(0, 10);

                text.Draw(tempPosition + new Vector2(camera.Width / 2.0f, 0), camera);

                tempPosition += new Vector2(0, text.Height + 15);
                for (int i = 0; i < openBracket.Count; i++)
                {
                    openBracket[i].Draw(tempPosition + new Vector2(80, 0), camera);
                    achieved[i].Draw(tempPosition + new Vector2(openBracket[i].Width + 90, 0), camera);
                    achievementName[i].Draw(tempPosition + new Vector2(openBracket[i].Width + 100, 0), camera);
                    currentNumber[i].Draw(tempPosition + new Vector2(camera.Width - 80, 0), camera);

                    tempPosition += new Vector2(0, openBracket[i].Height + 5);
                }

                hintText.Draw(new Vector2(camera.Width / 2, camera.Height / 2 + Position.Y + maxHeight / 2 - 5 - hintText.Height), camera);
            }
        }
    }
}
