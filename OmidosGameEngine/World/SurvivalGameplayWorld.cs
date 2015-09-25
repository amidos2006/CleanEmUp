using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Data;
using BloomPostprocess;
using OmidosGameEngine.Entity.Generator;
using OmidosGameEngine.Entity.OverLayer;
using OmidosGameEngine.Entity.Cursor;
using OmidosGameEngine.Sounds;
using OmidosGameEngine.Entity.Player;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity;
using OmidosGameEngine.Collision;
using Microsoft.Xna.Framework.Input;
using OmidosGameEngine.Entity.Player.Data;
using OmidosGameEngine.Entity.Player.Weapons;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Entity.Tutorial;
using OmidosGameEngine.Entity.Player.Bullet;
using OmidosGameEngine.Entity.Player.OverClocking;
using OmidosGameEngine.Tween;
using OmidosGameEngine.Entity.Object.File;

namespace OmidosGameEngine.World
{
    public class SurvivalGameplayWorld:BaseWorld
    {
        private LevelData levelData;
        private bool levelEnded;
        private BaseWorld nextWorld;
        private TitleButtonAnnouncerEntity pauseMenu;
        private TutorialEntity tutorialEntity;
        private double elapsedTime;

        public int NumberOfDocumentFiles
        {
            set;
            get;
        }

        public int TotalNumberOfDocumentFiles
        {
            set;
            get;
        }

        public int NumberOfDestroyedZip
        {
            set;
            get;
        }

        public int TotalNumberOfZipFiles
        {
            set;
            get;
        }

        public SurvivalGameplayWorld(LevelData levelData, BloomComponent bloomComponent)
            :base(levelData.Dimension, bloomComponent)
        {
            GlobalVariables.LevelScore = 0;
            GlobalVariables.EnableControls = false;
            
            this.levelData = levelData;
            this.levelEnded = false;
        }

        public override void Intialize()
        {
            GC.Collect();
            base.Intialize();

            AddBackground(GlobalVariables.Background);

            levelData.LoadLevel();

            TotalNumberOfZipFiles = levelData.NumberOfZip;
            if (levelData.NumberOfDocumentFiles == 0)
            {
                TotalNumberOfDocumentFiles = int.MaxValue;
            }
            else
            {
                TotalNumberOfDocumentFiles = levelData.NumberOfDocumentFiles;
            }

            foreach (BaseGenerator generator in levelData.Generator)
            {
                AddEntity(generator);
            }

            AddEntity(new PlayerEntity(levelData.StartPosition, GlobalVariables.TopPlayer, GlobalVariables.BottomPlayer));

            HUDEntity.GameScoreType = GlobalVariables.GetSurvivalHUDVariable();
            OGE.CurrentWorld.AddOverLayer(new HUDEntity());
            CursorEntity.CursorView = CursorType.Aim;

            int trackNumber = OGE.Random.Next(DriveData.MAX_DRIVE_NUMBER) + 1;
            if (trackNumber < 5)
            {
                SoundManager.PlayMusic("ingame" + trackNumber.ToString() + (OGE.Random.Next(3) + 1).ToString());
            }
            else
            {
                SoundManager.PlayMusic("ingame" + trackNumber.ToString());
            }

            LevelNameAnnouncerEntity levelTitle = new LevelNameAnnouncerEntity(null, levelData.LevelName, 
                (GlobalVariables.SurvivalMode + 1));
            levelTitle.TintColor = new Color(150, 255, 130);

            AddOverLayer(levelTitle);
        }

        private void ClearArea()
        {
            List<BaseEntity> enemies = GetCollisionEntitiesType(CollisionType.Enemy);

            foreach (BaseEntity entity in enemies)
            {
                (entity as BaseEnemy).EnemyDestroy();
            }

            List<BaseEntity> generators = GetCollisionEntitiesType(CollisionType.Generator);

            foreach (BaseEntity generator in generators)
            {
                RemoveEntity(generator);
            }

            AdditiveWhite.Alpha += 1;
        }

        private void RemoveTutorial()
        {
            RemoveEntity(tutorialEntity);
            tutorialEntity = null;
        }

        public override void PauseGame()
        {
            if (CheckLose())
            {
                return;
            }

            base.PauseGame();

            CursorEntity.CursorView = CursorType.Normal;

            List<Button> buttons = new List<Button>();
            Color color = new Color(150, 255, 130);
            buttons.Add(new Button(color, "Continue Scanning", new ButtonPressed(ReturnToGame)));
            buttons.Add(new Button(color, "Return to Armory Console", new ButtonPressed(ReturnToArmoryConsole)));
            buttons.Add(new Button(color, "Return to Main Console", new ButtonPressed(ReturnToMainMenu)));

            pauseMenu = new TitleButtonAnnouncerEntity(new AnnouncerEnded(GoToNextWorld), color, "Pause Console", buttons);
            pauseMenu.EscapeHandler = ReturnToGame;

            PauseEntity = pauseMenu;
        }

