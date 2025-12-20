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
    /// The pattern for the generator represented by this struct.
    /// </summary>
    public struct GeneratorPattern
    {
        [JsonProperty]
        private int _totalSpawns;

        [JsonProperty]
        private double _spawnDelay;

        [JsonProperty]
        private int _spawnAmount;

        [JsonProperty]
        private Vector2 _origin;

        [JsonProperty]
        private Vector4 _spawnRange;

        [JsonProperty]
        private double _spawnBias;

        [JsonProperty]
        private Vector2 _baseVelocity;

        [JsonProperty]
        private Vector4 _velocityRange;

        [JsonProperty]
        private double _velocityBias;

        [JsonProperty]
        private double _lifespan;

        [JsonProperty]
        private Vector2 _lifespanRange;

        [JsonProperty]
        private double _lifespanBias;

        [JsonProperty]
        private float _rotation;

        [JsonProperty]
        private Vector2 _rotationRange;

        [JsonProperty]
        private double _rotationBias;

        [JsonProperty]
        private float _angularSpeed;

        [JsonProperty]
        private Vector2 _angularSpeedRange;

        [JsonProperty]
        private double _angularSpeedBias;

        [JsonProperty]
        private Vector2 _scaleBase;

        [JsonProperty]
        private Vector4 _scaleRange;

        [JsonProperty]
        private double _scaleBias;

        [JsonProperty]
        private float _depthBase;

        [JsonProperty]
        private Vector2 _depthRange;

        [JsonProperty]
        private double _depthBias;

        [JsonProperty]
        private Color _baseColor;

        [JsonProperty]
        private Color _minColor;

        [JsonProperty] 
        private Color _maxColor;

        [JsonProperty]
        private double _colorBias;

        [JsonProperty]
        private bool _randomFrame;

        [JsonProperty]
        private bool _fullRandomPos;

        [JsonProperty]
        private ParticleType _type;

        [JsonProperty]
        private ParticleFlags _flags;

        [JsonProperty]
        private string? _spritePath;

       

        /// <summary>
        /// The total number of spawn cycles this generator is allowed.
        /// -1 is infinite.
        /// </summary>
        [JsonIgnore]
        public int TotalSpawns
        {
            get { return _totalSpawns; }
        }

        /// <summary>
        /// The time in milliseconds between spawns.
        /// </summary>
        [JsonIgnore]
        public double SpawnDelay
        {
            get { return _spawnDelay; }
        }

        /// <summary>
        /// The amount of particles that should be spawned every cycle.
        /// </summary>
        [JsonIgnore]
        public int SpawnAmount
        {
            get { return _spawnAmount; }
        }

        /// <summary>
        /// The position of the generator relative to the parent system.
        /// </summary>
        [JsonIgnore]
        public Vector2 Origin
        {
            get { return _origin; }
        }

        /// <summary>
        /// The possible variation of where a particle can spawn.
        /// X/Y Min/Max variation on the X axis.
        /// Z/W Min/Max variation on the Y axis.
        /// </summary>
        [JsonIgnore]
        public Vector4 SpawnRange
        {
            get { return _spawnRange; }
        }

        /// <summary>
        /// The bias of the random distribution of the spawn range.
        /// </summary>
        [JsonIgnore]
        public double SpawnBias
        {
            get { return _spawnBias; }
        }

        /// <summary>
        /// The base velocity for every particle spawned.
        /// </summary>
        [JsonIgnore]
        public Vector2 BaseVelocity
        {
            get { return _baseVelocity; }
        }

        /// <summary>
        /// The possible variation of a particles velocity.
        /// X/Y Min/Max variation on the X axis.
        /// Z/W Min/Max variation on the Y axis.
        /// </summary>
        [JsonIgnore]
        public Vector4 VelocityRange
        {
            get { return _velocityRange; }
        }

        /// <summary>
        /// The bias of the random distribution of the velocity range.
        /// </summary>
        [JsonIgnore]
        public double VelocityBias
        {
            get { return _velocityBias; }
        }

        /// <summary>
        /// The base lifespan, in milliseconds, of every particle spawned.
        /// </summary>
        [JsonIgnore]
        public double Lifespan
        {
            get { return _lifespan; }
        }

        /// <summary>
        /// The possible variation of a particles lifespawn.
        /// X/Y Min/Max variation.
        /// </summary>
        [JsonIgnore]
        public Vector2 LifespanRange
        {
            get { return _lifespanRange; }
        }

        /// <summary>
        /// The bias of the random distribution of the lifespawn range.
        /// </summary>
        [JsonIgnore]
        public double LifespanBias
        {
            get { return _lifespanBias; }
        }


        [JsonIgnore]
        public float BaseRotation
        {
            get { return _rotation; }
        }

        [JsonIgnore]
        public Vector2 RotationRange
        {
            get { return _rotationRange; }
        }

        [JsonIgnore]
        public double RotationBias
        {
            get { return _rotationBias; }
        }

        [JsonIgnore]
        public float AngularSpeed
        {
            get { return _angularSpeed; }
        }

        [JsonIgnore]
        public Vector2 AngularSpeedRange
        {
            get { return _angularSpeedRange; }
        }

        [JsonIgnore]
        public double AngularSpeedBias
        {
            get { return _angularSpeedBias; }
        }

        [JsonIgnore]
        public Vector2 ScaleBase
        {
            get { return _scaleBase; }
        }

        [JsonIgnore]
        public Vector4 ScaleRange
        {
            get { return _scaleRange; }
        }

        [JsonIgnore]
        public double ScaleBias
        {
            get { return _scaleBias; }
        }

        [JsonIgnore]
        public float DepthBase
        {
            get { return _depthBase; }
        }

        [JsonIgnore]
        public Vector2 DepthRange
        {
            get { return _depthRange; }
        }

        [JsonIgnore]
        public double DepthBias
        {
            get { return _depthBias; }
        }

        /// <summary>
        /// The base colour of every particle spawned.
        /// </summary>
        [JsonIgnore]
        public Color BaseColor
        {
            get { return _baseColor; }
        }

        /// <summary>
        /// The maximum the base colour can be reduced.
        /// </summary>
        [JsonIgnore]
        public Color MinColor
        {
            get { return _minColor; }
        }

        /// <summary>
        /// The maximum the base colour can be increased.
        /// </summary>
        [JsonIgnore]
        public Color MaxColor
        {
            get { return _maxColor; }
        }

        /// <summary>
        /// The bias of the random distribution for the colour range.
        /// </summary>
        [JsonIgnore]
        public double ColorBias
        {
            get { return _colorBias; }
        }

        [JsonIgnore]
        public bool RandomFrame
        {
            get { return _randomFrame; }
        }

        [JsonIgnore]
        public bool FullRandomPos
        {
            get { return _fullRandomPos; }
        }

        /// <summary>
        /// The type of particle.
        /// </summary>
        [JsonIgnore]
        public ParticleType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Flags defining particle behaviours.
        /// </summary>
        [JsonIgnore]
        public ParticleFlags Flags
        {
            get { return _flags; }
        }

        /// <summary>
        /// The path to the script for the sprite the particle will use.
        /// Ignored if the particle type is a Point.
        /// </summary>
        [JsonIgnore]
        public string? SpritePath
        {
            get { return _spritePath; }
        }
    }

    /// <summary>
    /// Generates particles after a delay.
    /// </summary>
    public class ParticleGenerator
    {
        private int _remainingSpawns = -1;

        private double _currentTime;

        private double _spawnDelay;

        private int _spawnAmount;

        private Vector2 _origin;

        private Vector4 _spawnRange;

        private double _spawnBias = 0f;

        private Vector2 _baseVelocity;

        private Vector4 _velocityRange;

        private double _velocityBias;

        private double _lifespan;

        private Vector2 _lifespanRange;

        private double _lifespanBias;

        private float _baseRotation;

        private Vector2 _rotationRange;

        private double _rotationBias;

        private float _angularSpeed;

        private Vector2 _angularSpeedRange;

        private double _angularSpeedBias;

        private Vector2 _scaleBase;

        private Vector4 _scaleRange;

        private double _scaleBias;

        private float _depthBase;

        private Vector2 _depthRange;

        private double _depthBias;

        private Color _baseColor;

        private Color _minColor;

        private Color _maxColor;

        private double _colorBias;

        private bool _randomFrame;

        private bool _fullRandomPos;

        private ParticleType _type;

        private ParticleFlags _flags;   

        private string? _spritePath;

        private ParticleSystem _parentSystem;

        private Random _parentRandom;

        /// <summary>
        /// Constructs a new ParticleGenerator from a specified GeneratorPattern and with a specified ParticleSystem as its parent.
        /// </summary>
        /// <param name="pattern">The GeneratorPattern to use.</param>
        /// <param name="parentSystem">The ParticleSystem that owns this generator.</param>
        public ParticleGenerator(GeneratorPattern pattern, ParticleSystem parentSystem)
        {
            _parentSystem = parentSystem;
            _parentRandom = parentSystem.Random;
            _remainingSpawns = pattern.TotalSpawns;
            _spawnDelay = pattern.SpawnDelay;
            _spawnAmount = pattern.SpawnAmount;
            _origin = pattern.Origin;
            _spawnRange = pattern.SpawnRange;
            _spawnBias = Math.Abs(pattern.SpawnBias);
            _baseVelocity = pattern.BaseVelocity;
            _velocityRange = pattern.VelocityRange;
            _velocityBias = Math.Abs(pattern.VelocityBias);
            _lifespan = pattern.Lifespan;
            _lifespanRange = pattern.LifespanRange;
            _lifespanBias = Math.Abs(pattern.LifespanBias);
            _baseRotation = pattern.BaseRotation;
            _rotationRange = pattern.RotationRange;
            _rotationBias = Math.Abs(pattern.RotationBias);
            _angularSpeed = pattern.AngularSpeed;
            _angularSpeedRange = pattern.AngularSpeedRange;
            _angularSpeedBias = Math.Abs(pattern.AngularSpeedBias);
            _scaleBase = pattern.ScaleBase;
            _scaleRange = pattern.ScaleRange;
            _scaleBias = Math.Abs(pattern.ScaleBias);
            _depthBase = pattern.DepthBase;
            _depthRange = pattern.DepthRange;
            _depthBias = Math.Abs(pattern.DepthBias);
            _baseColor = pattern.BaseColor;
            _minColor = pattern.MinColor;
            _maxColor = pattern.MaxColor;
            _colorBias = Math.Abs(pattern.ColorBias);
            _randomFrame = pattern.RandomFrame;
            _fullRandomPos = pattern.FullRandomPos;
            _type = pattern.Type;
            _flags = pattern.Flags;
            _spritePath = pattern.SpritePath;
        }

        private Vector2 GenerateBiasedVector2(Vector4 range, double bias)
        {
            // Get the center of the horizontal and vertical ranges
            double cHori = 0.5 * (range.X + range.Y);
            double cVert = 0.5 * (range.Z + range.W);

            // Get half the length of the horizontal and vertical ranges
            double hHori = 0.5 * (range.Y - range.X);
            double hVert = 0.5 * (range.W - range.Z);

            // Get a random position on the edge of a circle, in radians.
            double theta = _parentRandom.NextDouble() * Math.PI * 2.0;

            // Get the cosine and sine of that random radian.
            double cos = Math.Cos(theta);
            double sin = Math.Sin(theta);
            double x = cHori;
            double y = cVert;
            if (!_fullRandomPos)
            {
                double rMaxHori = Math.Abs(cos) < 1e-12 ? double.PositiveInfinity : hHori / Math.Abs(cos);
                double rMaxVert = Math.Abs(sin) < 1e-12 ? double.PositiveInfinity : hVert / Math.Abs(sin);
                double rMax = Math.Min(rMaxHori, rMaxVert);

                double r = rMax * Math.Pow(_parentRandom.NextDouble(), bias);


                x = cHori + r * cos;
                y = cVert + r * sin;
            }
            else
            {
                x += hHori * ((_parentRandom.NextDouble() * 2.0) - 1.0);
                y += hVert * ((_parentRandom.NextDouble() * 2.0) - 1.0);
            }
            return new Vector2((float)x, (float)y);  
        }

        private double GenerateBiasedValue(Vector2 range, double bias)
        {
            double u = _parentRandom.NextDouble();
            double t = 2.0 * u - 1.0;

            double b = Math.Sign(t) * Math.Pow(Math.Abs(t), bias);
            double norm = 0.5 * (b + 1.0);
            double result = range.X + (norm * (range.Y - range.X));
            return result;
        }



        private Vector2 GenerateNewParticlePosition()
        {
            //double bias = Math.Pow(_parentRandom.NextDouble(), _spawnBias);
            Vector2 position = GenerateBiasedVector2(_spawnRange, _spawnBias);
            position += _origin;
            return position;
        }

        private Vector2 GenerateNewParticleVelocity()
        {
            //double bias = Math.Pow(_parentRandom.NextDouble(), _velocityBias);
            Vector2 velocity = GenerateBiasedVector2(_velocityRange, _velocityBias);
            velocity += _baseVelocity;
            velocity *= _parentSystem.VelocityMultiplier;
            return velocity;
        }

        private double GenerateNewParticleLifespan()
        {
            //double bias = Math.Pow(_parentRandom.NextDouble(), _lifespanBias);
            double lifespan = GenerateBiasedValue(_lifespanRange, _lifespanBias);
            //lifespan *= bias;
            lifespan += _lifespan;
            return lifespan;
        }

        private float GenerateNewParticleRotation()
        {
            float rotation = (float)GenerateBiasedValue(_rotationRange, _rotationBias);

            rotation += _baseRotation;
            return rotation;
        }

        private float GenerateNewParticleAngularSpeed()
        {
            float angularSpeed = (float)GenerateBiasedValue(_angularSpeedRange, _angularSpeedBias);
            angularSpeed += _angularSpeed;
            return angularSpeed;
        }

        private Vector2 GenerateNewParticleScale()
        {
            Vector2 scale = GenerateBiasedVector2(_scaleRange, _scaleBias);
            scale += _scaleBase;
            return scale;
        }

        private float GenerateNewParticleDepth()
        {
            float depth = (float)GenerateBiasedValue(_depthRange, _depthBias);
            depth += _depthBase;
            return depth;
        }

        private Color GenerateNewParticleColor()
        {
            int r = (int)GenerateBiasedValue(new Vector2(-_minColor.R, _maxColor.R), _colorBias);
            int g = (int)GenerateBiasedValue(new Vector2(-_minColor.G, _maxColor.G), _colorBias);
            int b = (int)GenerateBiasedValue(new Vector2(-_minColor.B, _maxColor.B), _colorBias);
            int a = (int)GenerateBiasedValue(new Vector2(-_minColor.A, _maxColor.A), _colorBias);
            r += _baseColor.R;
            g += _baseColor.G;
            b += _baseColor.B;
            a += _baseColor.A;
            r = Math.Clamp(r, 0, 255);
            g = Math.Clamp(g, 0, 255);
            b = Math.Clamp(b, 0, 255);
            a = Math.Clamp(a, 0, 255);
            return new Color(r, g, b, a);
        }

        private void CreateParticle()
        {
            if (_type == ParticleType.Point)
            {
                PointParticle newParticle = new PointParticle(GenerateNewParticlePosition() + _parentSystem.Position, GenerateNewParticleVelocity(), GenerateNewParticleLifespan(), GenerateNewParticleRotation(), GenerateNewParticleAngularSpeed(), GenerateNewParticleScale(), GenerateNewParticleDepth(), GenerateNewParticleColor(), _flags, _parentSystem);
                _parentSystem.AddParticle(newParticle);
            }
            else if (_type == ParticleType.Simple)
            {
                SimpleParticle newParticle = new SimpleParticle(_spritePath, GenerateNewParticlePosition() + _parentSystem.Position, GenerateNewParticleVelocity(), GenerateNewParticleLifespan(), GenerateNewParticleRotation(), GenerateNewParticleAngularSpeed(), GenerateNewParticleScale(), GenerateNewParticleDepth(), GenerateNewParticleColor(), _flags, _parentSystem);
                _parentSystem.AddParticle(newParticle);
            }
            else if (_type == ParticleType.Complex)
            {
                ComplexParticle newParticle = new ComplexParticle(_spritePath, GenerateNewParticlePosition() + _parentSystem.Position, GenerateNewParticleVelocity(), GenerateNewParticleLifespan(), GenerateNewParticleRotation(), GenerateNewParticleAngularSpeed(), GenerateNewParticleScale(), GenerateNewParticleDepth(), GenerateNewParticleColor(), _randomFrame, _flags, _parentSystem);
                _parentSystem.AddParticle(newParticle);
            }
        }

        /// <summary>
        /// Updates the particle generator.
        /// Only call this if you have manually created this particle generator outside of the ParticleHandler.
        /// </summary>
        public void Update()
        {
            _currentTime += Engine.GameTime.ElapsedGameTime.TotalMilliseconds;
            if (_currentTime > _spawnDelay)
            {
                _currentTime -= _spawnDelay;
                int amount = (int)Math.Ceiling(_spawnAmount * Engine.ParticleMultiplier);
                for (int i = 0; i < amount; i++)
                {
                    CreateParticle();
                }
                if (_remainingSpawns > 0)
                {
                    _remainingSpawns--;
                }
                if (_remainingSpawns == 0)
                {
                    _parentSystem.Generators.Remove(this);
                }
            }
            
        }

    }
}
