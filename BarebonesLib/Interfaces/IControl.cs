using Barebones.Windows;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Interfaces
{
    /// <summary>
    /// Objects that implement this interface can be used as controls in a window.
    /// </summary>
    public interface IControl
    {

        /// <summary>
        /// The parent window.
        /// </summary>
        public abstract Window Parent
        {
            get;
        }

        /// <summary>
        /// The name of the control.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Update the control.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Check for inputs on the control.
        /// </summary>
        public abstract void CheckInput();

        /// <summary>
        /// Unload the control.
        /// </summary>
        public abstract void Unload();

        /// <summary>
        /// Draw the control.
        /// </summary>
        public abstract void Draw();


    }
}
