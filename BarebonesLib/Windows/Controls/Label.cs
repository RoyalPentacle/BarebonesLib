using Barebones.Drawable;
using Barebones.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Windows.Controls
{
    /// <summary>
    /// This control displays text, with an optional background.
    /// </summary>
    public class Label : IControl, IParent
    {
        private Window _parent;
        private string _name;
        private Rectangle _bounds;
        private Vector2 _textOffset;

        private bool _hasBackground;
        private bool _autoSized;

        private Text _labelText;
        private ComplexSprite _background;
        private ComplexSprite _topEdge;
        private ComplexSprite _rightEdge;
        private ComplexSprite _bottomEdge;
        private ComplexSprite _leftEdge;
        private ComplexSprite _topLeftCorner;
        private ComplexSprite _topRightCorner;
        private ComplexSprite _bottomLeftCorner;
        private ComplexSprite _bottomRightCorner;

        /// <summary>
        /// The parent window for this label.
        /// </summary>
        public Window Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// The name of the label.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The pre-localized string for the text in this label.
        /// </summary>
        public string LabelText
        {
            get { return _labelText.OriginalText; }
            set { _labelText.ChangeText(value); }
        }

        /// <summary>
        /// Construct a new Label with the specified arguments. Auto sizes to the size of the text.
        /// </summary>
        /// <remarks>Defaults to not having a background.</remarks>
        /// <param name="name">The name of the label.</param>
        /// <param name="position">The location of the label.</param>
        /// <param name="text">The text displayed by this label.</param>
        /// <param name="fontScriptPath">The script path to the font used by this label.</param>
        /// <param name="fontScale">The scale of the text.</param>
        /// <param name="textColor">The colour of the text.</param>
        /// <param name="parent">The parent window.</param>
        public Label(string name, Point position, string text, string fontScriptPath, float fontScale, Color textColor, Window parent) : this(name, position, false, text, fontScriptPath, fontScale, textColor, parent)
        {

        }

        /// <summary>
        /// Construct a new Label with the specified arguments. Auto sizes to the size of the text.
        /// </summary>
        /// <param name="name">The name of the label.</param>
        /// <param name="position">The location of the label.</param>
        /// <param name="hasBackground">Does this label have a background?</param>
        /// <param name="text">The text displayed by this label.</param>
        /// <param name="fontScriptPath">The script path to the font used by this label.</param>
        /// <param name="fontScale">The scale of the text.</param>
        /// <param name="textColor">The colour of the text.</param>
        /// <param name="parent">The parent window.</param>
        public Label(string name, Point position, bool hasBackground, string text, string fontScriptPath, float fontScale, Color textColor, Window parent)
        {
            _labelText = new Text(text, fontScriptPath, fontScale, this);
            _labelText.SetColour(textColor);
            _labelText.Font.IgnoreCulling = true;
            Rectangle bounds = new Rectangle(position.X, position.Y, _labelText.TextWidth, _labelText.TextHeight);
            bounds.Height = (int)(bounds.Height * 1.5f);
            if (bounds.Height % 2 == 1)
                bounds.Height += 1;
            float halfFirstChar = 0f;
            if (!string.IsNullOrEmpty(text))
            {
                if (_labelText.Font.GetAnimation(_labelText.StoredText[0].ToString(), out ComplexSprite.Anim? anim))
                {
                    if (anim != null)
                    {
                        bounds.Width += anim.Frames[0].Width;
                        halfFirstChar = 2; // anim.Frames[0].Width / 2f;
                    }
                }
            }
            if (bounds.Width % 2 == 1)
                bounds.Width += 1;
            _textOffset = new Vector2(-(_labelText.TextWidth / 2f - halfFirstChar), 0);
            _autoSized = true;
            Initialize(name, bounds, hasBackground, text, fontScriptPath, fontScale, textColor, parent);
        }

        /// <summary>
        /// Construct a new Label with the specified arguments. Creates a label of a specific size.
        /// </summary>
        /// <remarks>Defaults to not having a background.</remarks>
        /// <param name="name">The name of the label.</param>
        /// <param name="bounds">The bounds of the label.</param>
        /// <param name="text">The text displayed by this label.</param>
        /// <param name="fontScriptPath">The script path to the font used by this label.</param>
        /// <param name="fontScale">The scale of the text.</param>
        /// <param name="textColor">The colour of the text.</param>
        /// <param name="parent">The parent window.</param>
        public Label(string name, Rectangle bounds, string text, string fontScriptPath, float fontScale, Color textColor, Window parent) : this(name, bounds, false, text, fontScriptPath, fontScale, textColor, parent)
        {

        }

        /// <summary>
        /// Construct a new Label with the specified arguments. Creates a label of a specific size.
        /// </summary>
        /// <remarks>Defaults to not having a background.</remarks>
        /// <param name="name">The name of the label.</param>
        /// <param name="bounds">The bounds of the label.</param>
        /// <param name="hasBackground">Does this label have a background.</param>
        /// <param name="text">The text displayed by this label.</param>
        /// <param name="fontScriptPath">The script path to the font used by this label.</param>
        /// <param name="fontScale">The scale of the text.</param>
        /// <param name="textColor">The colour of the text.</param>
        /// <param name="parent">The parent window.</param>
        public Label(string name, Rectangle bounds, bool hasBackground, string text, string fontScriptPath, float fontScale, Color textColor, Window parent)
        {
            Initialize(name, bounds, hasBackground, text, fontScriptPath, fontScale, textColor, parent);
        }

        private void Initialize(string name, Rectangle bounds, bool hasBackground, string text, string fontScriptPath, float fontScale, Color textColor, Window parent)
        {
            _name = name;
            _parent = parent;
            _bounds = bounds;
            _hasBackground = hasBackground;
            if (_labelText == null)
            {
                _labelText = new Text(text, fontScriptPath, fontScale, this);
                _labelText.SetColour(textColor);
                _labelText.Font.IgnoreCulling = true;
                float halfFirstChar = 0f;
                if (!string.IsNullOrEmpty(text))
                {
                    if (_labelText.Font.GetAnimation(_labelText.StoredText[0].ToString(), out ComplexSprite.Anim? anim))
                    {
                        if (anim != null)
                        {
                            halfFirstChar = 2; // anim.Frames[0].Width / 2f;
                        }
                    }
                }
                _textOffset = new Vector2(-(_labelText.TextWidth / 2f - halfFirstChar), 0);
            }
            if (_hasBackground)
            {
                _background = new ComplexSprite(_parent.ScriptPath);
                _background.IgnoreCulling = true;
                _background.ChangeAnimation("LABELBACKGROUND");
                _background.SetScale(new Vector2(_bounds.Size.X / _background.CurrentFrame.Width, _bounds.Size.Y / _background.CurrentFrame.Height));
                _topEdge = new ComplexSprite(_parent.ScriptPath);
                _topEdge.IgnoreCulling = true;
                _topEdge.ChangeAnimation("LABELTOP");
                _topEdge.SetScale(new Vector2(_bounds.Size.X / _topEdge.CurrentFrame.Width, 1));
                _rightEdge = new ComplexSprite(_parent.ScriptPath);
                _rightEdge.IgnoreCulling = true;
                _rightEdge.ChangeAnimation("LABELRIGHT");
                _rightEdge.SetScale(new Vector2(1, _bounds.Size.Y / _rightEdge.CurrentFrame.Height));
                _bottomEdge = new ComplexSprite(_parent.ScriptPath);
                _bottomEdge.IgnoreCulling = true;
                _bottomEdge.ChangeAnimation("LABELBOTTOM");
                _bottomEdge.SetScale(new Vector2(_bounds.Size.X / _bottomEdge.CurrentFrame.Width, 1));
                _leftEdge = new ComplexSprite(_parent.ScriptPath);
                _leftEdge.IgnoreCulling = true;
                _leftEdge.ChangeAnimation("LABELLEFT");
                _leftEdge.SetScale(new Vector2(1, _bounds.Size.Y / _leftEdge.CurrentFrame.Height));
                _topLeftCorner = new ComplexSprite(_parent.ScriptPath);
                _topLeftCorner.IgnoreCulling = true;
                _topLeftCorner.ChangeAnimation("LABELTOPLEFT");
                _topRightCorner = new ComplexSprite(_parent.ScriptPath);
                _topRightCorner.IgnoreCulling = true;
                _topRightCorner.ChangeAnimation("LABELTOPRIGHT");
                _bottomLeftCorner = new ComplexSprite(_parent.ScriptPath);
                _bottomLeftCorner.IgnoreCulling = true;
                _bottomLeftCorner.ChangeAnimation("LABELBOTTOMLEFT");
                _bottomRightCorner = new ComplexSprite(_parent.ScriptPath);
                _bottomRightCorner.IgnoreCulling = true;
                _bottomRightCorner.ChangeAnimation("LABELBOTTOMRIGHT");
            }
        }

        /// <summary>
        /// Recalculate the size of the background for this label, if it exists.
        /// </summary>
        /// <remarks>Also recalculate the text offset.</remarks>
        public void RecalculateSize()
        {
            float halfFirstChar = 2f;
            if (_autoSized)
            {
                _bounds.Width = _labelText.TextWidth;
                _bounds.Height = _labelText.TextHeight;
                _bounds.Height = (int)(_bounds.Height * 1.5f);
                if (_bounds.Height % 2 == 1)
                    _bounds.Height += 1;
                if (!string.IsNullOrEmpty(_labelText.StoredText))
                {
                    if (_labelText.Font.GetAnimation(_labelText.StoredText[0].ToString(), out ComplexSprite.Anim? anim))
                    {
                        if (anim != null)
                        {
                            _bounds.Width += anim.Frames[0].Width;
                            halfFirstChar = 2f; // anim.Frames[0].Width / 2f;
                        }
                    }
                }
                if (_bounds.Width % 2 == 1)
                    _bounds.Width += 1;
                _background?.SetScale(new Vector2(_bounds.Size.X / _background.CurrentFrame.Width, _bounds.Size.Y / _background.CurrentFrame.Height));
                _topEdge?.SetScale(new Vector2(_bounds.Size.X / _topEdge.CurrentFrame.Width, 1));
                _rightEdge?.SetScale(new Vector2(1, _bounds.Size.Y / _rightEdge.CurrentFrame.Height));
                _bottomEdge?.SetScale(new Vector2(_bounds.Size.X / _bottomEdge.CurrentFrame.Width, 1));
                _leftEdge?.SetScale(new Vector2(1, _bounds.Size.Y / _leftEdge.CurrentFrame.Height));
            }
            _textOffset = new Vector2(-(_labelText.TextWidth / 2f - halfFirstChar), 0);

        }

        /// <summary>
        /// Check for inputs for this label.
        /// </summary>
        /// <remarks>Currently unused, here for compatability.</remarks>
        public void CheckInput()
        {
            
        }

        /// <summary>
        /// Draw this label.
        /// </summary>
        public void Draw()
        {
            if (_hasBackground)
            {
                _parent.DrawLocal(_background, new Vector2(_bounds.Center.X, _bounds.Center.Y));
                _parent.DrawLocal(_topEdge, new Vector2(_bounds.Center.X, _bounds.Top - _topEdge.CurrentFrame.Origin.Y));
                _parent.DrawLocal(_rightEdge, new Vector2(_bounds.Right + _rightEdge.CurrentFrame.Origin.X, _bounds.Center.Y));
                _parent.DrawLocal(_bottomEdge, new Vector2(_bounds.Center.X, _bounds.Bottom + _bottomEdge.CurrentFrame.Origin.Y));
                _parent.DrawLocal(_leftEdge, new Vector2(_bounds.Left - _leftEdge.CurrentFrame.Origin.X, _bounds.Center.Y));

                _parent.DrawLocal(_topLeftCorner, new Vector2(_bounds.Left - _topLeftCorner.CurrentFrame.Origin.X, _bounds.Top - _topLeftCorner.CurrentFrame.Origin.Y));
                _parent.DrawLocal(_topRightCorner, new Vector2(_bounds.Right + _topRightCorner.CurrentFrame.Origin.X, _bounds.Top - _topRightCorner.CurrentFrame.Origin.Y));
                _parent.DrawLocal(_bottomLeftCorner, new Vector2(_bounds.Left - _bottomLeftCorner.CurrentFrame.Origin.X, _bounds.Bottom + _bottomLeftCorner.CurrentFrame.Origin.Y));
                _parent.DrawLocal(_bottomRightCorner, new Vector2(_bounds.Right + _bottomRightCorner.CurrentFrame.Origin.X, _bounds.Bottom + _bottomRightCorner.CurrentFrame.Origin.Y));
            }
            _parent.DrawLocal(_labelText, _bounds.Center.ToVector2() + _textOffset);
        }

        /// <summary>
        /// Unload this label.
        /// </summary>
        public void Unload()
        {
            _background?.UnloadSprite();
            _topEdge?.UnloadSprite();
            _rightEdge?.UnloadSprite();
            _bottomEdge?.UnloadSprite();
            _leftEdge?.UnloadSprite();
            _topLeftCorner?.UnloadSprite();
            _topRightCorner?.UnloadSprite();
            _bottomLeftCorner?.UnloadSprite();
            _bottomRightCorner?.UnloadSprite();
            _labelText?.Unload();
            _parent.DeregisterControl(this);
        }

        /// <summary>
        /// Update this label.
        /// </summary>
        public void Update()
        {
            _background?.Update();
            _topEdge?.Update();
            _rightEdge?.Update();
            _bottomEdge?.Update();
            _leftEdge?.Update();
            _topLeftCorner?.Update();
            _topRightCorner?.Update();
            _bottomLeftCorner?.Update();
            _bottomRightCorner?.Update();
            _labelText?.Update();
        }


    }
}
