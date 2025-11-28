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

        public SimpleParticle(string scriptPath, Vector2 position, Vector2 velocity, double lifespan, Color color, ParticleFlags flags, ParticleSystem parentSystem) : base(position, velocity, lifespan, flags, parentSystem)
        {
            _sprite = new SimpleSprite(scriptPath, out _);
            _sprite.SetColour(color);
        }

        public override void Update()
        {
            base.Update();
            _sprite.UpdateSprite();
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
