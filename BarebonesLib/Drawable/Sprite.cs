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
    public class Sprite : SimpleSprite
    {

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

        private Dictionary<string, Anim> _animations;

        private Anim _currentAnimation;

        private string _currentAnimName;

        private Frame _currentFrame;

        private int _currentFrameIndex;

        private double _speedMultiplier = 1.0;

        private double _animTimer = 0.0;

        private string _nextAnim = "";

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
        /// A multiplier for animation speed.
        /// </summary>
        public double SpeedMultiplier
        {
            get { return _speedMultiplier; }
            set { _speedMultiplier = value; }
        }



        /// <summary>
        /// Construct a new sprite from a path to a SpriteScript.
        /// </summary>
        /// <param name="scriptPath">The path to the SpriteScript to load.</param>
        public Sprite(string scriptPath) : base(scriptPath, out SpriteScript script)
        {
            _animations = script.Anims;
            _texture = Textures.GetTexture(_texturePath);
            _colour = Color.White;
            ChangeAnimation(script.DefaultAnim);
        }

        #region Animation Functions

        /// <summary>
        /// Change the current animation.
        /// </summary>
        /// <param name="newAnim">The new animation.</param>
        public void ChangeAnimation(string newAnim)
        {
            ChangeAnimation(newAnim, "");
        }

        /// <summary>
        /// Change the current animation, then when the animation loops, change to another animation.
        /// </summary>
        /// <param name="newAnim">The first animation.</param>
        /// <param name="nextAnim">The animation to set next.</param>
        public void ChangeAnimation(string newAnim, string nextAnim)
        {
            try
            {
                if (_currentAnimation != _animations[newAnim])
                {
                    if (_currentAnimation?.EndingLuaScript != null)
                        Lua.Script.RunScript(_currentAnimation.EndingLuaScript);

                    _currentAnimation = _animations[newAnim];
                    _currentAnimName = newAnim; // Just for logging purposes.
                    _nextAnim = nextAnim;
                    ChangeFrame(0, true);

                    if (_currentAnimation.StartingLuaScript != null)
                        Lua.Script.RunScript(_currentAnimation.StartingLuaScript);
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
                    Lua.Script.RunScript(_currentFrame.EndingLuaScript);

                _currentFrame = _currentAnimation.Frames[frameIndex];
                _currentFrameIndex = frameIndex;
                _animTimer = 0;

                if (_currentFrame.StartingLuaScript != null)
                    Lua.Script.RunScript(_currentFrame.StartingLuaScript);

            }
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
                        return;
                    }
                }
                ChangeFrame(_currentFrameIndex, false);
            }
        }
        #endregion

        
        /// <summary>
        /// Update the sprite.
        /// </summary>
        public override void UpdateSprite()
        {
            UpdateAnimation();
            base.UpdateSprite();
        }



        /// <summary>
        /// Draw the sprite at a given position.
        /// </summary>
        /// <param name="position">The position to draw the sprite at.</param>
        public override void DrawSprite(Vector2 position)
        {
            if (_texture != null)
            {
                _drawRec.X = (int)position.X;
                _drawRec.Y = (int)position.Y;
                Engine.SpriteBatch.Draw(_texture, _drawRec, _currentFrame.SourceRec, _colour, _rotation, _currentFrame.Origin, _spriteEffect, _spriteDepth);
            }
        }


    }
}
