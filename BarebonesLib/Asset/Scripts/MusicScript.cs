using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Asset.Scripts
{
    /// <summary>
    /// A script that contains the properties to build a music object.
    /// </summary>
    public class MusicScript : Script
    {
        [JsonProperty]
        private string _musicPath;

        [JsonProperty]
        private TimeSpan _loopStart;

        [JsonProperty]
        private TimeSpan _loopEnd;

        /// <summary>
        /// The path to the music to load.
        /// </summary>
        [JsonIgnore]
        public string MusicPath
        {
            get { return _musicPath; }
        }

        /// <summary>
        /// The point of the track the loop should start from.
        /// </summary>
        [JsonIgnore]
        public TimeSpan LoopStart
        {
            get { return _loopStart; }
        }

        /// <summary>
        /// The point of the track the loop should end at.
        /// </summary>
        [JsonIgnore]
        public TimeSpan LoopEnd
        {
            get { return _loopEnd; }
        }
    }
}
