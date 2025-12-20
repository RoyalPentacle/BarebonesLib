using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Drawable.Particles
{
    internal class SimpleParticle : Particle
    {
        private SimpleSprite _sprite;

        public SimpleParticle(string scriptPath, Vector2 position, Vector2 velocity, double lifespan, float rotation, float angularSpeed, Vector2 scale, float depth, Color color, ParticleFlags flags, ParticleSystem parentSystem) : base(position, velocity, lifespan, rotation, angularSpeed, flags, parentSystem)
        {
            _sprite = new SimpleSprite(scriptPath, out _);
            _sprite.SetColour(color);
            _sprite.SetScale(scale);
            _sprite.Rotation = rotation;
            _sprite.SpriteDepth = depth;
        }

        public override void Update()
        {
            base.Update();
            _sprite.UpdateSprite();
            _sprite.Rotation += _angularSpeed;
        }
        public override void Draw()
        {
            _sprite.DrawSprite(_position);
        }

        public override void Unload()
        {
            _sprite.UnloadSprite();
        }
    }
}
