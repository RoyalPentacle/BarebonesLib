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
        private float _scale;
        private Rectangle _cullRec;
        private float _rotation;
        private float _depth;

        public PointParticle(Vector2 position, Vector2 velocity, double lifespan, float rotation, float angularSpeed, Vector2 scale, float depth, Color color, ParticleFlags flags, ParticleSystem parentSystem) : base(position, velocity, lifespan, rotation, angularSpeed, flags, parentSystem)
        {
            _texture = Textures.Shared.Pixel;
            _color = color;
            _rotation = rotation;
            _scale = scale.X;
            _depth = depth;
            _cullRec = new Rectangle((int)position.X, (int)position.Y, (int)(_texture.Width * scale.X + 1), (int)(_texture.Height * scale.X + 1));
        }


        public override void Update()
        {
            base.Update();
            _rotation += _angularSpeed;
            _cullRec.X = (int)_position.X - _cullRec.Width / 2;
            _cullRec.Y = (int)_position.Y - _cullRec.Height / 2;
        }

        public override void Draw()
        {
            if (_cullRec.Intersects(Engine.Camera.VisibleArea))
                Engine.SpriteBatch.Draw(_texture, _position, null, _color, _rotation, Vector2.One, _scale, SpriteEffects.None, _depth);
        }

        public override void Unload()
        {
            
        }
    }
}
