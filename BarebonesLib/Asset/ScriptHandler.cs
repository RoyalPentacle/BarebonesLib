using Barebones.Asset.Scripts;
using Barebones.Config;
using Newtonsoft.Json;

namespace Barebones.Asset
{
    /// <summary>
    /// Functions for finding scripts.
    /// </summary>
    public static class ScriptFinder
    {

        // Populated as we go, stores the most recent scripts we've used, to avoid unnecessary loading on repeated script requests.
        private static Dictionary<string, Script> _scriptCache = new Dictionary<string, Script>();

        // Since Dictionaries do not have a way to determine the order elements were inserted, we use this list to track that instead.
        private static List<string> _sortedCache = new List<string>();

        private static long _cacheSize = 0L;

        private static Mutex _mutex = new Mutex();

        /// <summary>
        /// Find a script of the given type and path, loading it if we do not have it cached.
        /// </summary>
        /// <typeparam name="T">The type of script, must be a Script or inherit Script</typeparam>
        /// <param name="scriptPath">The path of the script to find.</param>
        /// <returns>A script of the specified type.</returns>
        public static T FindScript<T>(string scriptPath) where T : Script
        {
            try
            {
                // We don't want multiple threads trying to access any of this at the same time.
                _mutex.WaitOne();
                // Try to get the script paired to the provided key...
                if (_scriptCache.TryGetValue(scriptPath, out var value))
                {
                    if (value is T typedScript) // If the script is the correct type
                    {
                        // Set the script to be the most recent found, then return it.
                        UpdateCachedScriptList(scriptPath);
                        return typedScript;
                    }

                    // Otherwise, spit out an error and return null.
                    // This shouldn't happen so long as proper naming conventions for scripts is followed.
                    Verbose.WriteErrorMajor($"SCRIPT: Found {value.GetType}, {scriptPath} but it was not the {typeof(T).Name} we expected!");
                    return null;
                }
                else // Otherwise, we don't have the script cached, so load it, cache it, return it.
                {

                    var script = LoadScript<T>(scriptPath);
                    UpdateCachedScriptList(scriptPath);
                    _scriptCache.Add(scriptPath, script);
                    _cacheSize += script.FileSize;
                    return script;
                }
            }
            catch (Exception ex) // Things can go wrong sometimes.
            {
                Verbose.WriteErrorMajor($"SCRIPT: Failed to find {typeof(T).Name}, {scriptPath} \n EX: {ex.Message}");
                return null;
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            
        }

        /// <summary>
        /// Loads a script of a given type from the disk and returns it.
        /// </summary>
        /// <typeparam name="T">The type of script to load.</typeparam>
        /// <param name="scriptPath">The name of the script to load.</param>
        /// <returns>A script of the specified type.</returns>
        private static T LoadScript<T>(string scriptPath) where T : Script
        {
            StreamReader? reader = null;
            try 
            {
                // Get the path, read that file, json parse it, return it.
                reader = File.OpenText(scriptPath);
                string json = reader.ReadToEnd();
                reader.Close();
                var script = JsonConvert.DeserializeObject<T>(json);
                Verbose.WriteLogMinor($"SCRIPT: Loaded {typeof(T).Name}, {scriptPath}");
                script.SetFilesize(new FileInfo(scriptPath).Length);
                return script;
            }
            catch (Exception ex)
            {
                Verbose.WriteErrorMajor($"SCRIPT: Failed to load {typeof(T).Name}, {scriptPath} \n EX: {ex.Message}");
                reader?.Close();
                return null;
            }
        }

        /// <summary>
        /// Keeps the ordered list in sync with the dictionary cache, adding a new entry, or moving an existing one to the top.
        /// </summary>
        /// <param name="scriptName">The name of the script to update.</param>
        private static void UpdateCachedScriptList(string scriptName)
        {
            // Remove the script from our ordered list, won't do anything if it doesn't exist.
            _sortedCache.Remove(scriptName);

            // Insert the script at the top of the ordered list.
            _sortedCache.Insert(0, scriptName);

            // Remove excess.
            TrimScriptCache();
        }

        /// <summary>
        /// Removes any excess scripts beyond the maximum cached amount.
        /// </summary>
        private static void TrimScriptCache()
        {
            while (_cacheSize > Engine.ScriptCacheMaxSize)
            {
                string toRemove = _sortedCache.Last();
                _cacheSize -= _scriptCache[toRemove].FileSize;
                _scriptCache[toRemove].Unload();
                _scriptCache.Remove(toRemove);
                _sortedCache.RemoveAt(_sortedCache.Count - 1);
            }
        }
    }
}