        public override void UnPauseGame()
        {

        }

        public void ReturnToGame()
        {
            CursorEntity.CursorView = CursorType.Aim;

            if (pauseMenu != null)
            {
                pauseMenu.FinishAnnouncer();
            }
        }

        private void GoToNextWorld()
        {
            if (nextWorld != null)
            {
                OGE.NextWorld = nextWorld;

                Color[] colors = new Color[OGE.HUDCamera.Width * OGE.HUDCamera.Height];
                bloomPostProcess.UnBloomedTexture.GetData(colors);
                OGE.NextWorld.Transition.SetData(colors);
                ReturnNormal();

                GlobalVariables.SaveGame();
            }
            else
            {
                base.UnPauseGame();
            }
        }

        private void RetryLevel()
        {
            nextWorld = new SurvivalGameplayWorld(levelData, bloomPostProcess);
        }

        private void ReturnToMainMenu()
        {
            nextWorld = new MainMenuWorld(bloomPostProcess);
        }

        private void ReturnToArmoryConsole()
        {
            nextWorld = new SurvivalArmoryWorld(bloomPostProcess);
        }

        public bool CheckLose()
        {
            List<BaseEntity> playerList = GetCollisionEntitiesType(CollisionType.Player);

            if (TotalNumberOfZipFiles > 0)
            {
                if (NumberOfDestroyedZip >= TotalNumberOfZipFiles && playerList.Count > 0)
                {
                    (playerList[0] as PlayerEntity).PlayerDestroy();
                }
            }

            return playerList.Count == 0;
        }

        private string GetScoreSubtitle()
        {
            string returnedString = "[";
            switch (GlobalVariables.SurvivalMode)
            {
                case 0:
                    returnedString += GlobalVariables.LevelScore + " pts";
                    break;
                case 1:
                    returnedString += (int)elapsedTime + " secs";
                    break;
                case 2:
                    returnedString += NumberOfDocumentFiles + " files";
                    break;
            }

            return returnedString +"]";
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            GlobalVariables.CheckAchievements();

            if (!levelEnded)
            {
                elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                HUDEntity.TimeScore = (int)elapsedTime;
                HUDEntity.FilesScore = NumberOfDocumentFiles;

                if (CheckLose())
                {
                    levelEnded = true;

                    List<int> scores = new List<int>();
                    scores.Add(GlobalVariables.LevelScore);
                    scores.Add((int)elapsedTime);
                    scores.Add(NumberOfDocumentFiles);

                    GlobalVariables.UpdateSurvivalVariables(scores);

                    CursorEntity.CursorView = CursorType.Normal;
                    SoundManager.ChangeMusicVolume(SoundManager.MIN_VOLUME);

                    List<Button> buttons = new List<Button>();
                    Color color = new Color(150, 255, 130);
                    buttons.Add(new Button(color, "Retry Area", new ButtonPressed(RetryLevel)));
                    buttons.Add(new Button(color, "Return to Armory Console", new ButtonPressed(ReturnToArmoryConsole)));
                    buttons.Add(new Button(color, "Return to Main Console", new ButtonPressed(ReturnToMainMenu)));

                    TitleSubtitleButtonAnnouncerEntity loseTitle = new TitleSubtitleButtonAnnouncerEntity(new AnnouncerEnded(GoToNextWorld), 
                        color, "Survival Ends", GetScoreSubtitle() , buttons);

                    AddOverLayer(loseTitle);
                }

                if (Input.CheckKeyboardButton(Keys.Escape) == GameButtonState.Pressed && !pauseGame)
                {
                    PauseGame();
                }

                if (Input.CheckKeyboardButton(Keys.F1) == GameButtonState.Pressed)
                {
                    if (tutorialEntity == null)
                    {
                        tutorialEntity = new TutorialEntity(new Color(150, 255, 130), RemoveTutorial);
                        AddEntity(tutorialEntity);
                    }
                    else
                    {
                        tutorialEntity.FinishTutorial();
                    }
                }

                if (Input.CheckLeftMouseButton() == GameButtonState.Up && Input.CheckRightMouseButton() == GameButtonState.Up)
                {
                    if (CursorEntity.IsShooting)
                    {
                        CursorEntity.IsShooting = false;
                    }
                }
            }
        }
    }
}
