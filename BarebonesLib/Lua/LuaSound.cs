using Barebones.Asset.Scripts;
using Barebones.Asset;
using Barebones.Config;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Lua
{
    /// <summary>
    /// Contains functions for sound that lua can access.
    /// </summary>
    public static class Sound
    {
        /// <summary>
        /// Play a sound from a given soundScript path.
        /// </summary>
        /// <param name="scriptPath"></param>
        public static void PlaySound(string scriptPath)
        {
            SoundScript soundScript = ScriptFinder.FindScript<SoundScript>(scriptPath);
            Asset.Sound.PlaySound(soundScript.SoundPath);
        }
    }

}
