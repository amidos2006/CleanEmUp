using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace OmidosGameEngine.Graphics
{
    /// <summary>
    /// Any drawing class must implement IGraphics interface
    /// </summary>
    interface IGraphics
    {
        /// <summary>
        /// draw the graphical object
        /// </summary>
        /// <param name="spriteBatch">spriteBatch supported by XNA to be used to draw</param>
        /// <param name="position">position of the graphical object to be drawn at</param>
        /// <param name="camera">camera object to draw the object with respect to camera</param>
        void Draw(Vector2 position, Camera camera);
    }
}
