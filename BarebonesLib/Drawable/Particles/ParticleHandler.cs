using Barebones.Asset;
using Barebones.Asset.Scripts;
using Barebones.Interfaces;
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
    /// The handler for all particle related functions.
    /// </summary>
    public static class ParticleHandler
    {
        private readonly static List<ParticleSystem> _particleSystems = new List<ParticleSystem>();

        private readonly static Queue<ParticleSystem> _addParticleSystemQueue = new Queue<ParticleSystem>();

        private readonly static Barrier _particleBarrier = new Barrier(1);

        /// <summary>
        /// A list of all current particle systems.
        /// </summary>
        public static List<ParticleSystem> ParticleSystems
        {
            get { return _particleSystems; }
        }
        
        /// <summary>
        /// A Barrier used to synchronize all particle systems with the main thread so they only tick once alongside the main thread.
        /// </summary>
        public static Barrier ParticleBarrier
        {
            get { return _particleBarrier; }
        }


        /// <summary>
        /// How many particle systems currently exist.
        /// </summary>
        public static int SystemCount
        {
            get { return _particleSystems.Count; }
        }

        /// <summary>
        /// How many particles currently exist.
        /// </summary>
        public static long ParticleCount
        {
            get
            {
                long count = 0;
                foreach (ParticleSystem p in _particleSystems)
                {
                    count += p.Particles.Count;
                }
                return count;
            }
        }

        internal static void Update()
        {
            while (_addParticleSystemQueue.TryDequeue(out ParticleSystem p))
            {
                _particleSystems.Add(p);
                p.Start();
            }
            _particleBarrier.SignalAndWait();
        }

        internal static void AwaitSystems()
        {
            _particleBarrier.SignalAndWait();
        }

        /// <summary>
        /// Add a new particle system with a specified ParticleScript at a specified Position and with specified constant Forces.
        /// </summary>
        /// <param name="scriptPath">The path to the ParticleScript to use.</param>
        /// <param name="position">The position to create the ParticleSystem at.</param>
        /// <param name="forces">The constant forces to apply to particles in the system.</param>
        public static void AddParticleSystem(string scriptPath, Vector2 position, Vector2 forces)
        {
            AddParticleSystem(scriptPath, position, forces, Vector2.One);
        }

        /// <summary>
        /// Add a new particle system with a specified ParticleScript at a specified Position with specified constant Forces, with a multiplier for starting velocities.
        /// </summary>
        /// <param name="scriptPath">The path to the ParticleScript to use.</param>
        /// <param name="position">The position to create the ParticleSystem at.</param>
        /// <param name="forces">The constant forces to apply to particles in the system.</param>
        /// <param name="velocityMultiplier">The multiplier to apply to the starting velocity of all particles in the system.</param>
        public static void AddParticleSystem(string scriptPath, Vector2 position, Vector2 forces, Vector2 velocityMultiplier)
        {
            AddParticleSystem(scriptPath, position, forces, velocityMultiplier, null);
        }

        /// <summary>
        /// Add a new particle system with a specified ParticleScript at a specified Position with specified constant Forces, with a multiplier for starting velocities.
        /// Additionally, monitors a specified ComplexSprite and mimics its animation state and SpriteEffects.
        /// </summary>
        /// <param name="scriptPath">The path to the ParticleScript to use.</param>
        /// <param name="position">The position to create the ParticleSystem at.</param>
        /// <param name="forces">The constant forces to apply to particles in the system.</param>
        /// <param name="velocityMultiplier">The multiplier to apply to the starting velocity of all particles in the system.</param>
        /// <param name="monitorSprite">The complex sprite that this particle system will monitor to match animations. Can be null.</param>
        public static void AddParticleSystem(string scriptPath, Vector2 position, Vector2 forces, Vector2 velocityMultiplier, ComplexSprite? monitorSprite)
        {
            _addParticleSystemQueue.Enqueue(new ParticleSystem(scriptPath, position, forces, velocityMultiplier, monitorSprite));
        }

        /// <summary>
        /// Add a new particle system with a specified ParticleScript bound to a specified ISpatiallyObservable with specified constant Forces, with a multiplier for starting velocities.
        /// Additionally, monitors a specified ComplexSprite and mimics its animation state and SpriteEffects.
        /// </summary>
        /// <param name="scriptPath">The path to the ParticleScript to use.</param>
        /// <param name="monitorPosition">The position to create the ParticleSystem at.</param>
        /// <param name="forces">The constant forces to apply to particles in the system.</param>
        /// <param name="velocityMultiplier">The multiplier to apply to the starting velocity of all particles in the system.</param>
        /// <param name="monitorSprite">The complex sprite that this particle system will monitor to match animations. Can be null.</param>
        public static void AddParticleSystem(string scriptPath, ISpatiallyObservable monitorPosition, Vector2 forces, Vector2 velocityMultiplier, ComplexSprite? monitorSprite)
        {
            _addParticleSystemQueue.Enqueue(new ParticleSystem(scriptPath, monitorPosition, forces, velocityMultiplier, monitorSprite));
        }

        /// <summary>
        /// Draw all particles.
        /// </summary>
        public static void Draw()
        {
            foreach (ParticleSystem p in _particleSystems)
            {
                p.Draw();
            }
        }
    }
}
