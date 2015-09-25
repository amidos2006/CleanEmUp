using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Data;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Graphics;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class DriveSelectorAnnouncer : AnnouncerEntity
    {
        private const int MAX_DRIVES_DEMO = 1;

        private List<CheckButton> drives;
        private Button playButton;
        private Button achievementButton;
        private Button survivalButton;
        private Button backButton;
        private Text driveDataText;
        private Text clearedSectorsText;
        private int selectedDrive;

        public DriveSelectorAnnouncer(AnnouncerEnded endFunction,Color color, ButtonPressed playPressed, 
            ButtonPressed survivalPressed, ButtonPressed achievementPressed, ButtonPressed backPressed)
            : base(endFunction, (float)(Math.Ceiling(LevelData.MAX_LEVEL_DRIVE_NUMBER / 5.0) * 110 + 125))
        {
            text = new Text("Drive Console", FontSize.Large);
            text.Align(AlignType.Center);

            if (GlobalVariables.ClearedLevels == 1)
            {
                clearedSectorsText = new Text("[ " + GlobalVariables.ClearedLevels + " Sector Secured ]", FontSize.Small);
            }
            else
            {
                clearedSectorsText = new Text("[ " + GlobalVariables.ClearedLevels + " Sectors Secured ]", FontSize.Small);
            }
            clearedSectorsText.Align(AlignType.Center);
            clearedSectorsText.TintColor = color;

            drives = new List<CheckButton>();

            for (int i = 0; i < DriveData.MAX_DRIVE_NUMBER; i++)
            {
                drives.Add(new CheckButton(color, GlobalVariables.Drive.DrivesData[i].DriveLetter, new ButtonPressed(ClearSelection)));
                drives[drives.Count - 1].Position.X = OGE.HUDCamera.Width / 2 + (i - DriveData.MAX_DRIVE_NUMBER / 2.0f) * 110 + 50;
                drives[drives.Count - 1].Position.Y = OGE.HUDCamera.Height / 2 - 110;
                drives[drives.Count - 1].Selected = false;
                drives[drives.Count - 1].Active = !GlobalVariables.LockedLevels[i * LevelData.MAX_LEVEL_DRIVE_NUMBER];
            }

            drives[GlobalVariables.CurrentDrive - 1].Selected = true;
            TintColor = color;

            backButton = new Button(color, "Return to Main Console", backPressed);
            backButton.Position.X = OGE.HUDCamera.Width / 2;
            backButton.Position.Y = OGE.HUDCamera.Height / 2 + maxHeight / 2 - 35;

            achievementButton = new Button(color, "Achievement Console", achievementPressed);
            achievementButton.Position.X = backButton.Position.X;
            achievementButton.Position.Y = backButton.Position.Y - 60;

            survivalButton = new Button(color, "Survival Console", survivalPressed);
            survivalButton.Position.X = achievementButton.Position.X;
            survivalButton.Position.Y = achievementButton.Position.Y - 60;

            playButton = new Button(color, "Select Drive", playPressed);
            playButton.Position.X = survivalButton.Position.X;
            playButton.Position.Y = survivalButton.Position.Y - 60;

            selectedDrive = GlobalVariables.CurrentDrive;
            driveDataText = new Text("Drive Name: " + GlobalVariables.Drive.DrivesData[selectedDrive - 1].DriveName, FontSize.Medium);
            driveDataText.TintColor = color;
            driveDataText.Align(AlignType.Center);

            if (GlobalVariables.IsDemoVersion)
            {
                for (int i = MAX_DRIVES_DEMO; i < DriveData.MAX_DRIVE_NUMBER; i++)
                {
                    drives[i].Active = false;
                }

                drives[0].Selected = true;
                survivalButton.Active = false;
            }
        }

        public int GetSelectedDrive()
        {
            return selectedDrive;
        }

        private void AssignSelectedDriveNumber()
        {
            for (int i = 0; i < drives.Count; i++)
            {
                if (drives[i].Selected && selectedDrive != i + 1)
                {
                    selectedDrive = i + 1;
                    driveDataText.TextContext = "Drive Name: " + GlobalVariables.Drive.DrivesData[selectedDrive - 1].DriveName;
                }
            }
        }

        private void ClearSelection()
        {
            for (int i = 0; i < drives.Count; i++)
            {
                drives[i].Selected = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (status == AnnouncerStatus.Steady)
            {
                foreach (CheckButton button in drives)
                {
                    button.Update(gameTime);
                }

                AssignSelectedDriveNumber();

                playButton.Update(gameTime);
                survivalButton.Update(gameTime);
                achievementButton.Update(gameTime);
                backButton.Update(gameTime);
                driveDataText.Update(gameTime);
                clearedSectorsText.Update(gameTime);
            }
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            if (status == AnnouncerStatus.Steady)
            {
                text.Draw(new Vector2(camera.Width / 2, camera.Height / 2 - height / 2 + 5), camera);
                if (GlobalVariables.ClearedLevels > 0)
                {
                    clearedSectorsText.Draw(new Vector2(camera.Width / 2, camera.Height / 2 - height / 2 + text.Height + 10), camera);
                }

                foreach (CheckButton button in drives)
                {
                    button.Draw(camera);
                }

                driveDataText.Draw(new Vector2(playButton.Position.X, playButton.Position.Y - 40 - driveDataText.Height), camera);

                playButton.Draw(camera);
                survivalButton.Draw(camera);
                achievementButton.Draw(camera);
                backButton.Draw(camera);
            }
        }
    }
}
