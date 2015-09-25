using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace OmidosGameEngine.Graphics.Particles
{
    public class ParticlePattern
    {
        [ContentSerializer(CollectionItemName = "Particle")]
        public Particle[] Particles;

        public ParticlePattern()
        {
        }

        public ParticlePattern(int amount)
        {
            Particles = new Particle[amount];
        }
    }
}
