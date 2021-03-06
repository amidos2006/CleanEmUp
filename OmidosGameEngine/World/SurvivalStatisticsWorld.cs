﻿using System;
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
    public class SurvivalStatisticsWorld:BaseWorld
    {
        private List<VirusEnemy> viruses;
        private StatisticAnnouncer announcer;
        private BaseWorld nextWorld;

        public SurvivalStatisticsWorld(BloomComponent bloomComponent)
            : base(new Vector2(OGE.HUDCamera.Width + 100, OGE.HUDCamera.Height + 100), bloomComponent)
        {
            viruses = new List<VirusEnemy>();
        }

        public override void Intialize()
        {
            base.Intialize();

            List<string> names = new List<string>();
            List<int> scores = new List<int>();
            List<string> units = new List<string>();

            names.Add("Best Score Survival");
            names.Add("Best Time Survival");
            names.Add("Best File Survival");

            units.Add("pts");
            units.Add("secs");
            units.Add("files");

            for (int i = 0; i < GlobalVariables.SurvivalScores.Count; i++)
            {
                scores.Add(GlobalVariables.SurvivalScores[i]);
            }

            announcer = new StatisticAnnouncer(ReturnToSurvivalTypeWorld, new Color(150, 255, 130), "Survival Statistic Console", 
                names, scores, units);
            announcer.EscapeHandler = ReturnToSurvivalTypeWorld;

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

        private void ReturnToSurvivalTypeWorld()
        {
            nextWorld = new SurvivalTypeWorld(bloomPostProcess);

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
