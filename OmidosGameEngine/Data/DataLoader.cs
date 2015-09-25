using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Xml.Serialization;

namespace OmidosGameEngine.Data
{
    public class DataLoader
    {
        [XmlElement(ElementName = "Character")]
        public CharacterData[] Characters;
        [XmlElement(ElementName = "Weapon")]
        public WeaponData[] Weapons;
        [XmlElement(ElementName = "Enemy")]
        public EnemyData[] Enemies;

        public DataLoader Clone()
        {
            DataLoader d = new DataLoader();
            d.Characters = new CharacterData[Characters.Length];
            d.Weapons = new WeaponData[Weapons.Length];
            d.Enemies = new EnemyData[Enemies.Length];
            
            for (int i = 0; i < Characters.Length; i++)
            {
                d.Characters[i].Name = Characters[i].Name;
                d.Characters[i].Description = Characters[i].Description;
                d.Characters[i].Overclocking = Characters[i].Overclocking;
                d.Characters[i].Locked = Characters[i].Locked;
                d.Characters[i].UnlockPoints = Characters[i].UnlockPoints;
            }

            for (int i = 0; i < Weapons.Length; i++)
            {
                d.Weapons[i].Name = Weapons[i].Name;
                d.Weapons[i].Description = Weapons[i].Description;
                d.Weapons[i].Locked = Weapons[i].Locked;
                d.Weapons[i].UnlockPoints = Weapons[i].UnlockPoints;
                d.Weapons[i].ClearedLevels = Weapons[i].ClearedLevels;
            }

            for (int i = 0; i < Enemies.Length; i++)
            {
                d.Enemies[i].Name = Enemies[i].Name;
                d.Enemies[i].Description = Enemies[i].Description;
                d.Enemies[i].Locked = false;
            }

            return d;
        }
    }
}
