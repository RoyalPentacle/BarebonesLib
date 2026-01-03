using Newtonsoft.Json;

namespace Barebones.Asset.Scripts
{
    /// <summary>
    /// A bundle is a wrapper for a String:String
    /// </summary>
    public class Bundle : Script
    {
        [JsonProperty]
        private Dictionary<string, string>? _pairs;

        /// <summary>
        /// The Dictionary of String:String stored in this bundle.
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, string>? Pairs
        {
            get { return _pairs; }
        }
        /// <summary>
        /// Constructs a null bundle as a fallback script.
        /// </summary>
        public Bundle()
        {
            _pairs = null;
        }
    }
}
