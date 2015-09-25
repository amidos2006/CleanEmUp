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
using OmidosGameEngine.Entity.Player.Data;
using OmidosGameEngine.Entity.Player.Weapons;

namespace OmidosGameEngine.World
{
    public class DriveSelectorWorld : BaseWorld
    {
        private List<VirusEnemy> viruses;
        private DriveSelectorAnnouncer announcer;
        private BaseWorld nextWorld;

        public DriveSelectorWorld(BloomComponent bloomComponent)
            : base(new Vector2(OGE.HUDCamera.Width + 100, OGE.HUDCamera.Height + 100), bloomComponent)
        {
            viruses = new List<VirusEnemy>();
        }

        public override void Intialize()
        {
            base.Intialize();

            announcer = new DriveSelectorAnnouncer(GoToNextWorld, new Color(150, 255, 130), new ButtonPressed(GoToStoryWorld), 
                new ButtonPressed(GoToSurvivalWorld), new ButtonPressed(GoToAchievementWorld), new ButtonPressed(GoToMainMenu));
            announcer.EscapeHandler = GoToMainMenu;

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

        private void GoToStoryWorld()
        {
            GlobalVariables.SurvivalMode = -1;

            GlobalVariables.CurrentDrive = announcer.GetSelectedDrive();
            DriveData.SetDriveBackdrop(GlobalVariables.CurrentDrive);

            GlobalVariables.CurrentLevel = GlobalVariables.GetLargestLevel(GlobalVariables.CurrentDrive);
            
            //Go to Level Selector
            nextWorld = new StoryWorld(bloomPostProcess);

            GoToNextWorld();
        }

        private void GoToSurvivalWorld()
        {
            nextWorld = new SurvivalTypeWorld(bloomPostProcess);

            GoToNextWorld();
        }

        private void GoToAchievementWorld()
        {
            nextWorld = new AchievementWorld(bloomPostProcess);

            GoToNextWorld();
        }

        private void GoToMainMenu()
        {
            nextWorld = new MainMenuWorld(bloomPostProcess);

            GoToNextWorld();
        }

        private void GoToNextWorld()
        {
            if (nextWorld != null)
            {
                OGE.NextWorld = nextWorld;

                Color[] colors = new Color[OGE.HUDCamera.Width * OGE.HUDCamera.Height];
                bloomPostProcess.UnBloomedTexture.GetData(colors);
                OGE.NextWorld.Transition.SetData(colors);
            }
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
