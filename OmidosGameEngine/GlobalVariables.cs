using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Entity.Player;
using OmidosGameEngine.Graphics;
using OmidosGameEngine.Entity.Player.Data;
using Microsoft.Xna.Framework.Storage;
using OmidosGameEngine.Data;
using System.IO;
using System.Xml.Serialization;
using OmidosGameEngine.Entity.Player.Weapons;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Entity.Object.File;
using OmidosGameEngine.Graphics.Particles;
using Microsoft.Xna.Framework.Graphics;
using OmidosGameEngine.Graphics.Lighting;
using OmidosGameEngine.Entity.OverLayer;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine
{
    public static class GlobalVariables
    {
        public const int SURVIVAL_TYPES = 3;

        private static string filename = "Savegame.sav";
        private static DataLoader dataLoader;
        private static List<PlayerData> allPlayerData;
        private static List<BaseWeapon> primaryWeaponData;
        private static List<BaseWeapon> secondaryWeaponData;
        private static Dictionary<Type, int> enemyNumberData;
        private static Dictionary<Type, AchievementData> achievements;
        private static DriveData driveData;
        private static List<bool> lockedLevels;
        private static List<int> survivalScores;

        public static bool IsReviewVersion
        {
            set;
            get;
        }

        public static bool IsDemoVersion
        {
            set;
            get;
        }

        public static int SurvivalMode
        {
            set;
            get;
        }

        public static bool IsFirstTime
        {
            set;
            get;
        }

        public static PlayerData TopPlayer
        {
            set;
            get;
        }

        public static PlayerData BottomPlayer
        {
            set;
            get;
        }

        public static int TopPlayerIndex
        {
            set;
            get;
        }

        public static bool EnableControls
        {
            set;
            get;
        }

        public static int BottomPlayerIndex
        {
            set;
            get;
        }

        public static int TopWeaponIndex
        {
            set;
            get;
        }

        public static int BottomWeaponIndex
        {
            set;
            get;
        }

        public static Backdrop Background
        {
            set;
            get;
        }

        public static int CurrentDrive
        {
            set;
            get;
        }

        public static int CurrentLevel
        {
            set;
            get;
        }

        public static int PlayerScore
        {
            set;
            get;
        }

        public static int LevelScore
        {
            set;
            get;
        }

        public static int ClearedLevels
        {
            set;
            get;
        }

        public static DataLoader Data
        {
            get
            {
                return dataLoader;
            }
        }
        
        public static List<bool> LockedLevels
        {
            get
            {
                return lockedLevels;
            }
        }

        public static List<int> SurvivalScores
        {
            get
            {
                return survivalScores;
            }
        }

        public static DriveData Drive
        {
            get
            {
                return driveData;
            }
        }

        public static Dictionary<Type, int> AllEnemyTypes
        {
            get
            {
                return enemyNumberData;
            }
        }

        public static Dictionary<Type, AchievementData> Achievements
        {
            get
            {
                return achievements;
            }
        }

        public static ParticleEffectSystem ExplosionParticleEffectSystem
        {
            set;
            get;
        }

        public static ParticleEffectSystem TrailParticleEffectSystem
        {
            set;
            get;
        }

        public static LightingSystem LightingSystemLayer
        {
            set;
            get;
        }

        public static AdditiveWhiteImage AdditiveWhiteLightLayer
        {
            set;
            get;
        }

        public static ControlType Controls
        {
            set;
            get;
        }

        public static bool IsFullScreen
        {
            set;
            get;
        }

        public static Action SettingsChanged
        {
            set;
            get;
        }

        public static Action RevertSettings
        {
            set;
            get;
        }

        public static void Intialize()
        {
            ExplosionParticleEffectSystem = new ParticleEffectSystem(BlendState.Additive);
            TrailParticleEffectSystem = new ParticleEffectSystem(BlendState.Additive);
            LightingSystemLayer = new LightingSystem();
            AdditiveWhiteLightLayer = new AdditiveWhiteImage(-0.015f);

            ExplosionParticleEffectSystem.LoadContent();
            TrailParticleEffectSystem.LoadContent();
            LightingSystemLayer.LoadContent();

            CurrentDrive = 1;
            CurrentLevel = 1;
            PlayerScore = 0;
            ClearedLevels = 0;
            SurvivalMode = -1;

            XmlSerializer xml = new XmlSerializer(typeof(DataLoader));
            StreamReader reader = new StreamReader(@"Content\GameData\GameData.xml");
            dataLoader = (DataLoader)xml.Deserialize(reader);
            reader.Close();

            xml = new XmlSerializer(typeof(DriveData));
            reader = new StreamReader(@"Content\GameData\DriveData.xml");
            driveData = (DriveData)xml.Deserialize(reader);
            reader.Close();

            lockedLevels = new List<bool>();

            survivalScores = new List<int>();
            for (int i = 0; i < SURVIVAL_TYPES; i++)
            {
                survivalScores.Add(0);
            }

            IsFirstTime = true;
            
            TopPlayerIndex = 0;
            BottomPlayerIndex = 0;
            TopWeaponIndex = 0;
            BottomWeaponIndex = 0;

            achievements = new Dictionary<Type, AchievementData>();
            xml = new XmlSerializer(typeof(GameAchievements));
            reader = new StreamReader(@"Content\GameData\AchievementData.xml");
            GameAchievements gameAch = (GameAchievements)xml.Deserialize(reader);
            reader.Close();

            foreach (AchievementData achievement in gameAch.Achievements)
            {
                achievements.Add(achievement.GetObjectType(), achievement.Clone());
            }

            for (int i = 0; i < DriveData.MAX_DRIVE_NUMBER; i++)
            {
                for (int j = 0; j < LevelData.MAX_LEVEL_DRIVE_NUMBER; j++)
                {
                    lockedLevels.Add(true);
                }
            }
            lockedLevels[0] = false;

            for (int i = 0; i < dataLoader.Enemies.Length; i++)
            {
                dataLoader.Enemies[i].Locked = true;
            }
            
            allPlayerData = new List<PlayerData>();
            primaryWeaponData = new List<BaseWeapon>();
            secondaryWeaponData = new List<BaseWeapon>();
            enemyNumberData = new Dictionary<Type, int>();

            allPlayerData.Add(new SmithData());
            allPlayerData.Add(new JessicaData());
            allPlayerData.Add(new JackData());
            allPlayerData.Add(new OmarData());
            allPlayerData.Add(new EbsData());
            allPlayerData.Add(new DieselData());

            primaryWeaponData.Add(new UziWeapon());
            primaryWeaponData.Add(new RifleWeapon());
            primaryWeaponData.Add(new EvaporatorWeapon());
            primaryWeaponData.Add(new FreezerWeapon());
            primaryWeaponData.Add(new TommygunWeapon());
            primaryWeaponData.Add(new GrenadeLauncherWeapon());
            primaryWeaponData.Add(new ShotgunWeapon());
            primaryWeaponData.Add(new MinigunWeapon());
            primaryWeaponData.Add(new RocketLauncherWeapon());
            primaryWeaponData.Add(new MineLauncherWeapon());
            primaryWeaponData.Add(new BulletGrenadeLauncherWeapon());
            primaryWeaponData.Add(new SpikeLauncherWeapon());
            primaryWeaponData.Add(new RifleXpWeapon());
            primaryWeaponData.Add(new HomingRocketLauncherWeapon());
            primaryWeaponData.Add(new HellgunWeapon());
            primaryWeaponData.Add(new XenaWeapon());
            primaryWeaponData.Add(new HellRocketLauncherWeapon());
            primaryWeaponData.Add(new MotherBombLauncherWeapon());

            foreach (BaseWeapon weapon in primaryWeaponData)
            {
                secondaryWeaponData.Add((BaseWeapon)Activator.CreateInstance(weapon.GetType()));
            }
            
            enemyNumberData.Add(typeof(VirusEnemy), 0);
            enemyNumberData.Add(typeof(ExeFile), 1);
            enemyNumberData.Add(typeof(TroyEnemy), 2);
            enemyNumberData.Add(typeof(MalzoneEnemy), 3);
            enemyNumberData.Add(typeof(HackintoshEnemy), 4);
            enemyNumberData.Add(typeof(PopurEnemy), 5);
            enemyNumberData.Add(typeof(ZipFile), 6);
            enemyNumberData.Add(typeof(DOSEnemy), 7);
            enemyNumberData.Add(typeof(WormEnemy), 8);
            enemyNumberData.Add(typeof(DocumentFile), 9);
            enemyNumberData.Add(typeof(Popur2Enemy), 10);
            enemyNumberData.Add(typeof(Troy2Enemy), 11);
            enemyNumberData.Add(typeof(SlowEnemy), 12);
            enemyNumberData.Add(typeof(Hackintosh2Enemy), 13);
            enemyNumberData.Add(typeof(DOS2Enemy), 14);
            enemyNumberData.Add(typeof(Malzone2Enemy), 15);
            enemyNumberData.Add(typeof(BouncerEnemy), 16);

            for (int i = 0; i < allPlayerData.Count; i++)
            {
                allPlayerData[i].LoadContent();
            }

            for (int i = 0; i < primaryWeaponData.Count; i++)
            {
                primaryWeaponData[i].LoadContent();
                secondaryWeaponData[i].LoadContent();
            }
        }

        private static SaveData ExtractData()
        {
            SaveData data;

            int count;
            for (count = 0; count < lockedLevels.Count; count++)
            {
                if (lockedLevels[count])
                {
                    break;
                }
            }

            data.LevelNumber = count - 1;
            data.PlayerScore = PlayerScore;
            data.ClearedLevels = ClearedLevels;
            data.SurvivalScores = new int[SURVIVAL_TYPES];
            data.AchievementNumbers = new int[Achievements.Count];
            data.LockedCharacters = new bool[dataLoader.Characters.Length];
            data.LockedWeapons = new bool[dataLoader.Weapons.Length];
            data.LockedEnemies = new bool[dataLoader.Enemies.Length];

            for (int i = 0; i < dataLoader.Characters.Length; i++)
            {
                data.LockedCharacters[i] = dataLoader.Characters[i].Locked;
            }

            for (int i = 0; i < dataLoader.Weapons.Length; i++)
            {
                data.LockedWeapons[i] = dataLoader.Weapons[i].Locked;
            }

            for (int i = 0; i < dataLoader.Enemies.Length; i++)
            {
                data.LockedEnemies[i] = dataLoader.Enemies[i].Locked;
            }

            for (int i = 0; i < SURVIVAL_TYPES; i++)
            {
                data.SurvivalScores[i] = SurvivalScores[i];
            }

            List<Type> keys = new List<Type>();
            foreach (Type key in achievements.Keys)
            {
                keys.Add(key);
            }

            for (int i = 0; i < Achievements.Count; i++)
            {
                data.AchievementNumbers[i] = Achievements[keys[i]].CurrentNumber;
            }

            data.TopCharacterIndex = TopPlayerIndex;
            data.TopWeaponIndex = TopWeaponIndex;
            data.BottomCharacterIndex = BottomPlayerIndex;
            data.BottomWeaponIndex = BottomWeaponIndex;

            return data;
        }

        private static void UpdateData(SaveData? data)
        {
            if(data == null)
            {
                return;
            }

            CurrentLevel = data.Value.LevelNumber % LevelData.MAX_LEVEL_DRIVE_NUMBER + 1;
            CurrentDrive = (int)Math.Floor(data.Value.LevelNumber * 1.0f / LevelData.MAX_LEVEL_DRIVE_NUMBER) + 1;

            PlayerScore = data.Value.PlayerScore;
            ClearedLevels = data.Value.ClearedLevels;

            for (int i = 0; i < dataLoader.Characters.Length; i++)
            {
                dataLoader.Characters[i].Locked = data.Value.LockedCharacters[i];
            }

            for (int i = 0; i < dataLoader.Weapons.Length; i++)
            {
                dataLoader.Weapons[i].Locked = data.Value.LockedWeapons[i];
            }

            for (int i = 0; i < dataLoader.Enemies.Length; i++)
            {
                dataLoader.Enemies[i].Locked = data.Value.LockedEnemies[i];
            }

            for (int i = 0; i < SURVIVAL_TYPES; i++)
            {
                SurvivalScores[i] = data.Value.SurvivalScores[i];
            }

            List<Type> keys = new List<Type>();
            foreach (Type key in achievements.Keys)
            {
                keys.Add(key);
            }

            for (int i = 0; i < Achievements.Count; i++)
            {
                Achievements[keys[i]].CurrentNumber = data.Value.AchievementNumbers[i];
                Achievements[keys[i]].Achieved = Achievements[keys[i]].IsAchieved;
            }

            for (int i = 0; i <= data.Value.LevelNumber; i++)
            {
                lockedLevels[i] = false;
            }

            TopPlayerIndex = data.Value.TopCharacterIndex;
            TopWeaponIndex = data.Value.TopWeaponIndex;
            BottomPlayerIndex = data.Value.BottomCharacterIndex;
            BottomWeaponIndex = data.Value.BottomWeaponIndex;
        }

        public static void SaveGame()
        {
            SaveData data = ExtractData();

            // Open a storage container.
            IAsyncResult result = OGE.Storage.BeginOpenContainer("Clean'Em Up", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = OGE.Storage.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            // Check to see whether the save exists.
            if (container.FileExists(filename))
                // Delete it so that we can create one fresh.
                container.DeleteFile(filename);

            // Create the file.
            Stream stream = container.CreateFile(filename);

            // Convert the object to XML data and put it in the stream.
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            serializer.Serialize(stream, data);

            // Close the file.
            stream.Close();

            // Dispose the container, to commit changes.
            container.Dispose();
        }

        public static void LoadGame()
        {
            IsFirstTime = false;

            // Open a storage container.
            IAsyncResult result = OGE.Storage.BeginOpenContainer("Clean'Em Up", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = OGE.Storage.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            // Check to see whether the save exists.
            if (!container.FileExists(filename))
            {
                // If not, dispose of the container and return.
                container.Dispose();

                UpdateData(null);
            }
            else
            {
                // Open the file.
                Stream stream = container.OpenFile(filename, System.IO.FileMode.Open);

                // Read the data from the file.
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                SaveData data = (SaveData)serializer.Deserialize(stream);

                // Close the file.
                stream.Close();

                // Dispose the container.
                container.Dispose();

                UpdateData(data);
            }
        }

        public static void DeleteSave()
        {
            Intialize();

            IAsyncResult result = OGE.Storage.BeginOpenContainer("Clean'Em Up", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = OGE.Storage.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();
            
            if (container.FileExists(filename))
            {
                container.DeleteFile(filename);
            }

            // Dispose the container, to commit the change.
            container.Dispose();
        }

        public static bool SaveExists()
        {
            IAsyncResult result = OGE.Storage.BeginOpenContainer("Clean'Em Up", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = OGE.Storage.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            if (container.FileExists(filename))
            {
                container.Dispose();
                return true;
            }

            // Dispose the container, to commit the change.
            container.Dispose();
            return false;
        }

        public static PlayerData GetCorrectPlayerData(int playerNumber)
        {
            return allPlayerData[playerNumber];
        }

        public static BaseWeapon GetCorrectPrimaryWeaponData(int weaponNumber)
        {
            return primaryWeaponData[weaponNumber];
        }

        public static BaseWeapon GetCorrectSecondaryWeaponData(int weaponNumber)
        {
            return secondaryWeaponData[weaponNumber];
        }

        public static EnemyData GetEnemyData(Type enemyType)
        {
            return dataLoader.Enemies[enemyNumberData[enemyType]];
        }

        public static void UnlockEnemy(Type enemyType)
        {
            dataLoader.Enemies[enemyNumberData[enemyType]].Locked = false;
        }

        public static void UpdateSurvivalVariables(List<int> scores)
        {
            if (GlobalVariables.SurvivalScores[GlobalVariables.SurvivalMode] < scores[GlobalVariables.SurvivalMode])
            {
                GlobalVariables.SurvivalScores[GlobalVariables.SurvivalMode] = scores[GlobalVariables.SurvivalMode];
            }
        }

        public static ScoreType GetSurvivalHUDVariable()
        {
            switch (GlobalVariables.SurvivalMode)
            {
                case 0:
                    return ScoreType.Points;
                case 1:
                    return ScoreType.Time;
                case 2:
                    return ScoreType.Files;
            }

            return ScoreType.Points;
        }

        public static Dictionary<Type, EnemyData> GetNewViruses()
        {
            Dictionary<Type, EnemyData> newViruses = new Dictionary<Type, EnemyData>();

            LevelData level = LevelData.GetNextLevel();
            foreach (GeneratorData data in level.GeneratorsData)
            {
                EnemyData enemyData = GlobalVariables.GetEnemyData(data.GetObjectType());

                if (enemyData.Locked)
                {
                    GlobalVariables.UnlockEnemy(data.GetObjectType());
                    enemyData.Locked = false;
                    newViruses.Add(data.GetObjectType(), enemyData);
                }
            }

            return newViruses;
        }

        public static int GetLargestLevel(int driveNumber)
        {
            for (int i = 0; i < LevelData.MAX_LEVEL_DRIVE_NUMBER; i++)
            {
                if (lockedLevels[i + (driveNumber - 1) * LevelData.MAX_LEVEL_DRIVE_NUMBER])
                {
                    return i;
                }
            }

            return LevelData.MAX_LEVEL_DRIVE_NUMBER;
        }

        public static void CheckAchievements()
        {
            foreach (AchievementData achievement in Achievements.Values)
            {
                if (achievement.IsAchieved && !achievement.Achieved)
                {
                    //TODO generate the achievement bar
                    InGameAchievementAnnouncer ingameAchievementAnnouncer = 
                        new InGameAchievementAnnouncer(null, achievement.Name + " has been achieved", new Color(150, 255, 130));
                    ingameAchievementAnnouncer.ChangePosition(OGE.HUDCamera.Height / 2 - 80);

                    OGE.CurrentWorld.AddOverLayer(ingameAchievementAnnouncer);

                    achievement.Achieved = true;
                }
            }
        }
    }
}
