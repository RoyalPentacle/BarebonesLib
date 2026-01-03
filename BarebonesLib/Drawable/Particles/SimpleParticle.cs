using Microsoft.Xna.Framework;

namespace Barebones.Drawable.Particles
{
    internal class SimpleParticle : Particle
    {
        private SimpleSprite _sprite;

        public SimpleParticle(string scriptPath, Vector2 position, Vector2 velocity, double lifespan, float rotation, float angularSpeed, Vector2 scale, float depth, Color color, ParticleFlags flags, ParticleSystem parentSystem) : base(position, velocity, lifespan, rotation, angularSpeed, flags, parentSystem)
        {
            _sprite = new SimpleSprite(scriptPath);
            _sprite.SetColour(color);
            _sprite.SetScale(scale);
            _sprite.Rotation = rotation;
            _sprite.SpriteDepth = depth;
        }

        public override void Update()
        {
            base.Update();
            _sprite.Update();
            _sprite.Rotation += _angularSpeed;
        }
        public override void Draw()
        {
            _sprite.Draw(_position);
        }

        public override void Unload()
        {
            _sprite.UnloadSprite();
        }
    }
}
