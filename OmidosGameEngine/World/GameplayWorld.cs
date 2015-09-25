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
using OmidosGameEngine.Entity.Boss;

namespace OmidosGameEngine.World
{
    public class GameplayWorld:BaseWorld
    {
        private LevelData levelData;
        private bool levelEnded;
        private BaseWorld nextWorld;
        private TitleButtonAnnouncerEntity pauseMenu;
        private TutorialEntity tutorialEntity;
        private string failReason;

        public Alarm LevelAlarm
        {
            set;
            get;
        }

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

        public GameplayWorld(LevelData levelData, BloomComponent bloomComponent)
            :base(levelData.Dimension, bloomComponent)
        {
            HUDEntity.BossMaxHealth = 0;

            GlobalVariables.LevelScore = 0;
            GlobalVariables.EnableControls = false;
            
            this.levelData = levelData;
            this.levelEnded = false;
            this.failReason = "System Failure";
        }

        public override void Intialize()
        {
            GC.Collect();
            base.Intialize();

            AddBackground(GlobalVariables.Background);

            levelData.LoadLevel();
            BaseEntity boss = levelData.GetBoss();
            if (boss != null)
            {
                AddEntity(boss);
                SoundManager.PlayMusic("boss");
            }
            else
            {
                if (GlobalVariables.CurrentDrive < 5)
                {
                    if (!SoundManager.CurrentRunningMusic.Contains("ingame"))
                    {
                        SoundManager.PlayMusic("ingame" + GlobalVariables.CurrentDrive + (OGE.Random.Next(3) + 1).ToString());
                    }
                }
                else
                {
                    SoundManager.PlayMusic("ingame" + GlobalVariables.CurrentDrive);
                }
            }

            TotalNumberOfZipFiles = levelData.NumberOfZip;
            if (TotalNumberOfZipFiles > 0)
            {
                AddEntity(HUDEntity.FileArrowEntity);
            }

            if (levelData.NumberOfDocumentFiles == 0)
            {
                //TotalNumberOfDocumentFiles = int.MaxValue;
            }
            else
            {
                TotalNumberOfDocumentFiles = levelData.NumberOfDocumentFiles;
            }
            if (levelData.TimeEndLevel > 0)
            {
                LevelAlarm = new Alarm(levelData.TimeEndLevel, TweenType.OneShot, ClearArea);
                BaseEntity e = new BaseEntity();
                e.AddTween(LevelAlarm, true);
                AddEntity(e);
            }

            foreach (BaseGenerator generator in levelData.Generator)
            {
                AddEntity(generator);
            }

            AddEntity(new PlayerEntity(levelData.StartPosition, GlobalVariables.TopPlayer, GlobalVariables.BottomPlayer));

            HUDEntity.GameScoreType = ScoreType.Points;
            OGE.CurrentWorld.AddOverLayer(new HUDEntity());
            CursorEntity.CursorView = CursorType.Aim;

            LevelNameAnnouncerEntity levelTitle = new LevelNameAnnouncerEntity(ShowTutorial, levelData.LevelName, 
                GlobalVariables.CurrentLevel);
            levelTitle.TintColor = new Color(150, 255, 130);

            AddOverLayer(levelTitle);
        }

        private void ShowTutorial()
        {
            if (GlobalVariables.CurrentLevel == 1 && GlobalVariables.CurrentDrive == 1)
            {
                tutorialEntity = new TutorialEntity(new Color(150, 255, 130), RemoveTutorial);
                AddEntity(tutorialEntity);
            }
        }

        public void ClearArea()
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

            addedEntities.Clear();
            addedOverlayerEntities.Clear();
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
            if (!levelData.IsSelectedLevel())
            {
                buttons.Add(new Button(color, "Return to Armory Console", new ButtonPressed(ReturnToUpgradeMenu)));
            }
            else
            {
                buttons.Add(new Button(color, "Return to Sector Console", new ButtonPressed(ReturnToLevelSelector)));
            }
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

