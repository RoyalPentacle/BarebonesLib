using Microsoft.Xna.Framework.Audio;
using Barebones.Config;

namespace Barebones.Asset
{
    /// <summary>
    /// Handler for all sounds.
    /// </summary>
    public static class Sound
    {
        /// <summary>
        /// Sounds that are shared across all classes, independent of the sprite system.
        /// </summary>
        public static class Shared
        {
            private static SoundEffect _fallback; // A very harsh square wave, used as a fallback if a sound fails to load.
            
            /// <summary>
            /// A harsh square wave SoundEffect, used as a fallback if a sound fails to load.
            /// </summary>
            public static SoundEffect FallbackSound
            {
                get { return _fallback; }
            }

            /// <summary>
            /// Initalize the shared sound class.
            /// </summary>
            public static void Init()
            {
                int sampleRate = 44100; // The samplerate of the fallback sound.
                float frequency = 110f; // The frequency of the fallback sound, 110 = A2
                float duration = 0.25f; // The duration in seconds of the fallback sound.
                short amplitude = short.MaxValue / 80; // The volume of the fallback sound. Note that this will 
                float dutyCycle = 0.25f; // The duty cycle of the square wave.


                byte[] buffer = new byte[(int)(sampleRate * duration * 2)];

                for (int i = 0; i < buffer.Length / 2; i++)
                {
                    double phase = (i * frequency / sampleRate) % 1.0;
                    short sample = (short)(amplitude * (phase < dutyCycle ? 1 : -1));

                    buffer[i * 2] = (byte)(sample & 0xFF);
                    buffer[i * 2 + 1] = (byte)(sample >> 8);
                }


                _fallback = new SoundEffect(buffer, sampleRate, AudioChannels.Mono);
            }
        }

        /// <summary>
        /// An object that represents a single loaded SoundEffect and supporting info.
        /// </summary>
        private class SoundMap
        {
            // The actual loaded sound.
            private SoundEffect _soundEffect;

            // The number of things currently using this sound.
            private int _count;

            private long _fileSize;

            /// <summary>
            /// The SoundEffect stored in this SoundMap.
            /// </summary>
            public SoundEffect Sound
            {
                get { return _soundEffect; }
            }

            /// <summary>
            /// The use count of this SoundMap.
            /// </summary>
            public int Count
            {
                get { return _count; }
                set { _count = value; }
            }

            public long FileSize
            {
                get { return _fileSize; }
            }

            /// <summary>
            /// Constructs a new SoundMap from the specified arguments.
            /// </summary>
            /// <param name="soundPath">The path of the sound to load.</param>
            public SoundMap(string soundPath)
            {
                try // Attempt to load the specified sound.
                {
                    _fileSize = new FileInfo(soundPath).Length;
                    _soundEffect = SoundEffect.FromFile(soundPath);
                }
                catch (Exception ex) // But if we can't, do nothing and print an error.
                {
                    _soundEffect = Shared.FallbackSound;
                    Verbose.WriteErrorMajor($"SOUND: Error loading file at: {soundPath}\n EX: {ex.Message}");
                }

                _count = 1; // A brand new sound is being used by one thing.
            }

            /// <summary>
            /// If the sound isn't null, dispose of the sound.
            /// </summary>
            public void Unload()
            {
                // Make sure we don't dispose the shared sounds.
                if (_soundEffect != null && _soundEffect != Shared.FallbackSound)
                {
                    _soundEffect.Dispose();
                }
            }
        }

        // A dictionary that contains all stored sounds.
        private static Dictionary<string, SoundMap> _soundDict = new Dictionary<string, SoundMap>();

        private static Dictionary<string, SoundMap> _soundCache = new Dictionary<string, SoundMap>();
        private static List<string> _sortedCache = new List<string>();
        private static long _cacheSize = 0L;

        /// <summary>
        /// Ask the handler to return a SoundEffect with a given name. If we don't have it, try to load it.
        /// </summary>
        /// <param name="soundName">The name of the sound to get.</param>
        /// <returns>A SoundEffect.</returns>
        public static SoundEffect GetSound(string soundName)
        {
            try // Try to get the sound from the dictionary
            {
                SoundMap sound = _soundDict[soundName];
                sound.Count++;
                return sound.Sound;
            }
            catch // If we can't, load it instead and return that.
            {
                LoadSound(soundName);
                return _soundDict[soundName].Sound;
            }
        }

        /// <summary>
        /// Loads a sound into our sound dictionary.
        /// </summary>
        /// <param name="soundName">The name of the sound to load.</param>
        public static void LoadSound(string soundName)
        {
            // Create sound definition
            try 
            {
                GetSoundFromCache(soundName);
            }
            catch 
            {
                SoundMap newSound = new SoundMap("get path from sound");
                _soundDict.Add(soundName, newSound);
            }
        }

        /// <summary>
        /// Let the handler know that an object is no longer using a given sound.
        /// If no other objects are using the sound, dispose of it, unless it's permanent.
        /// </summary>
        /// <param name="soundName">The name of the sound to unload.</param>
        public static void UnloadSound(string soundName)
        {
            try // Try to unload the sound
            {
                SoundMap sound = _soundDict[soundName];
                sound.Count--;
                if (sound.Count <= 0)
                {
                    AddSoundToCache(soundName, sound);
                }
                return;
            }
            catch (Exception ex) // If something goes wrong, which it could, spit out a minor error.
            {
                Verbose.WriteErrorMinor($"SOUND: Error unloading sound: {soundName}\n Doing nothing about this? EX: {ex.Message}");
            }
        }

        /// <summary>
        /// Move the specified sound from active use into the cache.
        /// Trim the cache.
        /// </summary>
        /// <param name="soundName">The name of the sound.</param>
        /// <param name="sound">The SoundMap representing the sound.</param>
        private static void AddSoundToCache(string soundName, SoundMap sound)
        {
            _soundDict.Remove(soundName);
            _soundCache.Add(soundName, sound);
            _sortedCache.Add(soundName);
            _cacheSize += sound.FileSize;
            TrimSoundCache();
        }

        /// <summary>
        /// Move the specified sound from the cache into active use.
        /// </summary>
        /// <param name="soundName">The name of the sound.</param>
        private static void GetSoundFromCache(string soundName)
        {
            SoundMap sound = _soundCache[soundName];
            _soundCache.Remove(soundName);
            _sortedCache.Remove(soundName);
            _cacheSize -= sound.FileSize;
            sound.Count++;
            _soundDict.Add(soundName, sound);
        }

        /// <summary>
        /// Removes any excess sounds beyond the maximum cache size.
        /// </summary>
        private static void TrimSoundCache()
        {
            while (_cacheSize > Engine.SoundCacheMaxSize)
            {
                string nameRemove = _sortedCache[0];
                SoundMap sound = _soundCache[nameRemove];
                _cacheSize -= sound.FileSize;
                sound.Unload();
                _soundCache.Remove(nameRemove);
                _sortedCache.RemoveAt(0);
            }
        }
    }
}
