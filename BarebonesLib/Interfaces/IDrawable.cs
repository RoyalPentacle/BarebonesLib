using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Interfaces
{
    /// <summary>
    /// Any object that can be drawn at a specified position.
    /// </summary>
    public interface IDrawnObject
    {

        /// <summary>
        /// Draw this object at the specified position.
        /// </summary>
        /// <param name="position">The position to draw the object.</param>
        public abstract void Draw(Vector2 position);
    }
}
