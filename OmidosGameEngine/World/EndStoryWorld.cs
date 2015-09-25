using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloomPostprocess;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Entity.OverLayer;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Cursor;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Data;

namespace OmidosGameEngine.World
{
    public class EndStoryWorld : BaseWorld
    {
        private List<VirusEnemy> viruses;

        public EndStoryWorld(BloomComponent bloomComponent)
            : base(new Vector2(OGE.HUDCamera.Width + 100, OGE.HUDCamera.Height + 100), bloomComponent)
        {
            viruses = new List<VirusEnemy>();
        }

        public override void Intialize()
        {
            base.Intialize();

            TextAnnouncerEntity announcer = new TextAnnouncerEntity(new AnnouncerEnded(GoToDriveSelector), new Color(150, 255, 130), 
                "Story Console", GlobalVariables.Drive.DrivesData[GlobalVariables.CurrentDrive - 1].EndStory, 600, 0.5f);
            announcer.EscapeHandler = GoToDriveSelector;

            AddOverLayer(announcer);

            AddBackground(GlobalVariables.Background);
            CursorEntity.CursorView = CursorType.Normal;

            for (int i = 0; i < 15; i++)
            {
                VirusEnemy e = new VirusEnemy();
                e.Position.X = OGE.Random.Next((int)Dimensions.X);
                e.Position.Y = OGE.Random.Next((int)Dimensions.Y);

                viruses.Add(e);
                AddEntity(e);
            }

            SoundManager.PlayMusic("menu");
        }

        private void GoToDriveSelector()
        {
            if (GlobalVariables.CurrentDrive < DriveData.MAX_DRIVE_NUMBER)
            {
                GlobalVariables.LockedLevels[GlobalVariables.CurrentDrive * LevelData.MAX_LEVEL_DRIVE_NUMBER] = false;
                GlobalVariables.CurrentDrive += 1;
                GlobalVariables.CurrentLevel = 1;
            }

            OGE.NextWorld = new DriveSelectorWorld(bloomPostProcess);
            GlobalVariables.SaveGame();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector2 mousePosition = Input.GetMousePosition(OGE.HUDCamera);
            Vector2 center = new Vector2(OGE.HUDCamera.Width / 2, OGE.HUDCamera.Height / 2);
            Vector2 distance = mousePosition - center;
            distance.X = (distance.X / (OGE.HUDCamera.Width / 2)) * 100;
            distance.Y = (distance.Y / (OGE.HUDCamera.Height / 2)) * 100;

            OGE.WorldCamera.X = (int)(Dimensions.X / 2 - OGE.WorldCamera.Width / 2 + distance.X);
            OGE.WorldCamera.Y = (int)(Dimensions.Y / 2 - OGE.WorldCamera.Height / 2 + distance.Y);

            foreach (VirusEnemy virus in viruses)
            {
                if (OGE.Random.NextDouble() < 0.001)
                {
                    virus.DestinationDirection = OGE.Random.Next(360);
                }
            }
        }
    }
}
