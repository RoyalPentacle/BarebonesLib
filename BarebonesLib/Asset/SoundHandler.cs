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
            internal static void Init()
            {
                int sampleRate = 44100; // The samplerate of the fallback sound.
                float frequency = 110f; // The frequency of the fallback sound, 110 = A2
                float duration = 0.25f; // The duration in seconds of the fallback sound.
                short amplitude = short.MaxValue / 80; // The volume of the fallback sound.
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
        private static Mutex _mutex = new Mutex();

        private static List<Audio.Sound> _allSounds = new List<Audio.Sound>();

        /// <summary>
        /// The current size of the sound cache.
        /// </summary>
        public static long CacheSize
        {
            get { return _cacheSize; }
        }

        /// <summary>
        /// Ask the handler to return a SoundEffect with a given name. If we don't have it, try to load it.
        /// </summary>
        /// <param name="soundPath">The name of the sound to get.</param>
        /// <returns>A SoundEffect.</returns>
        public static SoundEffect GetSound(string soundPath)
        {
            try // Try to get the sound from the dictionary
            {
                _mutex.WaitOne();
                SoundMap sound = _soundDict[soundPath];
                sound.Count++;
                return sound.Sound;
            }
            catch // If we can't, load it instead and return that.
            {
                LoadSound(soundPath);
                return _soundDict[soundPath].Sound;
            }
            finally 
            { 
                _mutex.ReleaseMutex(); 
            }
        }

        /// <summary>
        /// Loads a sound into our sound dictionary.
        /// </summary>
        /// <param name="soundPath">The name of the sound to load.</param>
        private static void LoadSound(string soundPath)
        {
            // Create sound definition
            try 
            {
                GetSoundFromCache(soundPath);
            }
            catch 
            {
                SoundMap newSound = new SoundMap(soundPath);
                _soundDict.Add(soundPath, newSound);
            }
        }

        /// <summary>
        /// Let the handler know that an object is no longer using a given sound.
        /// If no other objects are using the sound, dispose of it, unless it's permanent.
        /// </summary>
        /// <param name="soundPath">The name of the sound to unload.</param>
        public static void UnloadSound(string soundPath)
        {
            try // Try to unload the sound
            {
                _mutex.WaitOne();
                SoundMap sound = _soundDict[soundPath];
                sound.Count--;
                if (sound.Count <= 0)
                {
                    AddSoundToCache(soundPath, sound);
                }
                return;
            }
            catch (Exception ex) // If something goes wrong, which it could, spit out a minor error.
            {
                Verbose.WriteErrorMinor($"SOUND: Error unloading sound: {soundPath}\n Doing nothing about this? EX: {ex.Message}");
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Move the specified sound from active use into the cache.
        /// Trim the cache.
        /// </summary>
        /// <param name="soundPath">The name of the sound.</param>
        /// <param name="sound">The SoundMap representing the sound.</param>
        private static void AddSoundToCache(string soundPath, SoundMap sound)
        {
            _soundDict.Remove(soundPath);
            _soundCache.Add(soundPath, sound);
            _sortedCache.Add(soundPath);
            _cacheSize += sound.FileSize;
            TrimSoundCache();
        }

        /// <summary>
        /// Move the specified sound from the cache into active use.
        /// </summary>
        /// <param name="soundPath">The name of the sound.</param>
        private static void GetSoundFromCache(string soundPath)
        {
            SoundMap sound = _soundCache[soundPath];
            _soundCache.Remove(soundPath);
            _sortedCache.Remove(soundPath);
            _cacheSize -= sound.FileSize;
            sound.Count++;
            _soundDict.Add(soundPath, sound);
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

        internal static void DeclareSoundInstance(Audio.Sound sound)
        {
            _allSounds.Add(sound);
        }

        internal static void DisposeStoppedInstances()
        {
            for (int i = _allSounds.Count - 1; i >= 0; i--)
            {
                if (!_allSounds[i].IsActive)
                {
                    _allSounds[i].Unload();
                    _allSounds.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Play the sound found at the specified path.
        /// </summary>
        /// <param name="soundPath">The path to the sound.</param>
        public static void PlaySound(string soundPath)
        {
            SoundEffect soundEffect = GetSound(soundPath);
            Audio.Sound sound = new Audio.Sound(soundEffect, soundPath);
        }
    }
}
