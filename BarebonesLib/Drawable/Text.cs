using Barebones.Config;
using Barebones.Interfaces;
using Microsoft.Xna.Framework;

namespace Barebones.Drawable
{
    /// <summary>
    /// A class that draws text to the screen.
    /// </summary>
    public class Text : IDrawnObject
    {

        private static readonly List<Text> _textList = new List<Text>();

        internal static void ChangeLanguage()
        {
            for (int i = 0; i < _textList.Count; i++)
            {
                _textList[i].ChangeText(_textList[i]._originalText);
            }
        }

        private string _originalText;
        private string _storedText;
        private string[] _animArray;
        private ComplexSprite _font;
        private int _textWidth;
        private int _textHeight;
        private float _scale;

        private string _fontPath;

        private Color[] _letterColour;

        private IParent? _parent;

        /// <summary>
        /// The ComplexSprite that functions as the font for this text.
        /// </summary>
        public ComplexSprite Font
        {
            get { return _font; }
        }

        /// <summary>
        /// The pre-localization string for this text.
        /// </summary>
        public string OriginalText
        {
            get { return _originalText; }
        }

        /// <summary>
        /// The string to be displayed by this text.
        /// </summary>
        public string StoredText
        {
            get { return _storedText; }
        }

        /// <summary>
        /// The width of the text.
        /// </summary>
        public int TextWidth
        {
            get { return _textWidth; }
        }

        /// <summary>
        /// The height of the text.
        /// </summary>
        public int TextHeight
        {
            get { return _textHeight; }
        }

        /// <summary>
        /// The scale of the Text.
        /// </summary>
        public float Scale
        {
            get { return _scale; }
        }

        /// <summary>
        /// The path to the SpriteScript for the font used.
        /// </summary>
        public string FontPath
        {
            get { return _fontPath; }
        }

        /// <summary>
        /// Construct a new text with default scaling.
        /// </summary>
        /// <param name="text">The string to display. Will attempt to localize if a language file has been loaded.</param>
        /// <param name="scriptPath">The path to the spritescript to use as a font.</param>
        public Text(string text, string scriptPath) : this(text, scriptPath, 1f, null)
        {

        }

        /// <summary>
        /// Construct a new text with default scaling, and a parent.
        /// </summary>
        /// <param name="text">The string to display. Will attempt to localize if a language file has been loaded.</param>
        /// <param name="scriptPath">The path to the spritescript to use as a font.</param>
        /// <param name="parent">The parent object.</param>
        public Text(string text, string scriptPath, IParent parent) : this(text, scriptPath, 1f, parent)
        {

        }

        /// <summary>
        /// Construct a new text with scaling.
        /// </summary>
        /// <param name="text">The string to display. Will attempt to localize if a language file has been loaded.</param>
        /// <param name="scriptPath">The path to the spritescript to use as a font.</param>
        /// <param name="scale">The scale of the text.</param>
        public Text(string text, string scriptPath, float scale) : this(text, scriptPath, scale, null)
        {

        }

        /// <summary>
        /// Construct a new text with caling, and a parent.
        /// </summary>
        /// <param name="text">The string to display. Will attempt to localize if a language file has been loaded.</param>
        /// <param name="scriptPath">The path to the spritescript to use as a font.</param>
        /// <param name="scale">The scale of the text.</param>
        /// <param name="parent">The parent object.</param>
        public Text(string text, string scriptPath, float scale, IParent? parent)
        {
            _parent = parent;
            _font = new ComplexSprite(scriptPath);
            _fontPath = scriptPath;
            _scale = scale;
            _font.SetScale(scale, scale);
            ChangeText(text);
            _textList.Add(this);
        }
                
        /// <summary>
        /// Instantly set the colour of the text.
        /// Cancels active colourization.
        /// </summary>
        /// <param name="colour">The colour to set.</param>
        public void SetColour(Color colour)
        {
            _font.SetColour(colour);
            for (int i = 0; i < _letterColour?.Length; i++)
            {
                _letterColour[i] = colour;
            }
        }

        /// <summary>
        /// Instantly set the colour of each character in the text.
        /// Does not cancel active colourization.
        /// </summary>
        /// <param name="colours">A list of colours corrseponding to the characters in the text.</param>
        public void SetColour(Color[] colours)
        {
            for (int i = 0; i < _letterColour?.Length && i < colours.Length; i++)
            {
                _letterColour[i] = colours[i];
            }
        }

        /// <summary>
        /// Colourize the text over time.
        /// </summary>
        /// <param name="colour">The colour to transition to.</param>
        /// <param name="milliseconds">The time in milliseconds over which the change should occur.</param>
        public void Colourize(Color colour, float milliseconds)
        {
            _font.Colourize(colour, milliseconds);
        }

        /// <summary>
        /// Changes the displayed string of the text.
        /// </summary>
        /// <param name="text">The string to display.</param>
        public void ChangeText(string text)
        {
            if (text != _originalText)
            {
                string temp = _originalText;
                _originalText = text;
                _storedText = text;
                if (!string.IsNullOrEmpty(Language.CurrentLanguage))
                {
                    _storedText = Language.Translate(_originalText);
                }
                _animArray = new string[text.Length];
                for (int i = 0; i < text.Length; i++)
                {
                    _animArray[i] = text[i].ToString();
                }
                _letterColour = new Color[_animArray.Length];
                _textWidth = 0;
                _textHeight = 0;
                for (int i = 0; i < _animArray.Length; i++)
                {
                    if (_font.Animations.TryGetValue(_animArray[i], out ComplexSprite.Anim? anim))
                    {
                        _textWidth = (int)(_textWidth + ((anim.Frames[0].Width + 2) * _scale));
                        if (_textHeight < anim.Frames[0].Height * _scale)
                        {
                            _textHeight = (int)(anim.Frames[0].Height * _scale);
                        }
                    }
                    else
                    {
                        Verbose.WriteErrorMinor($"Could not find char: {_animArray[i]} in font {_font.TexturePath}");
                        _animArray[i] = "MISS";
                        _textWidth = (int)(_textWidth + 16 * _scale);
                    }
                    _letterColour[i] = Color.White;

                }
                //if the original text, before this change, was not empty or null, then recalculate the size of the parent, if it exists.
                if (!string.IsNullOrEmpty(temp)) 
                    _parent?.RecalculateSize();
            }
        }

        /// <summary>
        /// Updates the text object.
        /// </summary>
        public void Update()
        {
            _font.Update();
            if (_font.IsColourizing)
            {
                for (int i = 0; i < _letterColour.Length; i++)
                {
                    _letterColour[i] = _font.Colour;
                }
            }
        }

        /// <summary>
        /// Unloads the text object.
        /// </summary>
        public void Unload()
        {
            _textList.Remove(this);
            _font.UnloadSprite();
        }

        /// <summary>
        /// Draws the text object.
        /// </summary>
        /// <param name="position">The position to draw the text object.</param>
        public void Draw(Vector2 position)
        {
            for (int i = 0; i < _animArray.Length; i++)
            {
                if (!_font.IsColourizing)
                {
                    _font.SetColour(_letterColour[i]);
                }

                _font.ChangeAnimation(_animArray[i]);
                position.X += (_font.CurrentFrame.Width - _font.CurrentFrame.Origin.X) * _scale;
                _font.Draw(position);
                position.X += (_font.CurrentFrame.Width - _font.CurrentFrame.Origin.X + 2) * _scale;
            }
        }
    }
}
