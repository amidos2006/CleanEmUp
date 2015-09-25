using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidosGameEngine.Graphics.Particles;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine.Entity.ParticleGenerator
{
    public interface IParticleGenerator
    {
        void GenerateParticles(Vector2 position);
    }
}
