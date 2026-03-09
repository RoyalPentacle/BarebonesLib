using Barebones.Asset;
using Barebones.Asset.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Frozen;

namespace Barebones.Drawable
{
    /// <summary>
    /// A texture to be drawn on screen, with varying supporting functions.
    /// </summary>
    public class ComplexSprite : SimpleSprite
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
            private string? _startLuaScript;

            // A lua script to execute on frame end.
            [JsonProperty]
            private string? _endLuaScript;

            /// <summary>
            /// The rectangle we pull from the spriteSheet to draw.
            /// </summary>
            [JsonIgnore]
            public Rectangle SourceRec
            {
                get { return _sourceRec; }
                set { _sourceRec = value; }
            }

            /// <summary>
            /// The width of this Frame.
            /// </summary>
            [JsonIgnore]
            public int Width
            {
                get { return _sourceRec.Width; }
                set { _sourceRec.Width = value; }
            }

            /// <summary>
            /// The height of this Frame.
            /// </summary>
            [JsonIgnore]
            public int Height
            {
                get { return _sourceRec.Height; }
                set { _sourceRec.Height = value; }
            }

            /// <summary>
            /// The X coordinate of this Frame.
            /// </summary>
            [JsonIgnore]
            public int X
            {
                get { return _sourceRec.X; }
                set { _sourceRec.X = value; }
            }

            /// <summary>
            /// The Y coordinate of this Frame.
            /// </summary>
            [JsonIgnore]
            public int Y
            {
                get { return _sourceRec.Y; }
                set { _sourceRec.Y = value; }
            }

            /// <summary>
            /// The X coordinate of this Frames origin.
            /// </summary>
            [JsonIgnore]
            public float OriginX
            {
                get { return _origin.X; }
                set { _origin.X = value; } 
            }

            /// <summary>
            /// The Y coordinate of this Frames origin.
            /// </summary>
            [JsonIgnore]
            public float OriginY
            {
                get { return _origin.Y; }
                set { _origin.Y = value; }
            }

