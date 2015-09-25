using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmidosGameEngine.Data
{
    public struct SaveData
    {
        public int LevelNumber;
        public int PlayerScore;
        public int TopCharacterIndex;
        public int TopWeaponIndex;
        public int BottomCharacterIndex;
        public int BottomWeaponIndex;
        public int ClearedLevels;
        public int[] SurvivalScores;
        public int[] AchievementNumbers;
        public bool[] LockedCharacters;
        public bool[] LockedWeapons;
        public bool[] LockedEnemies;
    }
}
