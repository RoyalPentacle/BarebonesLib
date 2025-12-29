using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Asset.Scripts
{
    /// <summary>
    /// A script that contains the properties to build a LayeredSprite object.
    /// </summary>
    public class LayeredSpriteScript :  Script
    {
        
        /// <summary>
        /// The pattern for building an AttachPointMonitor specifically for a LayeredSprite.
        /// </summary>
        public struct AttachPointMonitorPattern
        {
            [JsonProperty]
            private int _layer;

            [JsonProperty]
            private string _attachPoint;

            [JsonProperty]
            private bool _inheritRotation;

            /// <summary>
            /// Which layer should we be monitoring?
            /// </summary>
            [JsonIgnore]
            public int Layer
            {
                get { return _layer; }
            }

            /// <summary>
            /// The name of the AttachPoint to monitor.
            /// </summary>
            [JsonIgnore]
            public string AttachPoint
            {
                get { return _attachPoint; }
            }

            /// <summary>
            /// Should we inherit the rotation of the AttachPoint?
            /// </summary>
            [JsonIgnore]
            public bool InheritRotation
            {
                get { return _inheritRotation; }
            }

        }
        
        /// <summary>
        /// A single layer for the LayeredSprite
        /// </summary>
        public struct LayerPattern
        {
            [JsonProperty]
            private string _spriteScript;

            [JsonProperty]
            private AttachPointMonitorPattern? _monitorPattern;

            [JsonProperty]
            private Vector2? _position;

            /// <summary>
            /// The path to the SpriteScript for the ComplexSprite that makes up this layer.
            /// </summary>
            [JsonIgnore]
            public string SpriteScript
            {
                get { return _spriteScript; }
            }

            /// <summary>
            /// The pattern for building the AttachPointMonitor.
            /// Can be null.
            /// </summary>
            [JsonIgnore]
            public AttachPointMonitorPattern? MonitorPattern
            {
                get { return _monitorPattern; }
            }

            /// <summary>
            /// The position relative to the LayeredSprites position that this layer should be offset by.
            /// Can be null.
            /// </summary>
            [JsonIgnore]
            public Vector2? Position
            {
                get { return _position; }
            }
        }

        [JsonProperty]
        private LayerPattern[] _layers;

        /// <summary>
        /// The collection of patterns for building the Layers of the LayeredSprite.
        /// </summary>
        [JsonIgnore]
        public LayerPattern[] Layers
        {
            get { return _layers; }
        }

        /// <summary>
        /// Constructs a default LayeredSpriteScript for fallback purposes.
        /// </summary>
        public LayeredSpriteScript()
        {
            _layers = null;
        }
    }
}
