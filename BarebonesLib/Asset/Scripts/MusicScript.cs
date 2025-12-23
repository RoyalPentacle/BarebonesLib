using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Asset.Scripts
{
    public class MusicScript : Script
    {
        [JsonProperty]
        private string _musicPath;

        [JsonIgnore]
        public string MusicPath
        {
            get { return _musicPath; }
        }

        [JsonProperty]
        private TimeSpan _loopStart;

        [JsonIgnore]
        public TimeSpan LoopStart
        {
            get { return _loopStart; }
        }

        [JsonProperty]
        private TimeSpan _loopEnd;

        [JsonIgnore]
        public TimeSpan LoopEnd
        {
            get { return _loopEnd; }
        }
    }
}
