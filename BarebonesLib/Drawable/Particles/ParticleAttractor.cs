using Barebones.Config;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Drawable.Particles
{
    /// <summary>
    /// The pattern for the attractor represented by this struct.
    /// </summary>
    public struct AttractorPattern
    {
        [JsonProperty]
        private double? _timeToLive;

        [JsonProperty]
        private Vector2 _origin;

        [JsonProperty]
        private float _attractionForce;

        [JsonProperty]
        private float _innerRadius;

        [JsonProperty]
        private float _outerRadius;

        [JsonProperty]
        private float _falloffExponent;

        /// <summary>
        /// How long in milliseconds should this attractor exist?
        /// null if infinite duration.
        /// </summary>
        [JsonIgnore]
        public double? TimeToLive
        {
            get { return _timeToLive; }
        }

        /// <summary>
        /// Where is this attractor located relative to the particle system?
        /// </summary>
        [JsonIgnore]
        public Vector2 Origin
        {
            get { return _origin; }
        }

        /// <summary>
        /// What is the maximum force the attractor can exert on a particle?
        /// </summary>
        [JsonIgnore]
        public float AttractionForce
        {
            get { return _attractionForce; }
        }

        /// <summary>
        /// The inner radius of the attractor where the maximum force is applied.
        /// </summary>
        [JsonIgnore]
        public float InnerRadius
        {
            get { return _innerRadius; }
        }

        /// <summary>
        /// The outer radius of the attractor, beyond which no force is applied.
        /// </summary>
        [JsonIgnore]
        public float OuterRadius
        {
            get { return _outerRadius; }
        }

        /// <summary>
        /// An exponent to adjust the curve of the force falloff between the inner and outer radi.
        /// </summary>
        [JsonIgnore]
        public float FalloffExponent
        {
            get { return _falloffExponent; }
        }
    }

    /// <summary>
    /// Acts as an attractor for particles, drawing them in.
    /// </summary>
    public class ParticleAttractor
    {
        private double? _timeToLive = null;

        private Vector2 _origin;

        private float _attractionForce;

        private float _innerRadius;

        private float _outerRadius;

        private float _falloffExponent;

        private ParticleSystem _parentSystem;

        /// <summary>
        /// The location of the attractor relative to the particle system.
        /// </summary>
        public Vector2 Position
        {
            get { return _origin; }
        }

        /// <summary>
        /// What is the maximum force the attractor can exert on a particle?
        /// </summary>
        public float AttractionForce
        {
            get { return _attractionForce; }
        }

        /// <summary>
        /// The inner radius of the attractor where the maximum force is applied.
        /// </summary>
        public float InnerRadius
        {
            get { return _innerRadius; }
        }

        /// <summary>
        /// The outer radius of the attractor, beyond which no force is applied.
        /// </summary>
        public float OuterRadius
        {
            get { return _outerRadius; }
        }

        /// <summary>
        /// An exponent to adjust the curve of the force falloff between the inner and outer radi.
        /// </summary>
        public float FalloffExponent
        {
            get { return _falloffExponent; }
        }

        /// <summary>
        /// Construct a new ParticleAttractor from the specified AttractorPattern and with the specified ParticleSystem as its parent.
        /// </summary>
        /// <param name="pattern">The AttractorPattern to use.</param>
        /// <param name="parentSystem">The ParticleSystem that owns this attractor.</param>
        public ParticleAttractor(AttractorPattern pattern, ParticleSystem parentSystem)
        {
            _parentSystem = parentSystem;
            _timeToLive = pattern.TimeToLive;
            _origin = pattern.Origin;
            _attractionForce = pattern.AttractionForce;
            _innerRadius = pattern.InnerRadius;
            _outerRadius = pattern.OuterRadius;
            _falloffExponent = pattern.FalloffExponent;
        }


        internal void Update()
        {
            if (_timeToLive != null)
            {
                _timeToLive -= Engine.GameTime.ElapsedGameTime.TotalMilliseconds;
                if (_timeToLive <= 0)
                {
                    _parentSystem.Attractors.Remove(this);
                }
            }
            
        }
    }
}
