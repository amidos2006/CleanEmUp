using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloomPostprocess;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.OverLayer;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Entity.Cursor;

namespace OmidosGameEngine.World
{
    public class SurvivalTypeWorld:BaseWorld
    {
        private List<VirusEnemy> viruses;
        private TitleButtonAnnouncerEntity announcer;
        private BaseWorld nextWorld;

        public SurvivalTypeWorld(BloomComponent bloomComponent)
            : base(new Vector2(OGE.HUDCamera.Width + 100, OGE.HUDCamera.Height + 100), bloomComponent)
        {
            viruses = new List<VirusEnemy>();
        }

        public override void Intialize()
        {
            base.Intialize();

            List<Button> buttons = new List<Button>();
            buttons.Add(new Button(new Color(150, 255, 130), "Score Survival", ScoreSurvival));
            buttons.Add(new Button(new Color(150, 255, 130), "Time Survival", DefenderSurvival));
            buttons.Add(new Button(new Color(150, 255, 130), "File Survival", CollectorSurvival));
            buttons.Add(new Button(new Color(150, 255, 130), "Survival Statistics", SurvivalStatistics));
            buttons.Add(new Button(new Color(150, 255, 130), "Return to Drive Console", ReturnToDriveConsole));
            
            announcer = new TitleButtonAnnouncerEntity(null, new Color(150, 255, 130), "Survival Console", buttons);
            announcer.EscapeHandler = ReturnToDriveConsole;

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

        private void ScoreSurvival()
        {
            GlobalVariables.SurvivalMode = 0;

            nextWorld = new SurvivalArmoryWorld(bloomPostProcess);

            GoToNextWorld();
        }

        private void DefenderSurvival()
        {
            GlobalVariables.SurvivalMode = 1;

            nextWorld = new SurvivalArmoryWorld(bloomPostProcess);

            GoToNextWorld();
        }

        private void CollectorSurvival()
        {
            GlobalVariables.SurvivalMode = 2;

            nextWorld = new SurvivalArmoryWorld(bloomPostProcess);

            GoToNextWorld();
        }

        private void SurvivalStatistics()
        {
            nextWorld = new SurvivalStatisticsWorld(bloomPostProcess);

            GoToNextWorld();
        }

        private void ReturnToDriveConsole()
        {
            nextWorld = new DriveSelectorWorld(bloomPostProcess);

            GoToNextWorld();
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
