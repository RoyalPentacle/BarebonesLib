using Barebones.Asset;
using Barebones.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Drawable.Particles
{
    internal class PointParticle : Particle
    {
        private Texture2D _texture;
        private Color _color;
        public PointParticle(Vector2 position, Vector2 velocity, double lifespan, Color color, ParticleFlags flags, ParticleSystem parentSystem) : base(position, velocity, lifespan, flags, parentSystem)
        {
            _texture = Textures.Shared.Pixel;
            _color = color;
        }


        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            Engine.SpriteBatch.Draw(_texture, _position, _color);
        }

        public override void Unload()
        {
            
        }
    }
}
