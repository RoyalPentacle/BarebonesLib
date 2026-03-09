using Barebones.Asset;
using Barebones.Config;
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
    /// This control accepts text input.
    /// </summary>
    public class Textbox : IControl
    {
        private Window _parent;
        private string _name;

        private Rectangle _bounds;

        private Text _displayText;
        private bool _activateOnFocusLoss = false;
        private bool _loseFocusOnEnter = true;

        private ComplexSprite _background;
        private ComplexSprite _topEdge;
        private ComplexSprite _rightEdge;
        private ComplexSprite _bottomEdge;
        private ComplexSprite _leftEdge;
        private ComplexSprite _topLeftCorner;
        private ComplexSprite _topRightCorner;
        private ComplexSprite _bottomLeftCorner;
        private ComplexSprite _bottomRightCorner;

        private bool _active = false;

        private Action<Textbox> _action;

        private HashSet<char> _mask;
        private bool _maskWhitelist;

        private int _maxLength;
        private int _cursorIndex;

        /// <summary>
        /// The parent window for this Textbox.
        /// </summary>
        public Window Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// The name of this Textbox.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The bounds of this Textbox.
        /// </summary>
        public Rectangle Bounds
        {
            get { return _bounds; }
        }

        /// <summary>
        /// The text stored in this Textbox.
        /// </summary>
        /// <remarks>Setter is not very robust, needs reworking.</remarks>
        public string Text
        {
            get { return _displayText.StoredText; }
            set { 

                _displayText.ChangeText(value); 
                _cursorIndex = _displayText.OriginalText.Length;
            }
        }

        /// <summary>
        /// Should this Textbox perform its action when it loses focus?
        /// </summary>
        public bool ActivateOnFocusLoss
        {
            get { return _activateOnFocusLoss; }
            set { _activateOnFocusLoss = value; }
        }

        /// <summary>
        /// Should this Textbox lose focus when enter is pressed?
        /// </summary>
        public bool LoseFocusOnEnter
        {
            get { return _loseFocusOnEnter; }
            set { _loseFocusOnEnter = value; }
        }


        /// <summary>
        /// Construct a new textbox with the specified arguments.
        /// </summary>
        /// <param name="name">The name of the Textbox.</param>
        /// <param name="bounds">The bounds of the Textbox.</param>
        /// <param name="fontScriptPath">The script path to the font for the Textbox.</param>
        /// <param name="fontScale">The scale of the font in the Textbox.</param>
        /// <param name="fontColor">The colour of the font in the Textbox.</param>
        /// <param name="mask">A mask used for allowing or disallowing specific characters.</param>
        /// <param name="maskWhitelist">Is the mask a Whitelist? If false, it's a blacklist.</param>
        /// <param name="maxLength">How many characters can be in the text? -1 is no maximum.</param>
        /// <param name="parent">The parent window for this Textbox.</param>
        /// <param name="action">The action performed when this Textbox is activated. I.E. enter is pressed.</param>
        public Textbox(string name, Rectangle bounds, string fontScriptPath, float fontScale, Color fontColor, HashSet<char>? mask, bool maskWhitelist, int maxLength, Window parent, Action<Textbox> action)
        {
            _name = name;
            _bounds = bounds;
            _displayText = new Text("", fontScriptPath, fontScale, null);
            _displayText.Font.IgnoreCulling = true;
            _displayText.SetColour(fontColor);
            if (mask != null)
            {
                _mask = mask;
            }
            else
            {
                _mask = new HashSet<char>();
                maskWhitelist = false;
            }
            _maskWhitelist = maskWhitelist;
            _parent = parent;
            _action = action;
            _background = new ComplexSprite(_parent.ScriptPath);
            _background.IgnoreCulling = true;
            _background.ChangeAnimation("TEXTBOXBACKGROUND");
            _background.SetScale(new Vector2(_bounds.Size.X / _background.CurrentFrame.Width, _bounds.Size.Y / _background.CurrentFrame.Height));
            _topEdge = new ComplexSprite(_parent.ScriptPath);
            _topEdge.IgnoreCulling = true;
            _topEdge.ChangeAnimation("TEXTBOXTOP");
            _topEdge.SetScale(new Vector2(_bounds.Size.X / _topEdge.CurrentFrame.Width, 1));
            _rightEdge = new ComplexSprite(_parent.ScriptPath);
            _rightEdge.IgnoreCulling = true;
            _rightEdge.ChangeAnimation("TEXTBOXRIGHT");
            _rightEdge.SetScale(new Vector2(1, _bounds.Size.Y / _rightEdge.CurrentFrame.Height));
            _bottomEdge = new ComplexSprite(_parent.ScriptPath);
            _bottomEdge.IgnoreCulling = true;
            _bottomEdge.ChangeAnimation("TEXTBOXBOTTOM");
            _bottomEdge.SetScale(new Vector2(_bounds.Size.X / _bottomEdge.CurrentFrame.Width, 1));
            _leftEdge = new ComplexSprite(_parent.ScriptPath);
            _leftEdge.IgnoreCulling = true;
            _leftEdge.ChangeAnimation("TEXTBOXLEFT");
            _leftEdge.SetScale(new Vector2(1, _bounds.Size.Y / _leftEdge.CurrentFrame.Height));
            _topLeftCorner = new ComplexSprite(_parent.ScriptPath);
            _topLeftCorner.IgnoreCulling = true;
            _topLeftCorner.ChangeAnimation("TEXTBOXTOPLEFT");
            _topRightCorner = new ComplexSprite(_parent.ScriptPath);
            _topRightCorner.IgnoreCulling = true;
            _topRightCorner.ChangeAnimation("TEXTBOXTOPRIGHT");
            _bottomLeftCorner = new ComplexSprite(_parent.ScriptPath);
            _bottomLeftCorner.IgnoreCulling = true;
            _bottomLeftCorner.ChangeAnimation("TEXTBOXBOTTOMLEFT");
            _bottomRightCorner = new ComplexSprite(_parent.ScriptPath);
            _bottomRightCorner.IgnoreCulling = true;
            _bottomRightCorner.ChangeAnimation("TEXTBOXBOTTOMRIGHT");
            if (maxLength >= 0)
                _maxLength = maxLength;
            else
                _maxLength = int.MaxValue;
        }

        /// <summary>
        /// Check the input for this textbox, when should it activate.
        /// </summary>
        public void CheckInput()
        {
            if (Control.LeftClickPressed() && _bounds.Contains(_parent.LocalMousePosition))
            {
                _active = true;
                Control.SetInputDelegate(ProcessTextInput);
            }
        }
       
        /// <summary>
        /// Change the size and location of this Textbox.
        /// </summary>
        /// <param name="bounds">The new bounds of this Textbox.</param>
        public void ChangeSize(Rectangle bounds)
        {
            _bounds = bounds;
            _background.SetScale(new Vector2(_bounds.Size.X / _background.CurrentFrame.Width, _bounds.Size.Y / _background.CurrentFrame.Height));
            _topEdge.SetScale(new Vector2(_bounds.Size.X / _topEdge.CurrentFrame.Width, 1));
            _rightEdge.SetScale(new Vector2(1, _bounds.Size.Y / _rightEdge.CurrentFrame.Height));
            _bottomEdge.SetScale(new Vector2(_bounds.Size.X / _bottomEdge.CurrentFrame.Width, 1));
            _leftEdge.SetScale(new Vector2(1, _bounds.Size.Y / _leftEdge.CurrentFrame.Height));
        }

        /// <summary>
        /// Forcefully activate this textbox.
        /// </summary>
        public void Activate()
        {
            _action.Invoke(this);
        }

        private void ProcessTextInput(char c)
        {
            if (c == '\b')
            {
                if (_displayText.StoredText.Length > 0)
                {
                    if (_cursorIndex > 0)
                    { 
                        _displayText.ChangeText(_displayText.StoredText.Remove(_cursorIndex - 1, 1));
                        _cursorIndex--;
                        _cursorIndex = Math.Clamp(_cursorIndex, 0, _displayText.StoredText.Length);
                    }
                }
            }
            else if (c == 127)
            {
                if (_displayText.StoredText.Length > 0)
                {
                    if (_cursorIndex < _displayText.StoredText.Length)
                    {
                        _displayText.ChangeText(_displayText.StoredText.Remove(_cursorIndex, 1));
                    }
                }
            }
            else if (c == '\r')
            {
                _action.Invoke(this);
            }
            else
            {
                if (_displayText.StoredText.Length < _maxLength)
                {
                    if (_maskWhitelist == _mask.Contains(c))
                    {
                        if (_cursorIndex < _displayText.StoredText.Length)
                            _displayText.ChangeText(_displayText.StoredText.Insert(_cursorIndex, c.ToString()));
                        else
                            _displayText.ChangeText(_displayText.StoredText + c);
                        _cursorIndex++;
                        _cursorIndex = Math.Clamp(_cursorIndex, 0, _displayText.StoredText.Length);
                    }
                }
            }
        }
        
        /// <summary>
        /// Draw this Textbox.
        /// </summary>
        public void Draw()
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
            if (_active)
            {
                Engine.SpriteBatch.Draw(Textures.Shared.Pixel, new Rectangle(_parent.Bounds.Left + _bounds.Left, _parent.Bounds.Top + _bounds.Top, _bounds.Width, _bounds.Height), new Color(255, 255, 255, 50));
            }

            _parent.DrawLocal(_displayText, new Vector2(_bounds.Left + 4, _bounds.Center.Y));
            if (_active)
            {
                Engine.SpriteBatch.Draw(Textures.Shared.Pixel, new Rectangle(_parent.Bounds.Left + _bounds.Left + 4 + (14 * _cursorIndex), _parent.Bounds.Top + _bounds.Top, 2, _bounds.Height), Color.White);
            }
        }

        /// <summary>
        /// Unload this Textbox.
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
            _displayText?.Unload();
            _parent.DeregisterControl(this);
        }

        /// <summary>
        /// Update this Textbox.
        /// </summary>
        public void Update()
        {
            if (_active)
            {
                if (Control.LeftClickPressed() && !_bounds.Contains(_parent.LocalMousePosition))
                {
                    _active = false;
                    if (Control.InputDelegate == ProcessTextInput)
                        Control.ClearInputDelegate();
                    if (_activateOnFocusLoss)
                    {
                        _action.Invoke(this);
                    }
                }
                else if (Control.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter) && _loseFocusOnEnter)
                {
                    _active = false;
                    Control.ClearInputDelegate();
                }
                else
                {
                    if (Control.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
                    {
                        _cursorIndex--;
                        _cursorIndex = Math.Clamp(_cursorIndex, 0, _displayText.StoredText.Length);
                    }
                    else if (Control.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
                    {
                        _cursorIndex++;
                        _cursorIndex = Math.Clamp(_cursorIndex, 0, _displayText.StoredText.Length);
                    }
                }
            }
            _background?.Update();
            _topEdge?.Update();
            _rightEdge?.Update();
            _bottomEdge?.Update();
            _leftEdge?.Update();
            _topLeftCorner?.Update();
            _topRightCorner?.Update();
            _bottomLeftCorner?.Update();
            _bottomRightCorner?.Update();
            _displayText?.Update();
        }
    }
}
