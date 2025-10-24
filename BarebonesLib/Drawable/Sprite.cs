using Barebones.Asset.Scripts;
using Barebones.Asset;
using Barebones.Config;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.VisualBasic;
using System.Runtime.Intrinsics.X86;
using System.Runtime.InteropServices;

namespace Barebones.Drawable
{
    /// <summary>
    /// A texture to be drawn on screen, with varying supporting functions.
    /// </summary>
    public class Sprite
    {

        /// <summary>
        /// Contains the scale of the sprite.
        /// </summary>
        public struct Scale
        {
            // The scale of the width of the sprite.
            private float _width;

            // The scale of the height of the sprite.
            private float _height;

            /// <summary>
            /// The scale of the width of the sprite. 1f is no scaling.
            /// </summary>
            public float Width
            {
                get { return _width; }
                set { _width = value; }
            }

            /// <summary>
            /// The scale of the higher of the sprite. 1f is no scaling.
            /// </summary>
            public float Height
            {
                get { return _height; }
                set { _height = value; }
            }

            /// <summary>
            /// Default constructor.
            /// </summary>
            public Scale()
            {
                Width = 1f;
                Height = 1f;
            }
        }

        /// <summary>
        /// Contains a single Frame for an Anim.
        /// </summary>
        public class Frame
        {
            // The rectangle we pull from the spriteSheet to draw.
            [JsonProperty]
            private Rectangle _sourceRec;

            // How long we wait until we move to the next frame.
            [JsonProperty]
            private float _speed;

            // The point this frame is centered on.
            [JsonProperty]
            private Vector2 _origin;

            // A list of all our attachpoints.
            [JsonProperty]
            private Dictionary<string, Vector2> _attachPoints;

            // A lua script to execute on frame start.
            [JsonProperty]
            private string _startLuaScript;

            // A lua script to execute on frame end.
            [JsonProperty]
            private string _endLuaScript;

            /// <summary>
            /// The rectangle we pull from the spriteSheet to draw.
            /// </summary>
            [JsonIgnore]
            public Rectangle SourceRec
            {
                get { return _sourceRec; }
            }

            /// <summary>
            /// The width of this Frame.
            /// </summary>
            [JsonIgnore]
            public int Width
            {
                get { return _sourceRec.Width; }
            }

            /// <summary>
            /// The height of this Frame.
            /// </summary>
            [JsonIgnore]
            public int Height
            {
                get { return _sourceRec.Height; }
            }

            /// <summary>
            /// How long we wait until we move to the next frame.
            /// </summary>
            [JsonIgnore]
            public float Speed
            {
                get { return _speed; }
            }

            /// <summary>
            /// The point this frame is centered on.
            /// </summary>
            [JsonIgnore]
            public Vector2 Origin
            {
                get { return _origin; }
            }

            /// <summary>
            /// The Lua script to execute when this frame becomes active.
            /// </summary>
            [JsonIgnore]
            public string StartingLuaScript
            {
                get { return _startLuaScript; }
            }

            /// <summary>
            /// The Lua script to execute when this frame becomes inactive.
            /// </summary>
            [JsonIgnore]
            public string EndingLuaScript
            {
                get { return _endLuaScript; }
            }

            /// <summary>
            /// Default Constructor.
            /// </summary>
            public Frame()
            {
                _sourceRec = new Rectangle(0, 0, 128, 128);
                _speed = 1000f;
                _origin = new Vector2(0, 0);
                _attachPoints = new Dictionary<string, Vector2>();
                _attachPoints.Add("DUMMY", new Vector2(0, 0));
            }

            /// <summary>
            /// Constructs a new Frame with a given sourceRec, speed and origin.
            /// </summary>
            /// <param name="sourceRec"></param>
            /// <param name="speed"></param>
            /// <param name="origin"></param>
            public Frame(Rectangle sourceRec, float speed, Vector2 origin)
            {
                _sourceRec = sourceRec;
                _speed = speed;
                _origin = origin;
            }

            /// <summary>
            /// Adds an AttachPoint to this frame with a given name and position.
            /// </summary>
            /// <param name="name">The name of the attach point.</param>
            /// <param name="Position">The position of the attach point, relative to the frame.</param>
            public void AddAttachPoint(string name, Vector2 Position)
            {
                _attachPoints.Add(name, Position);
            }

