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

        public ComplexParticle(string scriptPath, Vector2 position, Vector2 velocity, double lifespan, Color color, ParticleFlags flags, ParticleSystem parentSystem) : base(position, velocity, lifespan, flags, parentSystem)
        {
            _sprite = new ComplexSprite(scriptPath, true);
            _sprite.SetColour(color);
            if (_parentSystem.MonitorSprite != null)
            {
                _sprite.ChangeAnimation(_parentSystem.MonitorSprite.CurrentAnimationName);
                _sprite.SpriteEffect = _parentSystem.MonitorSprite.SpriteEffect;
            }
            
        }


        public override void Update()
        {
            base.Update();
            _sprite.SpriteEffect = _parentSystem.MonitorSprite.SpriteEffect;
            _sprite.ChangeAnimation(_parentSystem.MonitorSprite.CurrentAnimationName);
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
