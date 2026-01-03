using Newtonsoft.Json;
using System.Collections.Concurrent;
namespace Barebones.Config
{
    /// <summary>
    /// A class that contains handlers for localization of strings.
    /// Broadly speaking. Do not change the language while a game is running. Require a restart.
    /// </summary>
    public static class Language
    {
        private static readonly ConcurrentDictionary<string, string> _localization = new ConcurrentDictionary<string, string>();
        private static string _currentLang = "en";

        /// <summary>
        /// The current language of the game.
        /// </summary>
        public static string CurrentLanguage
        {
            get { return _currentLang; }
        }

        /// <summary>
        /// Only do this at launch, before loading the first language file.
        /// </summary>
        /// <param name="lang"></param>
        public static void SetLanguage(string lang)
        {
            _currentLang = lang;
        }

        /// <summary>
        /// Loads a localization set from a file.
        /// Appends to the existing localization dictionary if the language of the file is the same as the current.
        /// Writes an error if it is not the same language.
        /// </summary>
        /// <param name="languagePath"></param>
        public static void LoadLanguageFile(string languagePath)
        {
            try
            {
                using (StreamReader sr = File.OpenText(languagePath))
                {
                    string json = sr.ReadToEnd();
                    sr.Close();
                    Dictionary<string, string> _tempLocal = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> pair in _tempLocal)
                    {
                        _localization.AddOrUpdate(pair.Key, pair.Value, UpdateString);
                    }
                }
            }
            catch(Exception ex)
            {
                Verbose.WriteErrorMajor($"Failed to load localization file: {languagePath}\n Ex: {ex.Message}");
            }
        }

        private static string UpdateString(string key, string value)
        {
            return value;
        }

        /// <summary>
        /// Gets the corresponding string to the given localization token.
        /// </summary>
        /// <param name="token">The token to translate.</param>
        /// <returns>The translated string, or the token if no string exists.</returns>
        public static string Translate(string token)
        {
            if (_localization.TryGetValue(token, out var result))
            {
                return result;
            }
            else
            {
                Verbose.WriteErrorMinor($"Missing localization string for token: {token} for lang: {_currentLang}");
                return token;
            }
        }
    }
}
