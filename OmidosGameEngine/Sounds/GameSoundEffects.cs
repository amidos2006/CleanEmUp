using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OmidosGameEngine.Sounds
{
    public class GameSoundEffects
    {
        [XmlElement(ElementName = "SoundEffectProperties")]
        public SoundEffectProperties[] properties;
    }
}
