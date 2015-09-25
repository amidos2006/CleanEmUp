using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class LevelNameAnnouncerEntity: TimeAnnouncerEntity
    {
        private Text levelNumberText;

        public override Color TintColor
        {
            get
            {
                return base.TintColor;
            }
            set
            {
                base.TintColor = value;
                levelNumberText.TintColor = value;
            }
        }

        public LevelNameAnnouncerEntity(AnnouncerEnded endFunction, string levelName, int levelNumber)
            : base(endFunction,100, 2, FontSize.Large)
        {
            this.levelNumberText = new Text("Sector " + levelNumber, FontSize.Small);
            this.levelNumberText.Align(AlignType.Center);

            TextContext = levelName;
            this.speed = 4;
            this.text.OriginX = this.text.Width / 2;
            this.text.OriginY = this.text.Height / 2;
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            if (status == AnnouncerStatus.Steady)
            {
                levelNumberText.Draw(new Vector2(camera.Width / 2, camera.Height / 2 - levelNumberText.Height / 2 - text.Height / 2 - 5), 
                    camera);
                text.Draw(new Vector2(camera.Width / 2, camera.Height / 2 + levelNumberText.Height / 2), camera);
            }
        }
    }
}
