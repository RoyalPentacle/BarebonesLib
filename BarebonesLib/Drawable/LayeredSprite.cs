using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Barebones;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Design;
using System.ComponentModel;
using Barebones.Asset.Scripts;
using Barebones.Asset;
using Barebones.Drawable.Particles;

namespace Barebones.Drawable
{
    /// <summary>
    /// A single sprite object that supports layering of multiple ComplexSprites
    /// </summary>
    public class LayeredSprite
    {
        /// <summary>
        /// A single layer of the LayeredSprite
        /// </summary>
        public class Layer
        {
            private ComplexSprite _sprite;

            private float _rotation;

            private float _angularSpeed;

            private AttachPointMonitor? _attachPointMonitor;

            private Vector2 _offset;

            private LayeredSprite _parent;

            /// <summary>
            /// The ComplexSprite for this layer.
            /// </summary>
            public ComplexSprite Sprite
            {
                get { return _sprite; }
            }

            /// <summary>
            /// Construct a new Layer from the specified arguments.
            /// </summary>
            /// <param name="pattern">The LayerPattern to build this layer from/</param>
            /// <param name="depth">The SpriteDepth for this layer.</param>
            /// <param name="parent">The LayeredSprite that owns this layer.</param>
            public Layer(LayeredSpriteScript.LayerPattern pattern, float depth, LayeredSprite parent)
            {
                _parent = parent;
                _sprite = new ComplexSprite(pattern.SpriteScript);
                _sprite.SpriteDepth = depth;
                _rotation = pattern.Rotation;
                _angularSpeed = pattern.AngularSpeed;
                if (pattern.MonitorPattern.HasValue)
                    _attachPointMonitor = new AttachPointMonitor(_parent.Layers[pattern.MonitorPattern.Value.Layer].Sprite, pattern.MonitorPattern.Value.AttachPoint, pattern.MonitorPattern.Value.InheritRotation);
                else if (pattern.Position != null)
                {
                    _offset = (Vector2)pattern.Position;
                    return;
                }
                _offset = Vector2.Zero;

            }

            /// <summary>
            /// Update this layer.
            /// </summary>
            public void Update()
            {
                _rotation += _angularSpeed;
                _sprite.Rotation = _rotation;
                if (_attachPointMonitor != null)
                    _sprite.Rotation += _attachPointMonitor.Rotation;
                _sprite.Update();
            }

            /// <summary>
            /// Draw this layer, at the specified position.
            /// </summary>
            /// <remarks>
            /// Ignores position if this layer has an <see cref="AttachPointMonitor"/> specified.
            /// </remarks>
            /// <param name="position">The position to draw the layer at.</param>
            public void Draw(Vector2 position)
            {
                if (_attachPointMonitor != null)
                    _sprite.Draw(_attachPointMonitor.Position);
                else
                    _sprite.Draw(position + _offset);
            }
        }

        private Layer[] _layers;

        /// <summary>
        /// The collection of <see cref="Layer"/> that makes up this LayeredSprite
        /// </summary>
        public Layer[] Layers
        {
            get { return _layers; }
        }

        /// <summary>
        /// Constructs a new LayeredSprite from the specified script and at the specified SpriteDepth.
        /// </summary>
        /// <remarks>
        /// Each layer is offset by 0.0001f from the previous layer in the LayeredSprite, to ensure the correct ordering.
        /// </remarks>
        /// <param name="scriptPath">The path to the LayeredSpriteScript to load.</param>
        /// <param name="depth">The SpriteDepth of this LayeredSprite</param>
        public LayeredSprite(string scriptPath, float depth)
        {
            LayeredSpriteScript script = ScriptFinder.FindScript<LayeredSpriteScript>(scriptPath);
            if (script.Layers != null && script.Layers.Length > 0)
            {
                _layers = new Layer[script.Layers.Length];
                float depthAdd = 0f;
                for (int i = 0; i <  script.Layers.Length; i++)
                { 
                    _layers[i] = new Layer(script.Layers[i], depth + depthAdd, this);
                    depthAdd += 0.0001f;
                }
            }
        }

        /// <summary>
        /// Updates this LayeredSprite, and all layers within.
        /// </summary>
        public void Update()
        {
            foreach (Layer layer in Layers)
            {
                layer.Update();
            }
        }

        /// <summary>
        /// Draws this LayeredSprite at the specified position.
        /// </summary>
        /// <param name="position">The position to draw this sprite at.</param>
        public void Draw(Vector2 position)
        {
            foreach(Layer layer in Layers)
            {
                layer.Draw(position);
            }
        }
    }
}