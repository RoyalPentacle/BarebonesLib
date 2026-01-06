using Newtonsoft.Json;
using Barebones.Drawable;
using Microsoft.Xna.Framework;
using System.Collections.Frozen;

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
        private Dictionary<string, ComplexSprite.Anim> _anims = new Dictionary<string, ComplexSprite.Anim>();

        [JsonProperty]
        private string _defaultAnim;

        [JsonProperty]
        private Dictionary<uint, Color>[] _colourPalettes;


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
        public Dictionary<string, ComplexSprite.Anim> Anims
        {
            get { return _anims; }
        }

        /// <summary>
        /// The array of dictionaries for colour replacements.
        /// </summary>
        [JsonIgnore]
        public Dictionary<uint, Color>[] ColourPalettes
        {
            get { return _colourPalettes; }
        }

        /// <summary>
        /// Creates an empty SpriteScript for fallback purposes.
        /// </summary>
        public SpriteScript()
        {
            _texturePath = "fallback";
            _defaultAnim = "IDLE";
        }
    }
}
