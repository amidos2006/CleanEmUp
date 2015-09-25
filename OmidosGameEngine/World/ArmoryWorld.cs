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
    public class ArmoryWorld : BaseWorld
    {
        private List<VirusEnemy> viruses;
        private ArmoryAnnouncer announcer;
        private BaseWorld nextWorld;

        public ArmoryWorld(BloomComponent bloomComponent)
            : base(new Vector2(OGE.HUDCamera.Width + 100, OGE.HUDCamera.Height + 100), bloomComponent)
        {
            viruses = new List<VirusEnemy>();
        }

        public override void Intialize()
        {
            base.Intialize();

            announcer = new ArmoryAnnouncer(GoToNextWorld, new Color(150, 255, 130), new ButtonPressed(GoToGamePlay),
                new ButtonPressed(GoToAreaSelector), new ButtonPressed(GoToVirusWorld));
            announcer.EscapeHandler = GoToAreaSelector;

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

        private void GoToGamePlay()
        {
            Dictionary<Type, EnemyData> newViruses = GlobalVariables.GetNewViruses();

            if (newViruses.Count > 0)
            {
                nextWorld = new VirusNotifierWorld(newViruses, bloomPostProcess);
            }
            else
            {
                nextWorld = new GameplayWorld(LevelData.GetNextLevel(), bloomPostProcess);
            }

            GoToNextWorld();
        }

        private void GoToAreaSelector()
        {
            nextWorld = new LevelSelectorWorld(bloomPostProcess);

            GoToNextWorld();
        }

        private void GoToVirusWorld()
        {
            Dictionary<Type, EnemyData> viruses = new Dictionary<Type, EnemyData>();
            Dictionary<Type, int> temp = GlobalVariables.AllEnemyTypes;

            foreach (KeyValuePair<Type, int> item in temp)
            {
                viruses.Add(item.Key, GlobalVariables.Data.Enemies[item.Value]);
            }

            nextWorld = new VirusDatabaseWorld(viruses, bloomPostProcess);

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
