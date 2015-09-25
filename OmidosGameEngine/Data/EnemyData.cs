using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Xml.Serialization;

namespace OmidosGameEngine.Data
{
    public struct EnemyData
    {
        public string Name;
        public string Description;
        [XmlIgnore]
        public bool Locked;
    }
}
