using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class SettingsAnnouncerEntity : AnnouncerEntity
    {
        private List<Button> buttons;

        public SettingsAnnouncerEntity(AnnouncerEnded endFunction, Color color, string title, List<ButtonPressed> buttons)
            : base(endFunction, 60 * buttons.Count + 60)
        {
            this.text = new Text(title, FontSize.Large);
            this.text.OriginX = this.text.Width / 2;
            
            this.buttons = new List<Button>();
            this.buttons.Add(new Button(color, "Music:", buttons[0]));
            this.buttons.Add(new Button(color, "Sound:", buttons[1]));
            this.buttons.Add(new Button(color, "Movement:", buttons[2]));
            this.buttons.Add(new Button(color, "FullScreen:", buttons[3]));
            this.buttons.Add(new Button(color, "Apply Settings", buttons[4]));

            UpdateButtonText();

            this.TintColor = color;
            for (int i = 0; i < this.buttons.Count; i++)
            {
                this.buttons[i].TintColor = color;
                this.buttons[i].Position.X = OGE.HUDCamera.Width / 2;
                this.buttons[i].Position.Y = OGE.HUDCamera.Height / 2 + maxHeight / 2 - (this.buttons.Count - i) * 60 + 20;
            }
        }

        public void UpdateButtonText()
        {
            buttons[0].TextContext = "Music: " + (Sounds.SoundManager.MusicOn ? "On" : "Off");
            buttons[1].TextContext = "Sound: " + (Sounds.SoundManager.SoundOn ? "On" : "Off");
            buttons[2].TextContext = "Movement: " + (GlobalVariables.Controls == Data.ControlType.WASD ? "WASD" : "Arrows");
            buttons[3].TextContext = "FullScreen: " + (GlobalVariables.IsFullScreen ? "On" : "Off");
        }

        public override void ChangePosition(int yShift)
        {
            base.ChangePosition(yShift);

            for (int i = 0; i < this.buttons.Count; i++)
            {
                this.buttons[i].Position.X = OGE.HUDCamera.Width / 2;
                this.buttons[i].Position.Y = OGE.HUDCamera.Height / 2 + maxHeight / 2 - (this.buttons.Count - i) * 60 + yShift + 20;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (status == AnnouncerStatus.Steady)
            {
                foreach (Button button in buttons)
                {
                    button.Update(gameTime);
                }
            }

            UpdateButtonText();
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            if (status == AnnouncerStatus.Steady)
            {
                text.Draw(new Vector2(camera.Width / 2, Position.Y + camera.Height / 2 - maxHeight / 2 + 10), camera);
                foreach (Button button in buttons)
                {
                    button.Draw(camera);
                }
            }
        }
    }
}
