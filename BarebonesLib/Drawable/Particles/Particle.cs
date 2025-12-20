using Barebones.Config;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Drawable.Particles
{
    
    /// <summary>
    /// Represents the 3 types of particles.
    /// </summary>
    public enum ParticleType :  byte
    {
        /// <summary>
        /// A Point Particle.
        /// </summary>
        Point = 0,
        /// <summary>
        /// A Simple Particle with a SimpleSprite.
        /// </summary>
        Simple = 1,
        /// <summary>
        /// A Complex Particle with a ComplexSprite.
        /// </summary>
        Complex = 2
    }

    /// <summary>
    /// A variety of flags for configuring particle behaviour.
    /// </summary>
    [Flags]
    public enum ParticleFlags
    {
        /// <summary>
        /// Default behaviour.
        /// </summary>
        None = 0,
        /// <summary>
        /// Ignore any forces provided from the parent system.
        /// </summary>
        IgnoreForces = 1,
        /// <summary>
        /// Should this particle expire when it collides with a solid.
        /// </summary>
        CollideWithSolids = 2,
        /// <summary>
        /// Should this particle expire when it collides with a player.
        /// </summary>
        CollideWithPlayer = 4,
        /// <summary>
        /// Should this particle expire when it collides with an entity.
        /// </summary>
        CollideWithEntities = 8,
        /// <summary>
        /// Should this particle expire when it collides with a ParticleClip.
        /// </summary>
        CollideWithParticleClip = 16,
        /// <summary>
        /// Should this particle bounce off any solids it collides with.
        /// </summary>
        BounceOffSolids = 32,
        /// <summary>
        /// Should this particle bounce off any players it collides with.
        /// </summary>
        BounceOffPlayer = 64,
        /// <summary>
        /// Should this particle bounce off any entities it collides with.
        /// </summary>
        BounceOffEntities = 128,
        /// <summary>
        /// Should this particle bounce off any particle clips it collides with.
        /// </summary>
        BounceOffParticleClip = 256,
        /// <summary>
        /// Should this particle shrink as it expires.
        /// </summary>
        Shrink = 512,
        /// <summary>
        /// Should this particle fade away as it expires.
        /// </summary>
        Fade = 1024
    }

    /// <summary>
    /// The abstract base class of all particles.
    /// </summary>
    public abstract class Particle
    {
        private protected Vector2 _position;
        private protected Vector2 _velocity;
        private protected Rectangle _collision;
        private protected float _angularSpeed;
        

        private protected ParticleFlags _flags;

        private protected double _timeToLive;

        private protected ParticleSystem _parentSystem;

        internal Particle(Vector2 position, Vector2 velocity, double lifespan, float rotation, float angularSpeed, ParticleFlags flags, ParticleSystem parentSystem)
        {
            _position = position;
            _velocity = velocity;
            _timeToLive = lifespan;
            _angularSpeed = angularSpeed;
            _flags = flags;
            _parentSystem = parentSystem;
            _collision = new Rectangle((int)_position.X, (int)_position.Y, 0, 0);
        }

        /// <summary>
        /// Update the particle.
        /// </summary>
        public virtual void Update()
        {
            _timeToLive -= Engine.GameTime.ElapsedGameTime.TotalMilliseconds;
            if (_timeToLive <= 0)
            {
                _parentSystem.Particles.Remove(this);
                Unload();
                return;
            }

            // Add attractor code.

            _velocity += CalculateAttractionForce();
            _velocity += _parentSystem.Forces;
            _position += _velocity;
            _collision.X = (int)_position.X;
            _collision.Y = (int)_position.Y;
        }

        internal Vector2 CalculateAttractionForce()
        {
            Vector2 attractorForces = Vector2.Zero;
            foreach (ParticleAttractor attractor in _parentSystem.Attractors)
            {
                Vector2 p2a = (attractor.Position + _parentSystem.Position) - _position;
                float dist = p2a.Length();
                if (dist == 0)
                    return Vector2.Zero;
                
                Vector2 dir = p2a / dist;

                float forceMag;

                if (dist <= attractor.InnerRadius)
                {
                    forceMag = attractor.AttractionForce;
                }
                else if (dist >= attractor.OuterRadius)
                {
                    forceMag = 0f;
                }
                else
                {
                    float t = (dist - attractor.InnerRadius) / (attractor.OuterRadius - attractor.InnerRadius);

                    float falloff = MathF.Pow(1f - t, attractor.FalloffExponent);

                    forceMag = attractor.AttractionForce * falloff;
                }
                attractorForces += (dir * forceMag);
            }
            return attractorForces;
        }

        /// <summary>
        /// Draw the particle.
        /// </summary>
        public abstract void Draw();


        /// <summary>
        /// Unload any assets used by the particle.
        /// </summary>
        public abstract void Unload();

    }
}
