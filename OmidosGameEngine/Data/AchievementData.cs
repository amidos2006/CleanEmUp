using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Entity.Object.File;
using OmidosGameEngine.Entity.Object;
using Microsoft.Xna.Framework.Content;
using System.Xml.Serialization;

namespace OmidosGameEngine.Data
{
    public class AchievementData
    {
        public string Name;
        public string Description;
        public string ObjectType;
        public int GoalNumber;

        [XmlIgnore]
        public int CurrentNumber = 0;

        [XmlIgnore]
        public bool Achieved = false;

        public bool IsAchieved
        {
            get
            {
                return CurrentNumber >= GoalNumber;
            }
        }

        public AchievementData Clone()
        {
            AchievementData data = new AchievementData();

            data.Name = Name;
            data.Description = Description;
            data.ObjectType = ObjectType;
            data.GoalNumber = GoalNumber;
            data.CurrentNumber = CurrentNumber;
            data.Achieved = Achieved;

            return data;
        }

        public Type GetObjectType()
        {
            GeneratorData g = new GeneratorData();
            g.ObjectType = ObjectType;
            Type t = g.GetObjectType();

            if (t != null)
            {
                return t;
            }

            if (ObjectType.ToLower() == "health")
            {
                return typeof(MediumKitObject);
            }

            if (ObjectType.ToLower() == "shield")
            {
                return typeof(ShieldObject);
            }

            if (ObjectType.ToLower() == "quarantine")
            {
                return typeof(QuarantineObject);
            }

            if (ObjectType.ToLower() == "explosion")
            {
                return typeof(PlasticExplosionObject);
            }

            if (ObjectType.ToLower() == "mosiac")
            {
                return typeof(MosiacObject);
            }

            return null;
        }
    }
}
