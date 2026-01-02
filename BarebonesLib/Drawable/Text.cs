using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Drawable
{
    /// <summary>
    /// A class that draws text to the screen.
    /// </summary>
    public class Text
    {
        private string _storedText;
        private string[] _animArray;
        private ComplexSprite _font;
        private int _textWidth;
        private int _textHeight;
        private float _scale;

        private Color[] _letterColour;

        public ComplexSprite Font
        {
            get { return _font; }
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
        /// Construct a new text with default scaling.
        /// </summary>
        /// <param name="text">The string to display.</param>
        /// <param name="scriptPath">The path to the spritescript to use as a font.</param>
        public Text(string text, string scriptPath) : this(text, scriptPath, 1f)
        {

        }

        /// <summary>
        /// Construct a new text with scaling.
        /// </summary>
        /// <param name="text">The string to display.</param>
        /// <param name="scriptPath">The path to the spritescript to use as a font.</param>
        /// <param name="scale">The scale of the text.</param>
        public Text(string text, string scriptPath, float scale)
        {
            _font = new ComplexSprite(scriptPath);
            _scale = scale;
            _font.SetScale(scale, scale);
            ChangeText(text);
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
        public void SetColour(List<Color> colours)
        {
            for (int i = 0; i < _letterColour?.Length && i < colours.Count; i++)
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
            if (text != _storedText)
            {
                _storedText = text;
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
                        _textWidth = (int)(_textWidth + _font.Animations["MISS"].Frames[0].Width * _scale);
                    }
                    _letterColour[i] = Color.White;

                }
            }
        }

        /// <summary>
        /// Updates the text object.
        /// </summary>
        public void Update()
        {
            _font.UpdateSprite();
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
                    _font.SetColour(_letterColour[0]);
                }

                _font.ChangeAnimation(_animArray[i]);
                position.X += (_font.CurrentFrame.Width - _font.CurrentFrame.Origin.X) * _scale;
                _font.Draw(position);
                position.X += (_font.CurrentFrame.Width - _font.CurrentFrame.Origin.X + 2) * _scale;
            }
        }
    }
}
