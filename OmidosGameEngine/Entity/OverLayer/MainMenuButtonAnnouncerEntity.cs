using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class MainMenuButtonAnnouncerEntity : AnnouncerEntity
    {
        private List<Button> buttons;

        public MainMenuButtonAnnouncerEntity(AnnouncerEnded endFunction, Color color, string title, List<ButtonPressed> buttons)
            : base(endFunction, 60 * buttons.Count + 60)
        {
            this.text = new Text(title, FontSize.Large);
            this.text.OriginX = this.text.Width / 2;
            
            this.buttons = new List<Button>();
            this.buttons.Add(new Button(color, "Start New Scan", buttons[0]));
            this.buttons.Add(new Button(color, "Continue Scanning", buttons[1]));
            this.buttons.Add(new Button(color, "Settings", buttons[2]));
            this.buttons.Add(new Button(color, "Credits", buttons[3]));
            this.buttons.Add(new Button(color, "Stop Scanning", buttons[4]));

            this.buttons[1].Active = GlobalVariables.SaveExists();

            this.TintColor = color;
            for (int i = 0; i < this.buttons.Count; i++)
            {
                this.buttons[i].TintColor = color;
                this.buttons[i].Position.X = OGE.HUDCamera.Width / 2;
                this.buttons[i].Position.Y = OGE.HUDCamera.Height / 2 + maxHeight / 2 - (this.buttons.Count - i) * 60 + 20;
            }
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
