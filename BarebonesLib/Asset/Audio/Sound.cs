using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Asset.Audio
{
    internal class Sound
    {
        private string _soundPath;
        private SoundEffectInstance _instance;

        internal bool IsActive
        {
            get { return _instance.State == SoundState.Playing; }
        }

        internal Sound(SoundEffect soundEffect, string soundPath)
        {
            _instance = soundEffect.CreateInstance();
            _soundPath = soundPath;
            Asset.Sound.DeclareSoundInstance(this);
            _instance.Play();
        }

        internal void Unload()
        {
            _instance.Stop();
            _instance.Dispose();
            Asset.Sound.UnloadSound(_soundPath);
        }
    }
}
