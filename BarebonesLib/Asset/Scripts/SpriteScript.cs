using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Barebones.Drawable;

namespace Barebones.Asset.Scripts
{
    /// <summary>
    /// A script that contains the properties to build a sprite object.
    /// </summary>
    public class SpriteScript : Script
    {
        [JsonProperty]
        private string _texturePath;

        [JsonProperty]
        private Dictionary<string, Sprite.Anim> _anims = new Dictionary<string, Sprite.Anim>();

        [JsonProperty]
        private string _defaultAnim;

        /// <summary>
        /// The path to the texture for the sprite.
        /// </summary>
        [JsonIgnore]
        public string TexturePath
        {
            get { return _texturePath; }
        }
        /// <summary>
        /// The default animation this sprite should use upon loading.
        /// </summary>
        [JsonIgnore]
        public string DefaultAnim
        {
            get { return _defaultAnim; }
        }

        /// <summary>
        /// The Dictionary of Animations for the sprite.
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, Sprite.Anim> Anims
        {
            get { return _anims; }
        }
    }
}

