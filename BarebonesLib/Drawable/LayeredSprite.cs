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

            private AttachPointMonitor? _attachPointMonitor;

            private Vector2 _offset;

            private LayeredSprite _parent;

            public ComplexSprite Sprite
            {
                get { return _sprite; }
            }

            // TODO: LUA THE WHOLE THING????

            public Layer(LayeredSpriteScript.LayerPattern pattern, float depth, LayeredSprite parent)
            {
                _parent = parent;
                _sprite = new ComplexSprite(pattern.SpriteScript);
                _sprite.SpriteDepth = depth;
                if (pattern.MonitorPattern.HasValue)
                    _attachPointMonitor = new AttachPointMonitor(_parent.Layers[pattern.MonitorPattern.Value.Layer].Sprite, pattern.MonitorPattern.Value.AttachPoint, pattern.MonitorPattern.Value.InheritRotation);
                else if (pattern.Position != null)
                {
                    _offset = (Vector2)pattern.Position;
                    return;
                }
                _offset = Vector2.Zero;

            }

            public void Update()
            {
                _rotation += 0.003f; // testing rotation. expose to some other method, lua most likely.
                _sprite.Rotation = _rotation;
                if (_attachPointMonitor != null && _attachPointMonitor.InheritRotation)
                    _sprite.Rotation += _attachPointMonitor.Rotation;
                _sprite.UpdateSprite();
            }

            public void Draw(Vector2 position)
            {
                if (_attachPointMonitor != null)
                    _sprite.DrawSprite(_attachPointMonitor.Position);
                else
                    _sprite.DrawSprite(position + _offset);
            }
        }

        private Layer[] _layers;

        public Layer[] Layers
        {
            get { return _layers; }
        }

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
                    if (i >= 5)
                    {
                        ParticleHandler.AddParticleSystem("scripts/particles/test_thruster.pts", new AttachPointMonitor(_layers[i].Sprite, "end", true), Vector2.Zero, Vector2.One, null);
                    }
                }
            }
        }

        public void Update()
        {
            foreach (Layer layer in Layers)
            {
                layer.Update();
            }
        }

        public void Draw(Vector2 position)
        {
            foreach(Layer layer in Layers)
            {
                layer.Draw(position);
            }
        }

    }
}