            /// <summary>
            /// Gets an attachpoint with a given name.
            /// </summary>
            /// <param name="name">The attachpoint to find.</param>
            /// <returns>The position of the attach point.</returns>
            public Vector2 GetAttachPoint(string name)
            {
                Vector2 pos = Vector2.Zero;
                _attachPoints.TryGetValue(name, out pos);
                return pos;
            }
        }

        /// <summary>
        /// Contains a single animation for a Sprite
        /// </summary>
        public class Anim
        {
            // The list of frames used in this animation.
            [JsonProperty]
            private List<Frame> _frames;

            [JsonProperty]
            private string _startLuaScript;

            [JsonProperty]
            private string _endLuaScript;

            /// <summary>
            /// The list of frames used in this animation.
            /// </summary>
            [JsonIgnore]
            public List<Frame> Frames
            {
                get { return _frames; }
            }

            /// <summary>
            /// The Lua script to execute when this animation becomes active.
            /// </summary>
            [JsonIgnore]
            public string StartingLuaScript
            {
                get { return _startLuaScript; }
            }

            /// <summary>
            /// The Lua script to execute when this animation becomes inactive.
            /// </summary>
            [JsonIgnore]
            public string EndingLuaScript
            {
                get { return _endLuaScript; }
            }

            /// <summary>
            /// Adds a frame to this animation with the given arguments.
            /// </summary>
            public void AddFrame(Frame frame)
            {
                _frames.Add(frame);
            }

        }


        private string _texturePath;

        private Color _colour;

        private Dictionary<string, Anim> _animations;

        private Anim _currentAnimation;

        private string _currentAnimName;

        private Scale _scale = new Scale();

        private float _rotation = 0f;

        private Texture2D _texture;

        private Frame _currentFrame;

        private int _currentFrameIndex;

        private Rectangle _drawRec;

        private double _speedMultiplier = 1.0;

        private double _animTimer = 0.0;

        private string _nextAnim = "";

        private SpriteEffects _spriteEffect = SpriteEffects.None;

        private float _spriteDepth = 0.5f;


        #region Colourizing
        private struct ColorF
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

        private bool _isColourizing = false;
        private Color _colourizeDestColour;
        
        private ColorF _colourizeCurrentColour;
        private ColorF _colourizeChangeOverTime;
        private double _colourizeDuration;
        private double _colourizeElapsedTime;

        #endregion

        /// <summary>
        /// The path of the texture loaded.
        /// </summary>
        public string TexturePath
        {
            get { return _texturePath; }
        }

        /// <summary>
        /// The current colour of the sprite.
        /// </summary>
        public Color Colour
        {
            get { return _colour; }
        }

        /// <summary>
        /// The collection of animations for this sprite.
        /// </summary>
        public Dictionary<string, Anim> Animations
        {
            get { return _animations; }
        }

        /// <summary>
        /// The animation currently being displayed.
        /// </summary>
        public Anim CurrentAnimation
        {
            get { return _currentAnimation; }
        }

