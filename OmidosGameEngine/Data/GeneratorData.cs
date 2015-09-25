using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Entity.Enemy;
using OmidosGameEngine.Entity.Object.File;

namespace OmidosGameEngine.Data
{
    public class GeneratorData
    {
        public int NumberOfTimes;
        public string ObjectType;
        public int NumberOfObjects;
        public float StartTime;
        public float RandomStartTime;
        public float InterGenerationTime;
        public float RandomInterGenerationTime;

        public Type GetObjectType()
        {
            if (ObjectType.ToLower() == "viruseous")
            {
                return typeof(VirusEnemy);
            }

            if (ObjectType.ToLower() == "dos")
            {
                return typeof(DOSEnemy);
            }

            if (ObjectType.ToLower() == "hackintosh")
            {
                return typeof(HackintoshEnemy);
            }

            if (ObjectType.ToLower() == "malzone")
            {
                return typeof(MalzoneEnemy);
            }

            if (ObjectType.ToLower() == "popur")
            {
                return typeof(PopurEnemy);
            }

            if (ObjectType.ToLower() == "trojan")
            {
                return typeof(TroyEnemy);
            }

            if (ObjectType.ToLower() == "worm")
            {
                return typeof(WormEnemy);
            }

            if (ObjectType.ToLower() == "dosx")
            {
                return typeof(SlowEnemy);
            }

            if (ObjectType.ToLower() == "hakintroy")
            {
                return typeof(Hackintosh2Enemy);
            }

            if (ObjectType.ToLower() == "malbomb")
            {
                return typeof(Malzone2Enemy);
            }

            if (ObjectType.ToLower() == "roamer")
            {
                return typeof(BouncerEnemy);
            }

            if (ObjectType.ToLower() == "camouflager")
            {
                return typeof(Troy2Enemy);
            }

            if (ObjectType.ToLower() == "hanger")
            {
                return typeof(DOS2Enemy);
            }

            if (ObjectType.ToLower() == "auto pop-up")
            {
                return typeof(Popur2Enemy);
            }

            if (ObjectType.ToLower() == "document file")
            {
                return typeof(DocumentFile);
            }

            if (ObjectType.ToLower() == "exe file")
            {
                return typeof(ExeFile);
            }

            if (ObjectType.ToLower() == "zip file")
            {
                return typeof(ZipFile);
            }

            return null;
        }
    }
}
