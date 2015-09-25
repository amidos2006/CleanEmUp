using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Entity.OverLayer
{
    public class HUDEntity:BaseEntity
    {
        public static float PlayerHealth;
        public static float TotalPlayerHealth;
        public static float TopProcessor;
        public static float DownProcessor;
        public static float RageMeter;
        public static bool RageActive;
        public static bool TopOverHeat;
        public static bool DownOverHeat;
        public static string TopGunName = string.Empty;
        public static string DownGunName = string.Empty;
        public static string ShootingMode = string.Empty;
        public static int TimeScore = 0;
        public static int FilesScore = 0;
        public static int SurvivalRemainingTime = 0;
        public static int BossCurrentHealth = 0;
        public static int BossMaxHealth = 0;
        public static ScoreType GameScoreType = ScoreType.Points;
        public static ArrowEntity FileArrowEntity = new ArrowEntity();

        private Image lifeBGImage;
        private Image lifeImage;
        private Rectangle lifeRect;
        private Image topProcessorBGImage;
        private Image topProcessorImage;
        private Image downProcessorBGImage;
        private Image downProcessorImage;
        private Text bossText;
        private Image bossHealthImage;
        private Image bossHealthBGImage;
        private Rectangle processorRect;
        private Text topGunText;
        private Text downGunText;
        private Text overclockingText;
        private Image overclockingBGImage;
        private Image overclockingImage;
        private Text scoreText;
        private Color normalColor;
        private Color warningColor;
        private Color overheatColor;
        private float warningPercentage;
        private Text survivalTimeText;
        private int oldSurvivalTime = -1;
        private float bossHealthPercentage = 0;

        public HUDEntity()
        {
            Texture2D texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\HUD\LifeBG");
            lifeBGImage = new Image(texture, new Rectangle(0, 0, texture.Width, texture.Height));
            texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\HUD\Life");
            lifeRect = new Rectangle(0, 0, texture.Width, texture.Height);
            lifeImage = new Image(texture, lifeRect);

            texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\HUD\ProcessorBG");
            topProcessorBGImage = new Image(texture, new Rectangle(0, 0, texture.Width, texture.Height));
            downProcessorBGImage = new Image(texture, new Rectangle(0, 0, texture.Width, texture.Height));
            texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\HUD\Processor");
            processorRect = new Rectangle(0, 0, texture.Width, texture.Height);
            topProcessorImage = new Image(texture, processorRect);
            downProcessorImage = new Image(texture, processorRect);

            texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\HUD\RageBG");
            overclockingBGImage = new Image(texture, new Rectangle(0, 0, texture.Width, texture.Height));
            texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\HUD\Rage");
            overclockingImage = new Image(texture, new Rectangle(0, 0, texture.Width, texture.Height));

            texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\HUD\BossHealthBG");
            bossHealthBGImage = new Image(texture, new Rectangle(0, 0, texture.Width, texture.Height));
            texture = OGE.Content.Load<Texture2D>(@"Graphics\Entities\HUD\BossHealth");
            bossHealthImage = new Image(texture, new Rectangle(0, 0, texture.Width, texture.Height));

            topGunText = new Text(TopGunName, FontSize.Small);
            downGunText = new Text(DownGunName, FontSize.Small);

            scoreText = new Text(GlobalVariables.LevelScore.ToString(), FontSize.Large);
            scoreText.Align(AlignType.Right);

            overclockingText = new Text("Overclocking", FontSize.Small);
            overclockingText.Align(AlignType.Center);

            bossText = new Text("Boss", FontSize.Small);
            bossText.Align(AlignType.Center);

            survivalTimeText = new Text("00:00", FontSize.Medium);
            survivalTimeText.Align(AlignType.Center);

            normalColor = new Color(150, 255, 130);
            warningColor = new Color(255, 255, 130);
            overheatColor = new Color(255, 180, 180);

            warningPercentage = 0.75f;

            topGunText.TintColor = normalColor;
            downGunText.TintColor = normalColor;
            overclockingText.TintColor = normalColor;
            scoreText.TintColor = normalColor;
            survivalTimeText.TintColor = normalColor;
            bossHealthImage.TintColor = normalColor;
            bossHealthBGImage.TintColor = normalColor;
            bossText.TintColor = normalColor;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            bossHealthPercentage = BossCurrentHealth * 1.0f / BossMaxHealth;
            BossCurrentHealth = 0;

            topGunText.TextContext = TopGunName + " Processor";
            downGunText.TextContext = DownGunName + " Processor";

            switch (GameScoreType)
            {
                case ScoreType.Points:
                    scoreText.TextContext = GlobalVariables.LevelScore.ToString() + " pts";
                    break;
                case ScoreType.Time:
                    scoreText.TextContext = TimeScore.ToString() + " secs";
                    break;
                case ScoreType.Files:
                    scoreText.TextContext = FilesScore.ToString() + " files";
                    break;
            }

            scoreText.OriginX = scoreText.Width;

            if(SurvivalRemainingTime > 0)
            {
                if (oldSurvivalTime > SurvivalRemainingTime && SurvivalRemainingTime < 10)
                {
                    OGE.CurrentWorld.AddOverLayer(new TextNotifierEntity(SurvivalRemainingTime.ToString()));
                }
                survivalTimeText.TextContext = (SurvivalRemainingTime / 60).ToString("00") + ":"
                    + (SurvivalRemainingTime % 60).ToString("00");
                survivalTimeText.Align(AlignType.Center);
            }

            oldSurvivalTime = SurvivalRemainingTime;

            float healthPercentage = 1;
            if (TotalPlayerHealth > 0)
            {
                healthPercentage = (TotalPlayerHealth - PlayerHealth) / TotalPlayerHealth;
            }

            lifeImage.SourceRectangle = new Rectangle(0, (int)(healthPercentage * lifeRect.Height), lifeRect.Width, 
                lifeRect.Height - (int)(healthPercentage * lifeRect.Height));
        }

        public override void Draw(Camera camera)
        {
            base.Draw(camera);

            float healthPercentage = 1;
            if (TotalPlayerHealth > 0)
            {
                healthPercentage = (TotalPlayerHealth - PlayerHealth) / TotalPlayerHealth;
            }

            lifeBGImage.Draw(new Vector2(20, 20), camera);
            lifeImage.Draw(new Vector2(20, 20 + healthPercentage * lifeRect.Height), camera);

            if (SurvivalRemainingTime > 0)
            {
                survivalTimeText.Draw(new Vector2(20 + lifeBGImage.Width / 2, 20 + lifeBGImage.Height + 10), camera);
            }

            scoreText.Draw(new Vector2(camera.Width - 20, 20), camera);

            topGunText.Draw(new Vector2(20 + lifeRect.Width + 10, 30), camera);
            float processorPercentage = TopProcessor / 100;
            if (TopOverHeat)
            {
                topProcessorBGImage.TintColor = overheatColor;
                topProcessorImage.TintColor = overheatColor;
            }
            else if (processorPercentage < warningPercentage)
            {
                topProcessorBGImage.TintColor = normalColor;
                topProcessorImage.TintColor = normalColor;
            }
            else
            {
                topProcessorBGImage.TintColor = warningColor;
                topProcessorImage.TintColor = warningColor;
            }
            topProcessorBGImage.Draw(new Vector2(20 + lifeRect.Width + 10, 30 + topGunText.Height), camera);
            topProcessorImage.SourceRectangle = new Rectangle(0, 0, (int)(processorRect.Width * processorPercentage), processorRect.Height);
            topProcessorImage.Draw(new Vector2(20 + lifeRect.Width + 10, 30 + topGunText.Height), camera);

            downGunText.Draw(new Vector2(20 + lifeRect.Width + 10, 35 + topGunText.Height + processorRect.Height), camera);
            processorPercentage = DownProcessor / 100;
            if (DownOverHeat)
            {
                downProcessorBGImage.TintColor = overheatColor;
                downProcessorImage.TintColor = overheatColor;
            }
            else if (processorPercentage < warningPercentage)
            {
                downProcessorBGImage.TintColor = normalColor;
                downProcessorImage.TintColor = normalColor;
            }
            else
            {
                downProcessorBGImage.TintColor = warningColor;
                downProcessorImage.TintColor = warningColor;
            }
            downProcessorBGImage.Draw(new Vector2(20 + lifeRect.Width + 10, 35 + topGunText.Height + processorRect.Height + downGunText.Height), camera);
            downProcessorImage.SourceRectangle = new Rectangle(0, 0, (int)(processorRect.Width * processorPercentage), processorRect.Height);
            downProcessorImage.Draw(new Vector2(20 + lifeRect.Width + 10, 35 + topGunText.Height + processorRect.Height + downGunText.Height), camera);

            overclockingText.Draw(new Vector2(camera.Width / 2, camera.Height - overclockingText.Height - overclockingBGImage.Height - 10), camera);

            float ragePercentage = RageMeter / 100;
            if (RageActive)
            {
                overclockingImage.TintColor = overheatColor;
                overclockingBGImage.TintColor = overheatColor;
            }
            else
            {
                overclockingImage.TintColor = normalColor;
                overclockingBGImage.TintColor = normalColor;
            }

            overclockingBGImage.Draw(new Vector2(camera.Width / 2 - overclockingBGImage.Width / 2, camera.Height - overclockingBGImage.Height - 5), camera);
            overclockingImage.SourceRectangle = new Rectangle(0, 0, (int)(overclockingBGImage.Width * ragePercentage), overclockingBGImage.Height);
            overclockingImage.Draw(new Vector2(camera.Width / 2 - overclockingBGImage.Width / 2, camera.Height - overclockingBGImage.Height - 5), camera);

            if (bossHealthPercentage > 0)
            {
                Vector2 bossHealthPosition = new Vector2(camera.Width - bossHealthBGImage.Width - 10, 
                    camera.Height/2 - bossHealthBGImage.Height/2);
                bossHealthBGImage.Draw(bossHealthPosition, camera);
                bossHealthImage.SourceRectangle = new Rectangle(0, (int)((1 - bossHealthPercentage) * bossHealthBGImage.Height), 
                    bossHealthBGImage.Width, (int)(bossHealthPercentage * bossHealthBGImage.Height));
                bossHealthImage.Draw(bossHealthPosition + new Vector2(0, (1 - bossHealthPercentage) * bossHealthBGImage.Height), camera);

                bossText.Draw(bossHealthPosition + new Vector2(bossHealthBGImage.Width / 2, bossHealthBGImage.Height + 10), camera);
            }
        }
    }
}