        private void GameWin()
        {
            GlobalVariables.PlayerScore += GlobalVariables.LevelScore;
            GlobalVariables.CurrentLevel += 1;
            GlobalVariables.ClearedLevels += 1;

            if (GlobalVariables.CurrentLevel <= LevelData.MAX_LEVEL_DRIVE_NUMBER)
            {
                GlobalVariables.LockedLevels[(GlobalVariables.CurrentDrive - 1) * LevelData.MAX_LEVEL_DRIVE_NUMBER +
                    GlobalVariables.CurrentLevel - 1] = false;

                LevelData tempLevelData = LevelData.GetNextLevel();

                if (!tempLevelData.IsSelectedLevel())
                {
                    nextWorld = new ArmoryWorld(bloomPostProcess);
                }
                else
                {
                    //Go to game play
                    tempLevelData.LoadSelectedLevelData();

                    Dictionary<Type, EnemyData> newViruses = GlobalVariables.GetNewViruses();

                    if (newViruses.Count > 0)
                    {
                        nextWorld = new VirusNotifierWorld(newViruses, bloomPostProcess);
                    }
                    else
                    {
                        nextWorld = new GameplayWorld(tempLevelData, bloomPostProcess);
                    }
                }
                GoToNextWorld();
            }
            else
            {
                GlobalVariables.CurrentLevel = LevelData.MAX_LEVEL_DRIVE_NUMBER;

                if (GlobalVariables.IsDemoVersion)
                {
                    nextWorld = new DemoWorld(bloomPostProcess);
                }
                else
                {
                    nextWorld = new EndStoryWorld(bloomPostProcess);
                }
                
                GoToNextWorld();
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
            nextWorld = new GameplayWorld(levelData, bloomPostProcess);
        }

        private void ReturnToMainMenu()
        {
            nextWorld = new MainMenuWorld(bloomPostProcess);
        }

        private void ReturnToUpgradeMenu()
        {
            //TODO: go to the upgrade menu
            nextWorld = new ArmoryWorld(bloomPostProcess);
        }

        private void ReturnToLevelSelector()
        {
            nextWorld = new LevelSelectorWorld(bloomPostProcess);
        }

        private bool IsAllMalware(List<BaseEntity> enemyList)
        {
            foreach (BaseEntity e in enemyList)
            {
                if (!(e is MalzoneEnemy || e is Malzone2Enemy))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckWin()
        {
            if (TotalNumberOfDocumentFiles > 0)
            {
                int remainingZipFiles = 0;
                List<BaseEntity> zipFileList = GetCollisionEntitiesType(CollisionType.File);

                foreach (BaseEntity zipFile in zipFileList)
                {
                    if (zipFile is DocumentFile)
                    {
                        remainingZipFiles += 1;
                    }
                }

                if (remainingZipFiles == 0 && NumberOfDocumentFiles >= TotalNumberOfDocumentFiles)
                {
                    ClearArea();
                }
            }

            List<BaseEntity> enemyList = GetCollisionEntitiesType(CollisionType.Enemy);
            List<BaseEntity> bossList = GetCollisionEntitiesType(CollisionType.Boss);
            List<BaseEntity> generatorList = GetCollisionEntitiesType(CollisionType.Generator);

            return (IsAllMalware(enemyList) && bossList.Count == 0 && generatorList.Count == 0);
        }

        public bool CheckLose()
        {
            List<BaseEntity> playerList = GetCollisionEntitiesType(CollisionType.Player);

            if (TotalNumberOfZipFiles > 0)
            {
                if (NumberOfDestroyedZip >= TotalNumberOfZipFiles && playerList.Count > 0)
                {
                    failReason = "Files Destroyed";
                    (playerList[0] as PlayerEntity).PlayerDestroy();
                }
            }

            return playerList.Count == 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            GlobalVariables.CheckAchievements();

            if (LevelAlarm != null)
            {
                LevelAlarm.SpeedFactor = OGE.PlayerSlowFactor;
                HUDEntity.SurvivalRemainingTime = (int)Math.Ceiling(LevelAlarm.CurrentSeconds);
            }
            else
            {
                HUDEntity.SurvivalRemainingTime = 0;
            }

            if (!levelEnded)
            {
                if (CheckLose())
                {
                    if (LevelAlarm != null)
                    {
                        LevelAlarm.Pause();
                    }

                    levelEnded = true;

                    CursorEntity.CursorView = CursorType.Normal;
                    SoundManager.ChangeMusicVolume(SoundManager.MIN_VOLUME);

                    List<Button> buttons = new List<Button>();
                    Color color = new Color(255, 180, 180);
                    buttons.Add(new Button(color, "Retry Area", new ButtonPressed(RetryLevel)));
                    if (!levelData.IsSelectedLevel())
                    {
                        buttons.Add(new Button(color, "Return to Armory Console", new ButtonPressed(ReturnToUpgradeMenu)));
                    }
                    else
                    {
                        buttons.Add(new Button(color, "Return to Sector Console", new ButtonPressed(ReturnToLevelSelector)));
                    }
                    buttons.Add(new Button(color, "Return to Main Console", new ButtonPressed(ReturnToMainMenu)));

                    TitleButtonAnnouncerEntity loseTitle = new TitleButtonAnnouncerEntity(new AnnouncerEnded(GoToNextWorld), color,
                        failReason, buttons);

                    AddOverLayer(loseTitle);
                }

                if (CheckWin())
                {
                    levelEnded = true;

                    //Convert the files
                    List<BaseEntity> files = GetCollisionEntitiesType(CollisionType.File);

                    for (int i = 0; i < files.Count; i++)
                    {
                        if (files[i] is ExeFile)
                        {
                            if (!(files[i] as ExeFile).IsInfected)
                            {
                                (files[i] as ExeFile).AddScore();
                                GlobalVariables.Achievements[typeof(ExeFile)].CurrentNumber += 1;
                            }

                            (files[i] as ExeFile).LevelEnded();
                        }

                        if (files[i] is ZipFile)
                        {
                            GlobalVariables.Achievements[typeof(ZipFile)].CurrentNumber += 1;
                        }
                    }

                    TitleTimeAnnouncerEntity winTitle = new TitleTimeAnnouncerEntity(new AnnouncerEnded(GameWin), "Sector Secured");
                    winTitle.TintColor = new Color(150, 255, 130);

                    AddOverLayer(winTitle);
                }

                if (TotalNumberOfZipFiles > 0)
                {
                    List<BaseEntity> zipFiles = GetCollisionEntitiesType(CollisionType.File);
                    List<Vector2> zipPositions = new List<Vector2>();
                    foreach (BaseFile zipFile in zipFiles)
                    {
                        if (zipFile is ZipFile)
                        {
                            zipPositions.Add(zipFile.Position);
                        }
                    }

                    HUDEntity.FileArrowEntity.UpdatePosition(zipPositions);
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
