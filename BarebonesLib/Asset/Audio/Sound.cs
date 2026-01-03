using Barebones.Asset.Scripts;
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

        internal Sound(string scriptPath)
        {
            SoundScript script = ScriptFinder.FindScript<SoundScript>(scriptPath);
            SoundEffect soundEffect = Asset.Sound.GetSound(script.SoundPath);
            _instance = soundEffect.CreateInstance();
            _soundPath = script.SoundPath;
            Asset.Sound.DeclareSoundInstance(this);
            _instance.Volume = Engine.SoundVolume;
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
