using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BloomPostprocess;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Entity.Cursor;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Entity.OverLayer;
using OmidosGameEngine.Entity.Player.Data;
using OmidosGameEngine.Entity.Player.Weapons;
using OmidosGameEngine.Data;

namespace OmidosGameEngine.World
{
    public class StoryWorld : BaseWorld
    {
        private List<VirusEnemy> viruses;

        public StoryWorld(BloomComponent bloomComponent)
            : base(new Vector2(OGE.HUDCamera.Width + 100, OGE.HUDCamera.Height + 100), bloomComponent)
        {
            viruses = new List<VirusEnemy>();
        }

        public override void Intialize()
        {
            base.Intialize();

            TextAnnouncerEntity announcer = new TextAnnouncerEntity(new AnnouncerEnded(GoToGamePlay), new Color(150, 255, 130), 
                "Story Console", GlobalVariables.Drive.DrivesData[GlobalVariables.CurrentDrive - 1].BeginningStory, 600, 0.5f);
            announcer.EscapeHandler = GoToDriveConsole;

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

        private void GoToDriveConsole()
        {
            OGE.NextWorld = new DriveSelectorWorld(bloomPostProcess);

            Color[] colors = new Color[OGE.HUDCamera.Width * OGE.HUDCamera.Height];
            bloomPostProcess.UnBloomedTexture.GetData(colors);
            OGE.NextWorld.Transition.SetData(colors);
        }

        private void GoToGamePlay()
        {
            if (GlobalVariables.IsFirstTime)
            {
                GlobalVariables.TopPlayer = new SmithData();
                GlobalVariables.BottomPlayer = new SmithData();

                GlobalVariables.TopPlayer.Weapon = new UziWeapon();
                GlobalVariables.BottomPlayer.Weapon = new UziWeapon();

                GlobalVariables.TopPlayer.LoadContent();
                GlobalVariables.BottomPlayer.LoadContent();

                GlobalVariables.TopPlayer.Weapon.LoadContent();
                GlobalVariables.BottomPlayer.Weapon.LoadContent();

                OGE.NextWorld = new VirusNotifierWorld(GlobalVariables.GetNewViruses(), bloomPostProcess);
            }
            else
            {
                OGE.NextWorld = new LevelSelectorWorld(bloomPostProcess);
            }

            Color[] colors = new Color[OGE.HUDCamera.Width * OGE.HUDCamera.Height];
            bloomPostProcess.UnBloomedTexture.GetData(colors);
            OGE.NextWorld.Transition.SetData(colors);
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
