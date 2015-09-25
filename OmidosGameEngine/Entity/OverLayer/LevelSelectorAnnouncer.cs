using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Data;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class LevelSelectorAnnouncer : AnnouncerEntity
    {
        private List<CheckButton> levels;
        private Button playButton;
        private Button backButton;
        private Text levelDataText;
        private int selectedLevel;

        public LevelSelectorAnnouncer(AnnouncerEnded endFunction,Color color, ButtonPressed playPressed, ButtonPressed backPressed)
            : base(endFunction, (float)(Math.Ceiling(LevelData.MAX_LEVEL_DRIVE_NUMBER / 5.0) * 110 + 200))
        {
            text = new Text("Sector Console", FontSize.Large);
            text.Align(AlignType.Center);

            levels = new List<CheckButton>();

            int rowNumber = (int)Math.Ceiling(LevelData.MAX_LEVEL_DRIVE_NUMBER / 5.0);
            for (int i = 0; i < rowNumber; i++)
            {
                int coloumNumber = Math.Min(5, LevelData.MAX_LEVEL_DRIVE_NUMBER - i * 5);
                for (int j = 0; j < coloumNumber; j++)
                {
                    levels.Add(new CheckButton(color, (i * 5 + j + 1).ToString(), new ButtonPressed(ClearSelection)));
                    levels[levels.Count - 1].Position.X = OGE.HUDCamera.Width / 2 + (j - coloumNumber / 2.0f) * 110 + 50;
                    levels[levels.Count - 1].Position.Y = OGE.HUDCamera.Height / 2 + (i - rowNumber / 2.0f) * 110;
                    levels[levels.Count - 1].Selected = false;
                    levels[levels.Count - 1].Active = !GlobalVariables.LockedLevels[(GlobalVariables.CurrentDrive - 1) *
                        LevelData.MAX_LEVEL_DRIVE_NUMBER + i * 5 + j];
                }
            }

            levels[GlobalVariables.CurrentLevel - 1].Selected = true;
            TintColor = color;

            backButton = new Button(color, "Return to Drive Console", backPressed);
            backButton.Position.X = OGE.HUDCamera.Width / 2;
            backButton.Position.Y = OGE.HUDCamera.Height / 2 + maxHeight / 2 - 35;

            playButton = new Button(color, "Select Sector", playPressed);
            playButton.Position.X = backButton.Position.X;
            playButton.Position.Y = backButton.Position.Y - 60;

            selectedLevel = GlobalVariables.CurrentLevel;
            levelDataText = new Text("Sector Name: " + LevelData.GetLevel(selectedLevel).LevelName, FontSize.Medium);
            levelDataText.TintColor = color;
            levelDataText.Align(AlignType.Center);
        }

        public int GetSelectedLevel()
        {
            return selectedLevel;
        }

        private void AssignSelectedLevelNumber()
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].Selected && selectedLevel != i + 1)
                {
                    selectedLevel = i + 1;
                    levelDataText.TextContext = "Sector Name: " + LevelData.GetLevel(selectedLevel).LevelName;
                }
            }
        }

        private void ClearSelection()
        {
            for (int i = 0; i < levels.Count; i++)
            {
                levels[i].Selected = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (status == AnnouncerStatus.Steady)
            {
                foreach (CheckButton button in levels)
                {
                    button.Update(gameTime);
                }

                AssignSelectedLevelNumber();

                playButton.Update(gameTime);
                backButton.Update(gameTime);
                levelDataText.Update(gameTime);
            }
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            if (status == AnnouncerStatus.Steady)
            {
                text.Draw(new Vector2(camera.Width / 2, camera.Height / 2 - height / 2 + 5), camera);
                foreach (CheckButton button in levels)
                {
                    button.Draw(camera);
                }

                levelDataText.Draw(new Vector2(playButton.Position.X, playButton.Position.Y - 40 - levelDataText.Height), camera);

                playButton.Draw(camera);
                backButton.Draw(camera);
            }
        }
    }
}