        /// <summary>
        /// The current frame being displayed.
        /// </summary>
        public Frame CurrentFrame
        {
            get { return _currentFrame; }
        }

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
        /// A multiplier for animation speed.
        /// </summary>
        public double SpeedMultiplier
        {
            get { return _speedMultiplier; }
            set { _speedMultiplier = value; }
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
        /// What spriteeffect should be used for this sprite?
        /// </summary>
        public SpriteEffects SpriteEffect
        {
            get { return _spriteEffect; }
            set { _spriteEffect = value; }
        }

        /// <summary>
        /// Unload the current sprite.
        /// </summary>
        public void UnloadSprite()
        {
            Textures.UnloadTexture(_texturePath);
            _texture = null;
        }

        /// <summary>
        /// Construct a new sprite from a path to a SpriteScript.
        /// </summary>
        /// <param name="scriptPath"></param>
        public Sprite(string scriptPath)
        {
            SpriteScript script = ScriptFinder.FindScript<SpriteScript>(scriptPath);
            _texturePath = script.TexturePath;
            _animations = script.Anims;
            _texture = Textures.GetTexture(_texturePath);
            _colour = Color.White;
            _scale.Width = 10;
            _scale.Height = 10;
            ChangeAnimation(script.DefaultAnim);
        }

        /// <summary>
        /// Change the current animation.
        /// </summary>
        /// <param name="newAnim">The new animation.</param>
        public void ChangeAnimation(string newAnim)
        {
            try
            {
                if (_currentAnimation != _animations[newAnim])
                {
                    if (_currentAnimation?.EndingLuaScript != null)
                        Barebones.Lua.Script.RunScript(_currentAnimation.EndingLuaScript);

                    _currentAnimation = _animations[newAnim];
                    _currentAnimName = newAnim; // Just for logging purposes.
                    ChangeFrame(0, true);

                    if (_currentAnimation.StartingLuaScript != null)
                        Barebones.Lua.Script.RunScript(_currentAnimation.StartingLuaScript);
                }
            }
            catch (Exception ex)
            {
                string filename = _texturePath.Split('/').Last();
                Verbose.WriteErrorMinor($"Sprite({filename}) failed to change to animation: {newAnim}\n Ex: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Change the current frame.
        /// </summary>
        /// <param name="frameIndex">The index of the frame in the current animations frame array.</param>
        /// <param name="forceLuaScript">Should force the lua scripts to execute.</param>
        public void ChangeFrame(int frameIndex, bool forceLuaScript)
        {
            if (frameIndex > _currentAnimation.Frames.Count - 1 || frameIndex < 0)
            {
                string filename = _texturePath.Split('/').Last();
                Verbose.WriteErrorMinor($"Sprite({filename}), Animation: {_currentAnimName} attempted to change to non-existant frame: {frameIndex}. Setting to 0.");
                frameIndex = 0;
            }
            if (forceLuaScript || _currentFrame != _currentAnimation.Frames[frameIndex])
            {
                if (_currentFrame?.EndingLuaScript != null)
                    Barebones.Lua.Script.RunScript(_currentFrame.EndingLuaScript);

                _currentFrame = _currentAnimation.Frames[frameIndex];
                _currentFrameIndex = frameIndex;
                _animTimer = 0;

                if (_currentFrame.StartingLuaScript != null)
                    Barebones.Lua.Script.RunScript(_currentFrame.StartingLuaScript);

            }
        }

        /// <summary>
        /// Update the sprite.
        /// </summary>
        public void UpdateSprite()
        {
            UpdateAnimation();
            UpdateColour();
        }


        private void UpdateAnimation()
        {
            _animTimer += Engine.GameTime.ElapsedGameTime.TotalMilliseconds * SpeedMultiplier;
            if (_animTimer >= _currentFrame.Speed)
            {
                _currentFrameIndex++;
                if (_currentFrameIndex >= _currentAnimation.Frames.Count)
                {
                    _currentFrameIndex = 0;
                    if (_nextAnim != "")
                    {
                        ChangeAnimation(_nextAnim);
                        _nextAnim = "";
                    }
                }
                ChangeFrame(_currentFrameIndex, false);
            }
        }

        private void UpdateColour()
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
        /// <param name="colour"></param>
        public void SetColour(Color colour)
        {
            _isColourizing = false;
            _colour = colour;
        }

        /// <summary>
        /// Transition the sprite to the specified colour over the span of the specified milliseconds.
        /// </summary>
        /// <param name="colour">The colour to transition to.</param>
        /// <param name="milliseconds">The time in milliseconds over which the change should occur.</param>
        public void Colourize(Color colour, float milliseconds)
        {
            _colourizeDestColour = colour;
            _colourizeDuration = milliseconds;
            _colourizeElapsedTime = 0;
            _colourizeCurrentColour = new ColorF(_colour);
            _colourizeChangeOverTime = ColorF.GetChangeOverTime(_colour, colour, milliseconds);
            _isColourizing = true;
        }

        /// <summary>
        /// Draw the sprite at a given position.
        /// </summary>
        /// <param name="position">The position to draw the sprite at.</param>
        public void DrawSprite(Vector2 position)
        {
            if (_texture != null)
            {
                _drawRec.X = (int)position.X;
                _drawRec.Y = (int)position.Y;
                _drawRec.Width = (int)(_currentFrame.Width * _scale.Width);
                _drawRec.Height = (int)(_currentFrame.Height * _scale.Height);
                Engine.SpriteBatch.Draw(_texture, _drawRec, _currentFrame.SourceRec, _colour, _rotation, _currentFrame.Origin, _spriteEffect, _spriteDepth);
            }
        }


    }
}
