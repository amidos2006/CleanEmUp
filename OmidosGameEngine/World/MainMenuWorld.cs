using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BloomPostprocess;
using OmidosGameEngine.Entity.Cursor;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Entity.OverLayer;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Data;

namespace OmidosGameEngine.World
{
    public class MainMenuWorld : BaseWorld
    {
        private static bool firstRun = true;

        private LogoEntity logoEntity;
        private Image gameLogo;
        private List<VirusEnemy> viruses;
        private Image omidosLogo;
        private BaseWorld nextWorld;
        private float alphaIncrement = 0.02f;
        private float currentAlpha = 0;

        public MainMenuWorld(BloomComponent bloomComponent)
            : base(new Vector2(OGE.HUDCamera.Width + 100, OGE.HUDCamera.Height + 100), bloomComponent)
        {
            viruses = new List<VirusEnemy>();

            if (!firstRun)
            {
                currentAlpha = 1;
            }
        }

        public override void Intialize()
        {
            base.Intialize();

            if (firstRun)
            {
                logoEntity = new LogoEntity(ShowMainMenu);
            }
            else
            {
                ShowMainMenu();
            }

            gameLogo = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\WindowGraphics\CleanEmUpLogo"));
            gameLogo.CenterOrigin();
            gameLogo.OriginX += 50;
            gameLogo.TintColor = Color.White * currentAlpha;

            omidosLogo = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\WindowGraphics\Omidos"));
            omidosLogo.OriginX = omidosLogo.Width;
            omidosLogo.OriginY = omidosLogo.Height;
            omidosLogo.TintColor = Color.White * currentAlpha;

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

        private void ShowMainMenu()
        {
            firstRun = false;

            List<ButtonPressed> pressedButtons = new List<ButtonPressed>();
            Color color = new Color(150, 255, 130);
            pressedButtons.Add(new ButtonPressed(NewGame));
            pressedButtons.Add(new ButtonPressed(LoadGame));
            pressedButtons.Add(new ButtonPressed(GoToSettings));
            pressedButtons.Add(new ButtonPressed(GoToCredits));
            pressedButtons.Add(new ButtonPressed(ExitGame));

            MainMenuButtonAnnouncerEntity announcer = new MainMenuButtonAnnouncerEntity(null, color, "Main Console", pressedButtons);
            announcer.ChangePosition(140);
            announcer.EscapeHandler = ExitGame;

            AddOverLayer(announcer);
        }

        public override void UnPauseGame()
        {
            base.UnPauseGame();

            if (PauseEntity != null)
            {
                PauseEntity = null;
            }
        }

        private void NewGame()
        {
            if (!GlobalVariables.SaveExists())
            {
                //Go to the story
                GoToDriveSelector();
            }
            else
            {
                List<Button> buttons = new List<Button>();
                Color color = new Color(255, 180, 180);
                buttons.Add(new Button(color, "Yes", new ButtonPressed(DeleteSaveAndStart)));
                buttons.Add(new Button(color, "No", null));

                TitleButtonAnnouncerEntity pauseMenu = new TitleButtonAnnouncerEntity(new AnnouncerEnded(UnPauseGame), color,
                    "Delete Scan History and Start New Scan?", buttons);
                pauseMenu.ChangePosition(140);
                pauseMenu.EscapeHandler = pauseMenu.FinishAnnouncer;

                PauseEntity = pauseMenu;

                base.PauseGame();
            }
        }

        private void GoToCredits()
        {
            nextWorld = new CreditsWorld(bloomPostProcess);
            TransferToNextLevel();
        }

        private void GoToSettings()
        {
            nextWorld = new SettingsWorld(bloomPostProcess);
            TransferToNextLevel();
        }

        private void GoToDriveSelector()
        {
            nextWorld = new DriveSelectorWorld(bloomPostProcess);
            TransferToNextLevel();
        }

        private void DeleteSaveAndStart()
        {
            GlobalVariables.DeleteSave();
            GoToDriveSelector();
        }

        private void LoadGame()
        {
            GlobalVariables.LoadGame();

            nextWorld = new DriveSelectorWorld(bloomPostProcess);

            TransferToNextLevel();
        }

        private void ExitGame()
        {
            List<Button> buttons = new List<Button>();
            Color color = new Color(255, 180, 180);
            buttons.Add(new Button(color, "Yes", new ButtonPressed(TerminateApplication)));
            buttons.Add(new Button(color, "No", null));

            TitleButtonAnnouncerEntity pauseMenu = new TitleButtonAnnouncerEntity(new AnnouncerEnded(UnPauseGame), color, 
                "Stop Scanning?", buttons);
            pauseMenu.ChangePosition(140);
            pauseMenu.EscapeHandler = pauseMenu.FinishAnnouncer;

            PauseEntity = pauseMenu;

            base.PauseGame();
        }

        private void TerminateApplication()
        {
            CleanEmUp.SteamManager.CloseSteam();
            OGE.CleanEmUpApplication.Exit();
        }

        private void TransferToNextLevel()
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

            if (firstRun)
            {
                logoEntity.Update(gameTime);
            }
            else
            {
                currentAlpha += alphaIncrement;
                if (currentAlpha > 1)
                {
                    currentAlpha = 1;
                }

                gameLogo.TintColor = Color.White * currentAlpha;
                omidosLogo.TintColor = Color.White * currentAlpha;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            gameLogo.Draw(new Vector2(OGE.HUDCamera.Width / 2, gameLogo.Height / 2 + 10), OGE.HUDCamera);

            omidosLogo.Draw(new Vector2(OGE.HUDCamera.Width - 10, OGE.HUDCamera.Height - 10), OGE.HUDCamera);

            if (firstRun)
            {
                logoEntity.Draw(OGE.HUDCamera);
            }
        }
    }
}
