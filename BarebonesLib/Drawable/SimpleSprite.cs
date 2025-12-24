using Barebones.Asset;
using Barebones.Asset.Scripts;
using Barebones.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Drawable
{
    /// <summary>
    /// A simple sprite object that offer colourization and scaling functions, but no animation.
    /// </summary>
    public class SimpleSprite : BaseSprite
    {
        private protected struct Scale
        {
            // The scale of the width of the sprite.
            private Vector2 _scale;

            /// <summary>
            /// The scale of the width of the sprite.
            /// </summary>
            public float Width
            {
                get { return _scale.X; }
                set { _scale.X = value; }
            }

            /// <summary>
            /// The scale of the height of the sprite.
            /// </summary>
            public float Height
            {
                get { return _scale.Y; }
                set { _scale.Y = value; }
            }

            public Vector2 RawVector2
            {
                get { return _scale; }
            }

            /// <summary>
            /// Default constructor.
            /// </summary>
            public Scale()
            {
                Width = 1f;
                Height = 1f;
            }

            /// <summary>
            /// Gets the change per tick required to go from the starting scale to the destination scale over the duration.
            /// </summary>
            /// <param name="start">The starting scale.</param>
            /// <param name="dest">The destination scale.</param>
            /// <param name="time">The duration in milliseconds.</param>
            /// <returns></returns>
            public static Scale GetChangeOverTime(Scale start, Scale dest, float time)
            {
                Scale overTime = new Scale();
                float delta = 1000f / time / 60;
                overTime.Width = (dest.Width - start.Width) * delta;
                overTime.Height = (dest.Height - start.Height) * delta;
                return overTime;
            }
            /// <summary>
            /// Adds the two scales together.
            /// </summary>
            /// <param name="a">The First Scale.</param>
            /// <param name="b">The Second Scale.</param>
            /// <returns>The sum of the two scales.</returns>
            public static Scale operator +(Scale a, Scale b)
            {
                Scale result = new Scale();
                result.Width = a.Width + b.Width;
                result.Height = a.Height + b.Height;
                return result;
            }

            /// <summary>
            /// Subtracts the second scale from the first.
            /// </summary>
            /// <param name="a">The First Scale.</param>
            /// <param name="b">The Seconds Scale.</param>
            /// <returns>The result of subtracting the two scales.</returns>
            public static Scale operator -(Scale a, Scale b)
            {
                Scale result = new Scale();
                result.Width = a.Width - b.Width;
                result.Height = a.Height - b.Height;
                return result;
            }

            /// <summary>
            /// Multiply the two scales together.
            /// </summary>
            /// <param name="a">The First Scale.</param>
            /// <param name="b">The Second Scale.</param>
            /// <returns>The product of the two scales.</returns>
            public static Scale operator *(Scale a, Scale b)
            {
                Scale result = new Scale();
                result.Width = a.Width * b.Width;
                result.Height = a.Height * b.Height;
                return result;
            }

            /// <summary>
            /// Divide the first scale by the second scale.
            /// </summary>
            /// <param name="a">The First Scale.</param>
            /// <param name="b">The Second Scale.</param>
            /// <returns>The result of dividing the two scales.</returns>
            public static Scale operator /(Scale a, Scale b)
            {
                Scale result = new Scale();
                result.Width = a.Width / b.Width;
                result.Height = a.Height / b.Height;
                return result;
            }
        }

        private protected Scale _scale = new Scale();

        private protected float _rotation = 0f;

        private protected SpriteEffects _spriteEffect = SpriteEffects.None;

        private protected float _spriteDepth = 0.5f;


        #region Colourize Variables
        private protected struct ColorF
        {
            private float R;
            private float G;
            private float B;
            private float A;


            public ColorF(float r, float g, float b, float a)
            {
                R = Math.Clamp(r, -255f, 255f);
                G = Math.Clamp(g, -255f, 255f);
                B = Math.Clamp(b, -255f, 255f);
                A = Math.Clamp(a, -255f, 255f);
            }

            public ColorF(Color color)
            {
                R = color.R;
                G = color.G;
                B = color.B;
                A = color.A;
            }

            public static ColorF GetChangeOverTime(Color startColour, Color destColour, float time)
            {
                ColorF overTime = new ColorF();
                float delta = 1000f / time / 60;
                overTime.R = (destColour.R - startColour.R) * delta;
                overTime.G = (destColour.G - startColour.G) * delta;
                overTime.B = (destColour.B - startColour.B) * delta;
                overTime.A = (destColour.A - startColour.A) * delta;
                return overTime;
            }

            public Color GetColour
            {
                get
                {
                    return new Color((byte)Math.Abs(R), (byte)Math.Abs(G), (byte)Math.Abs(B), (byte)Math.Abs(A));
                }
            }

            public static ColorF operator +(ColorF a, ColorF b)
            {
                ColorF result = new ColorF();
                result.R = Math.Clamp(a.R + b.R, -255f, 255f);
                result.G = Math.Clamp(a.G + b.G, -255f, 255f);
                result.B = Math.Clamp(a.B + b.B, -255f, 255f);
                result.A = Math.Clamp(a.A + b.A, -255f, 255f);
                return result;
            }

            public static ColorF operator -(ColorF a, ColorF b)
            {
                ColorF result = new ColorF();
                result.R = Math.Clamp(a.R - b.R, -255f, 255f);
                result.G = Math.Clamp(a.G - b.G, -255f, 255f);
                result.B = Math.Clamp(a.B - b.B, -255f, 255f);
                result.A = Math.Clamp(a.A - b.A, -255f, 255f);
                return result;
            }

            public static ColorF operator *(ColorF a, ColorF b)
            {
                ColorF result = new ColorF();
                result.R = Math.Clamp(a.R * b.R, -255f, 255f);
                result.G = Math.Clamp(a.G * b.G, -255f, 255f);
                result.B = Math.Clamp(a.B * b.B, -255f, 255f);
                result.A = Math.Clamp(a.A * b.A, -255f, 255f);
                return result;
            }
            public static ColorF operator /(ColorF a, ColorF b)
            {
                ColorF result = new ColorF();
                result.R = Math.Clamp(a.R / b.R, -255f, 255f);
                result.G = Math.Clamp(a.G / b.G, -255f, 255f);
                result.B = Math.Clamp(a.B / b.B, -255f, 255f);
                result.A = Math.Clamp(a.A / b.A, -255f, 255f);
                return result;
            }

        }

        private protected bool _isColourizing = false;
        private protected Color _colourizeDestColour;

        private protected ColorF _colourizeCurrentColour;
        private protected ColorF _colourizeChangeOverTime;
        private protected double _colourizeDuration;
        private protected double _colourizeElapsedTime;

        #endregion

        #region ScaleOverTime Variables

        private protected bool _isScaling = false;
        private protected Scale _scalingDestinationScale;
        private protected Scale _scalingChangeOverTime;
        private protected double _scalingDuration;
        private protected double _scalingElapsedTime;

        #endregion

        /// <summary>
        /// The rotation of the sprite.
        /// Between -pi and pi
        /// </summary>
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        /// <summary>
        /// Where should the sprite be drawn, in terms of layering.
        /// </summary>
        public float SpriteDepth
        {
            get { return _spriteDepth; }
            set { _spriteDepth = value; }
        }

        /// <summary>
        /// What SpriteEffect should be used for this sprite?
        /// </summary>
        public SpriteEffects SpriteEffect
        {
            get { return _spriteEffect; }
            set { _spriteEffect = value; }
        }

        /// <summary>
        /// Constructs a new SimpleSprite from the SpriteScript at the provided path.
        /// Also outputs that SpriteScript to be used by derived constructors.
        /// </summary>
        /// <param name="scriptPath">The path to the SpriteScript to load.</param>
        /// <param name="spriteScript">The spriteScript loaded, for passing to inheritors.</param>
        public SimpleSprite(string scriptPath, out SpriteScript spriteScript) : base(scriptPath, out SpriteScript script)
        {
            spriteScript = script;
        }


        #region Colour Functions
        private protected void UpdateColour()
        {
            if (_isColourizing)
            {
                _colourizeCurrentColour += _colourizeChangeOverTime;
                _colourizeElapsedTime += Engine.GameTime.ElapsedGameTime.TotalMilliseconds;
                if (_colourizeElapsedTime >= _colourizeDuration)
                {
                    _isColourizing = false;
                    _colour = _colourizeDestColour;
                }
                else
                    _colour = _colourizeCurrentColour.GetColour;
            }
        }

        /// <summary>
        /// Instantly sets the colour of the sprite, cancelling any active colourization.
        /// </summary>
        /// <param name="colour">The colour to set the sprite to.</param>
        public virtual void SetColour(Color colour)
        {
            _isColourizing = false;
            _colour = colour;
        }

        /// <summary>
        /// Transition the sprite to the specified colour over the span of the specified milliseconds.
        /// </summary>
        /// <param name="colour">The colour to transition to.</param>
        /// <param name="milliseconds">The time in milliseconds over which the change should occur.</param>
        public virtual void Colourize(Color colour, float milliseconds)
        {
            _colourizeDestColour = colour;
            _colourizeDuration = milliseconds;
            _colourizeElapsedTime = 0;
            _colourizeCurrentColour = new ColorF(_colour);
            _colourizeChangeOverTime = ColorF.GetChangeOverTime(_colour, colour, milliseconds);
            _isColourizing = true;
        }
        #endregion

        #region Scale Functions

        /// <summary>
        /// Instantly set the scale of the sprite, cancelling any active scaling over time.
        /// </summary>
        /// <param name="width">The width of the scalar.</param>
        /// <param name="height">The height of the scalar.</param>
        public virtual void SetScale(float width, float height)
        {
            _isScaling = false;
            _scale.Width = width;
            _scale.Height = height;
        }

        /// <summary>
        /// Instantly set the scale of the sprite, cancelling any active scaling over time.
        /// </summary>
        /// <param name="scale">The scalar as a Vector2.</param>
        public void SetScale(Vector2 scale)
        {
            SetScale(scale.X, scale.Y);
        }


        /// <summary>
        /// Transition the sprite to the specified scale over the span of the specified milliseconds.
        /// </summary>
        /// <param name="width">The width scalar.</param>
        /// <param name="height">The height scalar.</param>
        /// <param name="milliseconds">The time in milliseconds over which the change should occur.</param>
        public virtual void ScaleOverTime(float width, float height, float milliseconds)
        {
            _scalingDestinationScale.Width = width;
            _scalingDestinationScale.Height = height;
            _scalingChangeOverTime = Scale.GetChangeOverTime(_scale, _scalingDestinationScale, milliseconds);
            _scalingElapsedTime = 0.0;
            _scalingDuration = milliseconds;
            _isScaling = true;
        }

        private protected void UpdateScale()
        {
            if (_isScaling)
            {
                _scalingElapsedTime += Engine.GameTime.ElapsedGameTime.TotalMilliseconds;
                if (_scalingElapsedTime > _scalingDuration)
                {
                    _isScaling = false;
                    _scale = _scalingDestinationScale;
                }
                else
                    _scale += _scalingDestinationScale;

                _cullRec.Width = (int)(_texture.Width * _scale.Width) + 1;
                _cullRec.Height = (int)(_texture.Height * _scale.Height) + 1;
            }
        }

        #endregion

        /// <summary>
        /// Update the sprite.
        /// </summary>
        public override void UpdateSprite()
        {
            UpdateColour();
            UpdateScale();
        }

        /// <summary>
        /// Draw the sprite at a given position.
        /// </summary>
        /// <param name="position">The position to draw the sprite at.</param>
        public override void DrawSprite(Vector2 position)
        {
            if (_texture != null)
            {
                

                _cullRec.Width = (int)(_texture.Width * _scale.Width) + 1;
                _cullRec.Height = (int)(_texture.Height * _scale.Height) + 1;
                _cullRec.X = (int)position.X - _cullRec.Width / 2;
                _cullRec.Y = (int)position.Y - _cullRec.Height / 2;
                if (_cullRec.Intersects(Engine.Camera.VisibleArea))
                    Engine.SpriteBatch.Draw(_texture, position, _texture.Bounds ,_colour, _rotation, _texture.Bounds.Center.ToVector2(), _scale.RawVector2, _spriteEffect, _spriteDepth);
            }
        }
    }
}
