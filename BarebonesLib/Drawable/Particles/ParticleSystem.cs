using Barebones.Asset;
using Barebones.Asset.Scripts;
using Barebones.Drawable.Particles;
using Barebones.Interfaces;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Drawable.Particles
{
    /// <summary>
    /// Contains various ParticleGenerators and ParticleAttractors.
    /// </summary>
    public class ParticleSystem
    {
        private Vector2 _position;

        private Thread _thread;

        private List<Particle> _particles;

        private List<ParticleGenerator> _generators;

        private List<ParticleAttractor> _attractors;

        private Random _random;

        private Vector2 _forces;

        private Vector2 _velocityMultiplier;

        private ComplexSprite? _monitorSprite;

        private ISpatiallyObservable _monitorPosition;

        private string? _luaScript;
        
        private bool _luaFinished = false;

        /// <summary>
        /// Constant forces to be applied to all particles in the system.
        /// </summary>
        public Vector2 Forces
        {
            get 
            { 
                return _forces; 
            }
            set
            {
                _forces = value;
            }
        }

        /// <summary>
        /// A random number generator for this particle system.
        /// </summary>
        public Random Random
        {
            get { return _random; }
        }

        /// <summary>
        /// The position of this particle system.
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
        }

        /// <summary>
        /// The multiplier to apply to all starting velocities of particles in this system.
        /// </summary>
        public Vector2 VelocityMultiplier
        {
            get { return _velocityMultiplier; }
        }

        /// <summary>
        /// A list of all particles currently in this system.
        /// </summary>
        public List<Particle> Particles
        {
            get { return _particles; }
        }

        /// <summary>
        /// A list of all generators currently in this system.
        /// </summary>
        public List<ParticleGenerator> Generators
        {
            get { return _generators; }
        }

        /// <summary>
        /// A list of all attractors currently in this system.
        /// </summary>
        public List<ParticleAttractor> Attractors
        {
            get { return _attractors; }
        }

        /// <summary>
        /// The monitored ComplexSprite, if applicable.
        /// Can be null.
        /// </summary>
        public ComplexSprite? MonitorSprite
        {
            get { return _monitorSprite; }
        }

        /// <summary>
        /// Create a new ParticleSystem from the ParticleScript at the specified path, at a given position and with given constant forces.
        /// </summary>
        /// <param name="scriptPath">The path to the ParticleScript.</param>
        /// <param name="position">The position to create the system at.</param>
        /// <param name="forces">The constant forces applied to particles in the system. E.G. gravity</param>
        public ParticleSystem(string scriptPath, Vector2 position, Vector2 forces) : this(scriptPath, position, forces, Vector2.One)
        {
            
        }

        /// <summary>
        /// Create a new ParticleSystem from the ParticleScript at the specified path, at a given position and with given constant forces.
        /// Additionally with a multiplier for the starting velocity of all particles in the system.
        /// </summary>
        /// <param name="scriptPath">The path to the ParticleScript.</param>
        /// <param name="position">The position to create the system at.</param>
        /// <param name="forces">The constant forces applied to particles in the system. E.G. gravity</param>
        /// <param name="velocityMultiplier">The multiplier for the starting velocity of all particles in the system.</param>
        public ParticleSystem(string scriptPath, Vector2 position, Vector2 forces, Vector2 velocityMultiplier) : this(scriptPath, position, forces, velocityMultiplier, null)
        {

        }

        /// <summary>
        /// Create a new ParticleSystem from the ParticleScript at the specified path, at a given position and with given constant forces.
        /// Additionally with a multiplier for the starting velocity of all particles in the system.
        /// Further, with a pointer to a ComplexSprite that can be monitored so it can be mimic'd by the particle system.
        /// </summary>
        /// <param name="scriptPath">The path to the ParticleScript.</param>
        /// <param name="position">The position to create the system at.</param>
        /// <param name="forces">The constant forces applied to particles in the system. E.G. gravity</param>
        /// <param name="velocityMultiplier">The multiplier for the starting velocity of all particles in the system.</param>
        /// <param name="monitorSprite">The ComplexSprite to monitor.</param>
        public ParticleSystem(string scriptPath, Vector2 position, Vector2 forces, Vector2 velocityMultiplier, ComplexSprite? monitorSprite)
        {
            ParticleScript script = ScriptFinder.FindScript<ParticleScript>(scriptPath);
            _position = position;
            _forces = forces;
            _velocityMultiplier = velocityMultiplier;
            _random = new Random();
            _luaScript = script.LuaScript;
            BuildLists(script);
            _particles = new List<Particle>();
            _thread = new Thread(Update);
            _monitorSprite = monitorSprite;
        }

        /// <summary>
        /// Create a new ParticleSystem from the ParticleScript at the specified path, bound to a specified ISpatiallyObservable and with given constant forces.
        /// Additionally with a multiplier for the starting velocity of all particles in the system.
        /// Further, with a pointer to a ComplexSprite that can be monitored so it can be mimic'd by the particle system.
        /// </summary>
        /// <param name="scriptPath">The path to the ParticleScript.</param>
        /// <param name="monitorPosition">The ISpatiallyObservable to bind to.</param>
        /// <param name="forces">The constant forces applied to particles in the system. E.G. gravity</param>
        /// <param name="velocityMultiplier">The multiplier for the starting velocity of all particles in the system.</param>
        /// <param name="monitorSprite">The ComplexSprite to monitor.</param>
        public ParticleSystem(string scriptPath, ISpatiallyObservable monitorPosition, Vector2 forces, Vector2 velocityMultiplier, ComplexSprite? monitorSprite)
        {
            ParticleScript script = ScriptFinder.FindScript<ParticleScript>(scriptPath);
            _position = monitorPosition.Position;
            _monitorPosition = monitorPosition;
            _forces = forces;
            _velocityMultiplier = velocityMultiplier;
            _random = new Random();
            _luaScript = script.LuaScript;
            BuildLists(script);
            _particles = new List<Particle>();
            _thread = new Thread(Update);
            _monitorSprite = monitorSprite;
        }

        private void BuildLists(ParticleScript script)
        {
            _generators = new List<ParticleGenerator>();
            _attractors = new List<ParticleAttractor>();
            if (script.Generators != null)
            {
                foreach (GeneratorPattern pattern in script.Generators)
                {
                    _generators.Add(new ParticleGenerator(pattern, this));
                }
            }
            if (script.Attractors != null)
            {
                foreach (AttractorPattern pattern in script.Attractors)
                {
                    _attractors.Add(new ParticleAttractor(pattern, this));
                }
            }
        }

        internal void AddParticle(Particle p)
        {
            _particles.Add(p);
        }

        internal void Start()
        {
            ParticleHandler.ParticleBarrier.AddParticipant();
            _thread.Start();
            
        }

        private void Update()
        {
            while (true)
            {
                ParticleHandler.ParticleBarrier.SignalAndWait();
                if (_monitorPosition != null)
                    _position = _monitorPosition.Position;
                if (_particles.Count == 0 && _generators.Count == 0)
                {
                    ParticleHandler.ParticleSystems.Remove(this);
                    ParticleHandler.ParticleBarrier.RemoveParticipant();
                    return;
                }
                for (int i = _particles.Count - 1; i >= 0; i--)
                {
                    _particles[i].Update();
                }
                for (int i = _generators.Count - 1; i >= 0; i--)
                {
                    _generators[i].Update();
                }
                for (int i = _attractors.Count - 1; i >= 0; i--)
                {
                    _attractors[i].Update();
                }
                ParticleHandler.ParticleBarrier.SignalAndWait();
            }
        }


        /// <summary>
        /// Draw all particles in this system.
        /// </summary>
        public void Draw()
        {
            foreach (Particle p in _particles)
            {
                p.Draw();
            }
        }
    }
}
