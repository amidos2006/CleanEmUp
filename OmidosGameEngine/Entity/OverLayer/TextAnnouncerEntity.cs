using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class TextAnnouncerEntity:AnnouncerEntity
    {
        private Text titleText;
        private Text inlineText;
        private List<Text> drawableInLineText;
        private string totalText;
        private float currentText;
        private float textSpeed;

        public TextAnnouncerEntity(AnnouncerEnded endFunction, Color color, string title, string text, int width, float speed)
            : base(endFunction, 1)
        {
            this.text = new Text("", FontSize.Small);
            this.TintColor = color;

            titleText = new Text(title, FontSize.Large);
            inlineText = new Text(text[0] + "", FontSize.Medium);
            hintText = new Text("Click to continue", FontSize.Small);

            titleText.Align(AlignType.Center);
            hintText.Align(AlignType.Center);

            titleText.TintColor = color;
            hintText.TintColor = color;

            currentText = 1;
            totalText = Text.GetText(text, (int)(width / inlineText.Width));
            maxHeight = totalText.Split('\n').Length * inlineText.Height + titleText.Height + hintText.Height + 50;

            string[] separatedStrings = totalText.Split('\n');
            drawableInLineText = new List<Text>();
            for (int i = 0; i < separatedStrings.Length; i++)
            {
                drawableInLineText.Add(new Text("", FontSize.Medium));
                drawableInLineText[drawableInLineText.Count - 1].Align(AlignType.Center);
                drawableInLineText[drawableInLineText.Count - 1].TintColor = color;
            }

            this.textSpeed = speed;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            currentText += textSpeed;
            if (currentText >= totalText.Length)
            {
                currentText = totalText.Length;
            }

            string[] separatedStrings = totalText.Substring(0, (int)currentText).Split('\n');
            for (int i = 0; i < separatedStrings.Length; i++)
            {
                drawableInLineText[i].TextContext = separatedStrings[i];
                drawableInLineText[i].Align(AlignType.Center);
            }

            if (currentText < totalText.Length)
            {
                drawableInLineText[separatedStrings.Length - 1].TextContext += "_";
            }

            if (Input.CheckLeftMouseButton() == GameButtonState.Pressed)
            {
                if (currentText >= totalText.Length)
                {
                    if (endFunction != null)
                    {
                        endFunction();
                    }
                }
                else
                {
                    currentText = totalText.Length;
                }
            }
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            if (status == AnnouncerStatus.Steady)
            {
                titleText.Draw(new Vector2(camera.Width/2, Position.Y + camera.Height / 2 - height / 2), camera);
                
                for (int i = 0; i < drawableInLineText.Count; i++)
                {
                    drawableInLineText[i].Draw(new Vector2(camera.Width / 2, 
                        Position.Y + camera.Height / 2 - height / 2 + titleText.Height + 20 + i * drawableInLineText[i].Height), camera);
                }

                hintText.Draw(new Vector2(camera.Width / 2, Position.Y + camera.Height / 2 + height / 2 - hintText.Height - 5), camera);
            }
        }
    }
}
