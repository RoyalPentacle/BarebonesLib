using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Asset.Scripts
{
    /// <summary>
    /// A script that contains the properties to build a sound object.
    /// </summary>
    public class SoundScript : Script
    {
        [JsonProperty]
        private string _soundPath;

        /// <summary>
        /// The path to the file for this sound.
        /// </summary>
        [JsonIgnore]
        public string SoundPath
        {
            get { return _soundPath; }
        }
    }
}
