using Barebones.Asset;
using Barebones.Asset.Scripts;
using Barebones.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Drawable
{
    /// <summary>
    /// The base class for all sprites.
    /// </summary>
    public abstract class BaseSprite
    {

        private protected string _texturePath;

        private protected Color _colour;

        private protected Texture2D _texture;

        private protected Rectangle _drawRec;




        /// <summary>
        /// The path of the texture loaded.
        /// </summary>
        public string TexturePath
        {
            get { return _texturePath; }
        }

        /// <summary>
        /// The current colour of the sprite.
        /// </summary>
        public Color Colour
        {
            get { return _colour; }
        }



        /// <summary>
        /// Constructs a new SimpleSprite from the SpriteScript at the provided path.
        /// Also outputs that SpriteScript to be used by derived constructors.
        /// </summary>
        /// <param name="scriptPath">The path to the SpriteScript to load.</param>
        /// <param name="script">The SpriteScript loaded.</param>
        public BaseSprite(string scriptPath, out SpriteScript script)
        {
            script = ScriptFinder.FindScript<SpriteScript>(scriptPath);
            _texturePath = script.TexturePath;
            if (Engine.IsMainThread)
                _texture = Textures.GetTexture(_texturePath);
            else
                Textures.GetTextureAsync(this);
            
        }
        
        internal void GetTextureAsync()
        {
            _texture = Textures.GetTexture(_texturePath);
        }

        /// <summary>
        /// Unload the current sprite.
        /// </summary>
        public void UnloadSprite()
        {
            Textures.UnloadTexture(_texturePath);
            _texture = null;
        }

        /// <summary>
        /// Update the sprite.
        /// </summary>
        public abstract void UpdateSprite();

        /// <summary>
        /// Draw the sprite at a given position.
        /// </summary>
        /// <param name="position">The position to draw the sprite at.</param>
        public abstract void DrawSprite(Vector2 position);
    }
}
