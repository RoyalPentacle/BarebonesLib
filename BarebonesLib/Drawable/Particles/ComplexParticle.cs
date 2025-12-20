using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Drawable.Particles
{
    internal class ComplexParticle : Particle
    {
        private ComplexSprite _sprite;

        public ComplexParticle(string scriptPath, Vector2 position, Vector2 velocity, double lifespan, float rotation, float angularSpeed, Vector2 scale, float depth, Color color, bool randomFrame, ParticleFlags flags, ParticleSystem parentSystem) : base(position, velocity, lifespan, rotation, angularSpeed, flags, parentSystem)
        {
            _sprite = new ComplexSprite(scriptPath, true);
            _sprite.SetColour(color);
            _sprite.Rotation = rotation;
            _sprite.SetScale(scale);
            _sprite.SpriteDepth = depth;
            if (_parentSystem.MonitorSprite != null)
            {
                _sprite.ChangeAnimation(_parentSystem.MonitorSprite.CurrentAnimationName);
                _sprite.SpriteEffect = _parentSystem.MonitorSprite.SpriteEffect;
            }
            if (randomFrame)
                _sprite.ChangeFrame(Random.Shared.Next(_sprite.CurrentAnimation.Frames.Count), false);
        }


        public override void Update()
        {
            base.Update();
            if (_parentSystem.MonitorSprite != null)
            {
                _sprite.SpriteEffect = _parentSystem.MonitorSprite.SpriteEffect;
                _sprite.ChangeAnimation(_parentSystem.MonitorSprite.CurrentAnimationName);
            }
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
