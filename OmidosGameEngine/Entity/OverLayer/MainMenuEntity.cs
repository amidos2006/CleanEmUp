using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OmidosGameEngine.World;
using OmidosGameEngine.Data;
using BloomPostprocess;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class MainMenuEntity : BaseEntity
    {
        private List<Button> mainMenuButtons;
        private bool saveExists;
        private BloomComponent bloomComponent;

        public RenderTarget2D Transition
        {
            set;
            get;
        }

        public MainMenuEntity(BloomComponent bloomComponent)
        {
            mainMenuButtons = new List<Button>();
            this.bloomComponent = bloomComponent;

            Color color = new Color(150, 255, 130);
            mainMenuButtons.Add(new Button(color,"Start New Scan", new ButtonPressed(NewGame)));
            mainMenuButtons.Add(new Button(color,"Continue Scanning", new ButtonPressed(LoadContent)));
            mainMenuButtons.Add(new Button(color,"Stop Scanning", new ButtonPressed(ExitGame)));

            for (int i = 0; i < mainMenuButtons.Count; i++)
            {
                mainMenuButtons[i].Position.X = OGE.HUDCamera.Width / 2;
                mainMenuButtons[i].Position.Y = OGE.HUDCamera.Height - (mainMenuButtons.Count - i) * 70 - 50;
            }

            saveExists = GlobalVariables.SaveExists();
        }

        private void NewGame()
        {
            // Go to Story World
            OGE.NextWorld = new GameplayWorld(LevelData.GetNextLevel(), bloomComponent);
            
            Color[] colors = new Color[OGE.HUDCamera.Width * OGE.HUDCamera.Height];
            Transition.GetData(colors);
            OGE.NextWorld.Transition.SetData(colors);
        }

        private void LoadGame()
        {
            GlobalVariables.LoadGame();
            // Go to Upgrade World
        }

        private void ExitGame()
        {
            CleanEmUp.SteamManager.CloseSteam();
            OGE.CleanEmUpApplication.Exit();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            mainMenuButtons[1].Active = saveExists;
           
            foreach (Button button in mainMenuButtons)
            {
                button.Update(gameTime);
            }
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            foreach (Button button in mainMenuButtons)
            {
                button.Draw(camera);
            }
        }
    }
}