            /// <summary>
            /// How long we wait until we move to the next frame.
            /// </summary>
            [JsonIgnore]
            public float Speed
            {
                get { return _speed; }
                set { _speed = value; }
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
            public string? StartingLuaScript
            {
                get { return _startLuaScript; }
            }

            /// <summary>
            /// The Lua script to execute when this frame becomes inactive.
            /// </summary>
            [JsonIgnore]
            public string? EndingLuaScript
            {
                get { return _endLuaScript; }
            }

            /// <summary>
            /// Default Constructor.
            /// </summary>
            public Frame()
            {
                _sourceRec = new Rectangle(0, 0, 32, 32);
                _speed = 1000f;
                _origin = new Vector2(16, 16);
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
            /// <param name="pos">The position of the attachpoint.</param>
            /// <returns>The position of the attach point.</returns>
            public bool TryGetAttachPoint(string name, out Vector2 pos)
            {
                pos = Vector2.Zero;
                return _attachPoints.TryGetValue(name, out pos);

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
                if (_frames == null)
                    _frames = new List<Frame>();
                _frames.Add(frame);
            }

        }

        private Dictionary<string, Anim> _animations;

        private List<Dictionary<uint, Color>> _colourPalettes;

        private int _currentPalette = -1;

        private Texture2D? _colouredTexture;

        private Vector2 _lastPosition;

        private Anim _currentAnimation;

        private string _currentAnimName;

        private Frame _currentFrame;

        private int _currentFrameIndex;

        private double _speedMultiplier = 1.0;

        private double _animTimer = 0.0;

        private string _nextAnim = "";

        private bool _ignoreLua = false;

        private bool _ignoreCulling = false;

        private string _defaultAnim;

        /// <summary>
        /// The default animation for this sprite.
        /// </summary>
        public string DefaultAnim
        {
            get { return _defaultAnim; }
            set { _defaultAnim = value; }
        }

        /// <summary>
        /// Always draw the sprite regardless of position relative to the camera.
        /// </summary>
        public bool IgnoreCulling
        {
            get { return _ignoreCulling; }
            set { _ignoreCulling = value; }
        }

        /// <summary>
        /// Should this sprite ignore all lua commands from its animations?
        /// </summary>
        public bool IgnoreLua
        {
            get { return _ignoreLua; }
            set { _ignoreLua = value; }
        }

        /// <summary>
        /// The last position this sprite was drawn to.
        /// </summary>
        public Vector2 LastPosition
        {
            get { return _lastPosition; }
        }

        /// <summary>
        /// The collection of animations for this sprite.
        /// </summary>
        public Dictionary<string, Anim> Animations
        {
            get { return _animations; }
        }

        /// <summary>
        /// The collection of colour palettes for this sprite.
        /// </summary>
        public List<Dictionary<uint, Color>> ColourPalettes
        {
            get { return _colourPalettes; }
        }

        /// <summary>
        /// The animation currently being displayed.
        /// </summary>
        public Anim CurrentAnimation
        {
            get { return _currentAnimation; }
        }

        /// <summary>
        /// The name of the current animation being displayed.
        /// </summary>
        public string CurrentAnimationName
        {
            get { return _currentAnimName; }
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

        internal bool IsColourizing
        {
            get { return _isColourizing; }
        }


        private bool _showFullTexture = false;

        internal bool ShowFullTexture
        {
            get { return _showFullTexture; }
            set { _showFullTexture = value; }
        }

        private bool _pauseAnimation = false;
        
        internal bool PauseAnimation
        {
            get { return _pauseAnimation; }
            set { _pauseAnimation = value; }
        }

        internal int TextureWidth
        {
            get 
            {
                if (_texture != null)
                    return _texture.Width;
                else
                    return 0;
            }
        }

        internal int TextureHeight
        {
            get 
            {
                if (_texture != null)
                    return _texture.Height;
                else
                    return 0;
            }
        }

        /// <summary>
        /// The number of palettes available to this sprite.
        /// </summary>
        /// <remarks>
        /// Not including the default appearance.
        /// </remarks>
        public int PaletteCount
        {
            get 
            { 
                if (_colourPalettes != null)
                    return _colourPalettes.Count;
                else 
                    return 0;
            }
        }

        /// <summary>
        /// Construct an empty ComplexSprite.
        /// </summary>
        public ComplexSprite()
        {
            _colour = Color.White;
            _texturePath = "";
            if (_animations == null)
            {
                _animations = new Dictionary<string, Anim>();
                Anim anim = new Anim();
                anim.AddFrame(new Frame(new Rectangle(0, 0, 32, 32), 1000f, new Vector2(16, 16)));
                _animations.Add("IDLE", anim);
                _defaultAnim = "IDLE";
            }
        }

        internal void ChangeTexture(string texturePath)
        {

            try
            {
                Texture2D tex = Textures.GetTexture(texturePath);
                if (tex != Textures.Shared.FallbackTexture)
                {
                    UnloadSprite();
                    _texture = tex;
                    _texturePath = texturePath;
                    if (_currentFrame == null)
                    {
                        _currentFrame = new Frame(_texture.Bounds, 1000f, _texture.Bounds.Center.ToVector2());
                    }
                    if (_currentAnimation == null)
                    {
                        _currentAnimation = new Anim();
                        _currentAnimation.AddFrame(_currentFrame);
                    }
                }
            }
            catch (Exception ex)
            {
                Verbose.WriteErrorMajor($"Failed to load texture: {texturePath} \nEX: {ex.Message}");
            }
        }

        /// <summary>
        /// Construct a new sprite from a path to a SpriteScript.
        /// </summary>
        /// <param name="scriptPath">The path to the SpriteScript to load.</param>
        public ComplexSprite(string scriptPath) : this(scriptPath, false)
        {

        }


        /// <summary>
        /// Construct a new sprite from a path to a SpriteScript, with the option to ignore lua scripts.
        /// </summary>
        /// <param name="scriptPath">The path to the SpriteScript to load.</param>
        /// <param name="ignoreLua">Ignore Lua?</param>
        public ComplexSprite(string scriptPath, bool ignoreLua) : base(scriptPath, out SpriteScript script)
        {
            _ignoreLua = ignoreLua;
            _animations = script.Anims;
            if (_animations.Count == 0)
            {
                Anim fallback = new Anim();
                if (_texture != null)
                    fallback.AddFrame(new Frame(new Rectangle(0, 0, _texture.Width, _texture.Height), 1000f, new Vector2(_texture.Width / 2, _texture.Height / 2)));
                else
                    fallback.AddFrame(new Frame(new Rectangle(0, 0, 32, 32), 1000f, new Vector2(16, 16)));
                _animations.Add("IDLE", fallback);
            }
            _defaultAnim = script.DefaultAnim;
            ChangeAnimation(script.DefaultAnim);
            _colour = Color.White;
            _colourPalettes = script.ColourPalettes;
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
                    if (!_ignoreLua && _currentAnimation?.EndingLuaScript != null)
                        Lua.Functions.RunScript(_currentAnimation.EndingLuaScript);

                    _currentAnimation = _animations[newAnim];
                    _currentAnimName = newAnim; // Just for logging purposes.
                    _nextAnim = nextAnim;
                    ChangeFrame(0, true);
                    if (!_ignoreLua && _currentAnimation.StartingLuaScript != null)
                        Lua.Functions.RunScript(_currentAnimation.StartingLuaScript);
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
                if (!_ignoreLua && _currentFrame?.EndingLuaScript != null)
                    Lua.Functions.RunScript(_currentFrame.EndingLuaScript);

                _currentFrame = _currentAnimation.Frames[frameIndex];
                _currentFrameIndex = frameIndex;
                _animTimer = 0;
                
                if (!_ignoreLua && _currentFrame.StartingLuaScript != null)
                    Lua.Functions.RunScript(_currentFrame.StartingLuaScript);

            }
            _cullRec.Width = (int)(_currentFrame.Width * _scale.Width) + 1;
            _cullRec.Height = (int)(_currentFrame.Height * _scale.Height) + 1;
        }

        private void UpdateAnimation()
        {
            _animTimer += Engine.GameTime.ElapsedGameTime.TotalMilliseconds * SpeedMultiplier;
            if (_currentFrame != null)
            {
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
        }
        #endregion

        #region Palette Swap Functions

        /// <summary>
        /// Changes the palette of the sprite to the specified index.
        /// </summary>
        /// <remarks>
        /// This is a software colour replacement. Specific RGBA values swapped for other specific RGBA values.
        /// Do not do this very often, it's fairly expensive.
        /// </remarks>
        /// <param name="paletteIndex">The index of the colour palette. -1 to revert to default.</param>
        public void ChangePalette(int paletteIndex)
        {
            if (paletteIndex != _currentPalette)
            {
                _currentPalette = paletteIndex;
                if (paletteIndex <= -1)
                {
                    _currentPalette = -1;
                    _colouredTexture?.Dispose();
                    _colouredTexture = null;
                }
                else if (_colourPalettes != null && paletteIndex < _colourPalettes.Count)
                {
                    if (_texture != null)
                    {
                        _colouredTexture?.Dispose();
                        _colouredTexture = null;
                        Dictionary<uint, Color> palette = _colourPalettes[(int)paletteIndex];
                        Color[] pixels = new Color[_texture.Width * _texture.Height];
                        _texture.GetData(pixels);
                        for (int i = 0; i < pixels.Length; i++)
                        {
                            if (pixels[i] == Color.Transparent)
                                continue;
                            else
                            {
                                if (palette.TryGetValue(pixels[i].PackedValue, out Color col))
                                    pixels[i] = col;
                            }
                        }
                        _colouredTexture = new Texture2D(Engine.Graphics.GraphicsDevice, _texture.Width, _texture.Height);
                        _colouredTexture.SetData(pixels);
                    }
                }
            }
        }

        /// <summary>
        /// Revert to the default colour palette for the sprite.
        /// </summary>
        public void RevertToDefaultPalette()
        {
            ChangePalette(-1);
        }

        /// <summary>
        /// Cycle the colour palette to the next set in the array.
        /// </summary>
        /// <remarks>
        /// If at the end of the array, wraps back to the default appearance.
        /// </remarks>
        public void IncrementPalette()
        {
            if (_colourPalettes != null)
            {
                int nextPalette = _currentPalette + 1;
                if (nextPalette >= _colourPalettes.Count)
                    nextPalette = -1;
                ChangePalette(nextPalette);
            }
        }

        /// <summary>
        /// Cycle the colour palette to the previous set in the array.
        /// </summary>
        /// <remarks>
        /// If at the start of the array, wraps to the end of the array.
        /// </remarks>
        public void DecrementPalette()
        {
            if (_colourPalettes != null)
            {
                int prevPalette = _currentPalette - 1;
                if (prevPalette < -1)
                    prevPalette = _colourPalettes.Count - 1;
                ChangePalette(prevPalette);
            }
        }

        #endregion

        /// <summary>
        /// Update the sprite.
        /// </summary>
        public override void Update()
        {
            if (!_pauseAnimation)
                UpdateAnimation();
            base.Update();
        }

        /// <summary>
        /// Unload the current sprite.
        /// </summary>
        public override void UnloadSprite()
        {
            _colouredTexture?.Dispose();
            _colouredTexture = null;
            base.UnloadSprite();
        }

        /// <summary>
        /// Get the animation of the specified name, if it exists, otherwise gets "IDLE".
        /// </summary>
        /// <param name="animName">The name of the animation.</param>
        /// <param name="animation">The output animation.</param>
        /// <returns>True if the animation exists, False otherwise.</returns>
        public bool GetAnimation(string animName, out Anim? animation)
        {
            if (_animations.TryGetValue(animName, out animation))
                return true;
            else
                animation = _animations["IDLE"];
            return false;
            
        }

        /// <summary>
        /// Draw the sprite at a given position.
        /// </summary>
        /// <param name="position">The position to draw the sprite at.</param>
        public override void Draw(Vector2 position)
        {
            if (_texture != null)
            {
                _cullRec.X = (int)position.X - _cullRec.Width / 2;
                _cullRec.Y = (int)position.Y - _cullRec.Height / 2 ;
                _lastPosition = position;
                if (!_showFullTexture)
                {
                    if (_ignoreCulling || _cullRec.Intersects(Engine.Camera.VisibleArea))
                    {
                        if (_colouredTexture == null)
                            Engine.SpriteBatch.Draw(_texture, position, _currentFrame.SourceRec, _colour, _rotation, _currentFrame.Origin, _scale.RawVector2, _spriteEffect, _spriteDepth);
                        else
                            Engine.SpriteBatch.Draw(_colouredTexture, position, _currentFrame.SourceRec, _colour, _rotation, _currentFrame.Origin, _scale.RawVector2, _spriteEffect, _spriteDepth);
                    }
                }
                else if (_showFullTexture)
                {
                    if (_ignoreCulling || _cullRec.Intersects(Engine.Camera.VisibleArea))
                    {
                        if (_colouredTexture == null)
                            Engine.SpriteBatch.Draw(_texture, position, _texture.Bounds, _colour, _rotation, _texture.Bounds.Center.ToVector2(), _scale.RawVector2, _spriteEffect, _spriteDepth);
                        else
                            Engine.SpriteBatch.Draw(_colouredTexture, position, _texture.Bounds, _colour, _rotation, _texture.Bounds.Center.ToVector2(), _scale.RawVector2, _spriteEffect, _spriteDepth);
                    }
                }
            }
        }


    }
}
