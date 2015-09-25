using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OmidosGameEngine.Entity.Generator;
using Microsoft.Xna.Framework.Content;
using OmidosGameEngine.Entity.Object.File;
using OmidosGameEngine.Entity.Player.Data;
using OmidosGameEngine.Entity.Boss;
using OmidosGameEngine.Entity;
using System.Xml.Serialization;
using System.IO;

namespace OmidosGameEngine.Data
{
    public class LevelData
    {
        public const int MAX_LEVEL_DRIVE_NUMBER = 15;
        
        public string LevelName;
        public Vector2 Dimension;
        public Vector2 StartPosition;
        public int TimeEndLevel;
        public int NumberOfDocumentFiles;
        public int TopPlayerBodyIndex;
        public int BottomPlayerBodyIndex;
        public int TopPlayerWeaponIndex;
        public int BottomPlayerWeaponIndex;
        public string BossName;
        [XmlElement(ElementName = "Generator")]
        public GeneratorData[] GeneratorsData;

        [XmlIgnore]
        public List<BaseGenerator> Generator
        {
            set;
            get;
        }

        [XmlIgnore]
        public int NumberOfZip
        {
            set;
            get;
        }

        public void LoadLevel()
        {
            bool noDocumentFiles = true;
            Generator = new List<BaseGenerator>();
            
            NumberOfZip = 0;

            foreach (GeneratorData data in GeneratorsData)
            {
                for (int i = 0; i < data.NumberOfTimes; i++)
                {
                    if (data.GetObjectType() == typeof(ZipFile))
                    {
                        NumberOfZip += data.NumberOfObjects;
                    }

                    if (data.GetObjectType() == typeof(DocumentFile))
                    {
                        noDocumentFiles = false;
                    }

                    Generator.Add(new BaseGenerator(data.GetObjectType(), data.NumberOfObjects,
                        data.StartTime, data.InterGenerationTime, data.RandomStartTime, data.RandomInterGenerationTime));
                }
            }

            if (noDocumentFiles)
            {
                NumberOfDocumentFiles = -1;
            }
        }

        public BaseEntity GetBoss()
        {
            if (BossName.ToLower() == "trojasaur")
            {
                return new TroyBoss();
            }

            if (BossName.ToLower() == "tera-virus")
            {
                return new VirusBoss();
            }

            if (BossName.ToLower() == "hackinzord")
            {
                return new HackintoshBoss();
            }

            if (BossName.ToLower() == "popur")
            {
                return new PopurBoss();
            }

            if (BossName.ToLower() == "xxxii")
            {
                return new DosBossController();
            }

            return null;
        }

        public bool IsSelectedLevel()
        {
            return !(TopPlayerWeaponIndex < 0 || TopPlayerBodyIndex < 0 || BottomPlayerBodyIndex < 0 || BottomPlayerWeaponIndex < 0);
        }

        public void LoadSelectedLevelData()
        {
            if (TopPlayerBodyIndex > -1)
            {
                PlayerData topPlayer = GlobalVariables.GetCorrectPlayerData(TopPlayerBodyIndex);
                if (TopPlayerWeaponIndex > -1)
                {
                    topPlayer.Weapon = GlobalVariables.GetCorrectPrimaryWeaponData(TopPlayerWeaponIndex);
                }

                GlobalVariables.TopPlayer = topPlayer.Clone();
                if (TopPlayerWeaponIndex > -1)
                {
                    GlobalVariables.TopPlayer.Weapon = GlobalVariables.GetCorrectPrimaryWeaponData(TopPlayerWeaponIndex);
                }
            }

            if (BottomPlayerBodyIndex > -1)
            {
                PlayerData bottomPlayer = GlobalVariables.GetCorrectPlayerData(BottomPlayerBodyIndex);
                if (BottomPlayerWeaponIndex > -1)
                {
                    bottomPlayer.Weapon = GlobalVariables.GetCorrectSecondaryWeaponData(BottomPlayerWeaponIndex);
                }

                GlobalVariables.BottomPlayer = bottomPlayer.Clone();
                if (TopPlayerWeaponIndex > -1)
                {
                    GlobalVariables.BottomPlayer.Weapon = GlobalVariables.GetCorrectSecondaryWeaponData(BottomPlayerWeaponIndex);
                }
            }
        }

        public static LevelData GetLevel(int levelNumber)
        {
            if (levelNumber > MAX_LEVEL_DRIVE_NUMBER || GlobalVariables.CurrentDrive > DriveData.MAX_DRIVE_NUMBER)
            {
                return null;
            }

            XmlSerializer xml = new XmlSerializer(typeof(LevelData));
            StreamReader reader = new StreamReader(@"Content\Levels\Drive" + GlobalVariables.CurrentDrive + @"\Level" + levelNumber + ".xml");
            LevelData level = (LevelData)xml.Deserialize(reader);
            reader.Close();

            return level;
        }

        public static LevelData GetNextLevel()
        {
            if (GlobalVariables.CurrentLevel > MAX_LEVEL_DRIVE_NUMBER || GlobalVariables.CurrentDrive > DriveData.MAX_DRIVE_NUMBER)
            {
                return null;
            }

            XmlSerializer xml = new XmlSerializer(typeof(LevelData));
            StreamReader reader = new StreamReader(@"Content\Levels\Drive" + GlobalVariables.CurrentDrive + @"\Level" + GlobalVariables.CurrentLevel + ".xml");
            LevelData level = (LevelData)xml.Deserialize(reader);
            reader.Close();

            return level;
        }

        public static LevelData GetSurvivalLevel()
        {
            if (GlobalVariables.SurvivalMode > GlobalVariables.SURVIVAL_TYPES)
            {
                return null;
            }

            XmlSerializer xml = new XmlSerializer(typeof(LevelData));
            StreamReader reader = new StreamReader(@"Content\Levels\Survival\Survival" + (GlobalVariables.SurvivalMode + 1) + ".xml");
            LevelData level = (LevelData)xml.Deserialize(reader);
            reader.Close();

            return level;
        }
    }
}
