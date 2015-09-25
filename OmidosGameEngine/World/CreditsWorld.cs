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
    public class CreditsWorld : BaseWorld
    {
        private Image gameLogo;
        private List<VirusEnemy> viruses;
        private Image omidosLogo;
        private BaseWorld nextWorld;

        public CreditsWorld(BloomComponent bloomComponent)
            : base(new Vector2(OGE.HUDCamera.Width + 100, OGE.HUDCamera.Height + 100), bloomComponent)
        {
            viruses = new List<VirusEnemy>();
        }

        public override void Intialize()
        {
            base.Intialize();

            Color color = new Color(150, 255, 130);
            List<CreditData> credits = new List<CreditData>();
            
            credits.Add(new CreditData { Title = "Game by" });
            credits[0].Names.Add("Omidos Entertainment");

            credits.Add(new CreditData { Title = "Music by" });
            credits[1].Names.Add("Mikeal BauAir");

            credits.Add(new CreditData { Title = "Additional Art by" });
            credits[2].Names.Add("Ahmed Ibrahim");

            credits.Add(new CreditData { Title = "Additional Support by" });
            credits[3].Names.Add("Mohamed Assem");

            credits.Add(new CreditData { Title = "Thanks for everyone who helped to test the game" });

            CreditsAnnouncer announcer = new CreditsAnnouncer(ReturnToMainMenu, color, "Credits Console", credits);
            announcer.ChangePosition(140);
            announcer.EscapeHandler = ReturnToMainMenu;

            AddOverLayer(announcer);

            gameLogo = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\WindowGraphics\CleanEmUpLogo"));
            gameLogo.CenterOrigin();
            gameLogo.OriginX += 50;

            omidosLogo = new Image(OGE.Content.Load<Texture2D>(@"Graphics\Entities\WindowGraphics\Omidos"));
            omidosLogo.OriginX = omidosLogo.Width;
            omidosLogo.OriginY = omidosLogo.Height;

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

        private void ReturnToMainMenu()
        {
            nextWorld = new MainMenuWorld(bloomPostProcess);

            TransferToNextLevel();
        }

        public override void UnPauseGame()
        {
            base.UnPauseGame();

            if (PauseEntity != null)
            {
                PauseEntity = null;
            }
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
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            gameLogo.Draw(new Vector2(OGE.HUDCamera.Width / 2, gameLogo.Height / 2 + 10), OGE.HUDCamera);

            omidosLogo.Draw(new Vector2(OGE.HUDCamera.Width - 10, OGE.HUDCamera.Height - 10), OGE.HUDCamera);
        }
    }
}
