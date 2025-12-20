using Barebones.Drawable.Particles;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Asset.Scripts
{
    /// <summary>
    /// A particle script contains the necessary data for constructing a particle system.
    /// </summary>
    public class ParticleScript : Script
    {

        [JsonProperty]
        private GeneratorPattern[] _generators;

        /// <summary>
        /// The array of GeneratorPatterns for constructing a particle system.
        /// </summary>
        [JsonIgnore]
        public GeneratorPattern[] Generators
        {
            get { return _generators; }
        }

        [JsonProperty]
        private AttractorPattern[] _attractors;

        /// <summary>
        /// The array of AttractorPatterns for constructing a particle system.
        /// </summary>
        [JsonIgnore]
        public AttractorPattern[] Attractors
        {
            get { return _attractors; }
        }

        [JsonProperty]
        private string? _luaScript = null;

        /// <summary>
        /// The lua script for this particle system, if applicable.
        /// </summary>
        [JsonIgnore]
        public string? LuaScript
        {
            get { return _luaScript; }
        }
    }
}
