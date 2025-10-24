using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Config
{
    /// <summary>
    /// Contains functions for checking for player input.
    /// </summary>
    public static class Control
    {
        /// <summary>
        /// Checks if the specified key was pressed this frame.
        /// Only if the key was not pressed the previous frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key has just been pressed. False otherwise.</returns>
        public static bool KeyPressed(Keys key)
        {
            return Engine.OldKeyboardState[key] == KeyState.Up && Engine.NewKeyboardState[key] == KeyState.Down;
        }

        /// <summary>
        /// Checks if the specified key is being held down.
        /// Specifically, is the key being pressed at all.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key is being pressed. False otherwise.</returns>
        public static bool KeyHeld(Keys key)
        {
            return Engine.NewKeyboardState[key] == KeyState.Down;
        }

        /// <summary>
        /// Checks if the specified key was released this frame.
        /// Only if the key was pressed the previous frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was released. False otherwise.</returns>
        public static bool KeyReleased(Keys key)
        {
            return Engine.OldKeyboardState[key] == KeyState.Down && Engine.NewKeyboardState[key] == KeyState.Up;
        }
    }
}
