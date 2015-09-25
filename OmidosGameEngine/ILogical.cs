using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OmidosGameEngine
{
    public interface ILogical
    {
        /// <summary>
        /// This function is called when the object implenting it is updating its state
        /// </summary>
        /// <param name="gameTime">time variable provided by XNA</param>
        void Update(GameTime gameTime);
    }
}
