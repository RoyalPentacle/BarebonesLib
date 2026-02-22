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
    /// A Listbox is a scrollable collection of TextButtons that can be dynamically changed to display a variety of things.
    /// </summary>
    public class Listbox : IControl
    {

        private Window _parent;
        private string _name;

        private Rectangle _bounds;

        private List<TextButton> _buttons;

        private ComplexSprite _background;
        private ComplexSprite _topEdge;
        private ComplexSprite _rightEdge;
        private ComplexSprite _bottomEdge;
        private ComplexSprite _leftEdge;
        private ComplexSprite _topLeftCorner;
        private ComplexSprite _topRightCorner;
        private ComplexSprite _bottomLeftCorner;
        private ComplexSprite _bottomRightCorner;

        private int _maxButtonDisplay;
        private int _displayButtonStart;
        private int _displayButtonEnd;

       

        /// <summary>
        /// The parent window for this Listbox
        /// </summary>
        public Window Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// The name of this Listbox.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Construct a new Listbox with the specified arguments.
        /// </summary>
        /// <param name="name">The name of this Listbox.</param>
        /// <param name="bounds">The bounds of this Listbox.</param>
        /// <param name="maxButtonDisplay">The maximum number of buttons that can be shown at once in this Listbox.</param>
        /// <param name="parent">The parent window of this Listbox.</param>
        public Listbox(string name, Rectangle bounds, int maxButtonDisplay, Window parent)
        {
            _name = name;
            _bounds = bounds;
            _parent = parent;
            _maxButtonDisplay = maxButtonDisplay;
            _buttons = new List<TextButton>();
            _background = new ComplexSprite(_parent.ScriptPath);
            _background.IgnoreCulling = true;
            _background.ChangeAnimation("TEXTBUTTONBACKGROUND");
            _background.SetScale(new Vector2(_bounds.Size.X / _background.CurrentFrame.Width, _bounds.Size.Y / _background.CurrentFrame.Height));
            _topEdge = new ComplexSprite(_parent.ScriptPath);
            _topEdge.IgnoreCulling = true;
            _topEdge.ChangeAnimation("TEXTBUTTONTOP");
            _topEdge.SetScale(new Vector2(_bounds.Size.X / _topEdge.CurrentFrame.Width, 1));
            _rightEdge = new ComplexSprite(_parent.ScriptPath);
            _rightEdge.IgnoreCulling = true;
            _rightEdge.ChangeAnimation("TEXTBUTTONRIGHT");
            _rightEdge.SetScale(new Vector2(1, _bounds.Size.Y / _rightEdge.CurrentFrame.Height));
            _bottomEdge = new ComplexSprite(_parent.ScriptPath);
            _bottomEdge.IgnoreCulling = true;
            _bottomEdge.ChangeAnimation("TEXTBUTTONBOTTOM");
            _bottomEdge.SetScale(new Vector2(_bounds.Size.X / _bottomEdge.CurrentFrame.Width, 1));
            _leftEdge = new ComplexSprite(_parent.ScriptPath);
            _leftEdge.IgnoreCulling = true;
            _leftEdge.ChangeAnimation("TEXTBUTTONLEFT");
            _leftEdge.SetScale(new Vector2(1, _bounds.Size.Y / _leftEdge.CurrentFrame.Height));
            _topLeftCorner = new ComplexSprite(_parent.ScriptPath);
            _topLeftCorner.IgnoreCulling = true;
            _topLeftCorner.ChangeAnimation("TEXTBUTTONTOPLEFT");
            _topRightCorner = new ComplexSprite(_parent.ScriptPath);
            _topRightCorner.IgnoreCulling = true;
            _topRightCorner.ChangeAnimation("TEXTBUTTONTOPRIGHT");
            _bottomLeftCorner = new ComplexSprite(_parent.ScriptPath);
            _bottomLeftCorner.IgnoreCulling = true;
            _bottomLeftCorner.ChangeAnimation("TEXTBUTTONBOTTOMLEFT");
            _bottomRightCorner = new ComplexSprite(_parent.ScriptPath);
            _bottomRightCorner.IgnoreCulling = true;
            _bottomRightCorner.ChangeAnimation("TEXTBUTTONBOTTOMRIGHT");
        }

        /// <summary>
        /// Add a TextButton to this Listbox.
        /// </summary>
        /// <param name="button">The button to add.</param>
        /// <returns>True if the button was added successfully, False otherwise.</returns>
        public bool AddButton(TextButton button)
        {
            foreach (TextButton b in _buttons)
            {
                if (b.Name == button.Name)
                {
                    return false;
                }
            }
            button.Bounds = new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, button.Bounds.Height);
            _buttons.Add(button);
            RecalculateSize();
            SanityCheckDisplay();
            return true;
        }

        private void SanityCheckDisplay()
        {
            _displayButtonStart = Math.Clamp(_displayButtonStart, 0, _buttons.Count - 1);
            _displayButtonEnd = _displayButtonStart + _maxButtonDisplay;
            _displayButtonEnd = Math.Clamp(_displayButtonEnd, _displayButtonStart + 1, _buttons.Count);
            for (int i = _displayButtonStart; i < _displayButtonEnd; i++)
            {
                if (i < _buttons.Count)
                    _buttons[i].Bounds = new Rectangle(_bounds.X, _bounds.Y + (i - _displayButtonStart) * _buttons[i].Bounds.Height, _bounds.Width, _buttons[i].Bounds.Height);
            }
        }

        /// <summary>
        /// Get a TextButton by name from this Listbox.
        /// </summary>
        /// <param name="name">The name of the TextButton.</param>
        /// <param name="button">The output TextButton.</param>
        /// <returns>True if the TextButton exists, False otherwise.</returns>
        public bool GetButton(string name, out TextButton? button)
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i].Name == name)
                {
                    button = _buttons[i];
                    return true;
                }
            }
            button = null;
            return false;
        }

        /// <summary>
        /// Remove a TextButton by name from this Listbox.
        /// </summary>
        /// <param name="name">The name of the button to remove.</param>
        public void RemoveButton(string name)
        {
            if (GetButton(name, out TextButton? b))
            {
                if (b != null)
                {
                    b.Unload();
                    _buttons.Remove(b);
                }
            }
            RecalculateSize();
            SanityCheckDisplay();
        }

        /// <summary>
        /// Recalculates the height of the Listbox, and the width of all TextButtons to match the width of the Listbox.
        /// </summary>
        public void RecalculateSize()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].Bounds = new Rectangle(_bounds.X, _bounds.Y + i * _buttons[i].Bounds.Height, _bounds.Width, _buttons[i].Bounds.Height);
            }
            if (_buttons.Count > 0)
            {
                _bounds.Height = _buttons[0].Bounds.Height * _maxButtonDisplay;
                _background.SetScale(new Vector2(_bounds.Size.X / _background.CurrentFrame.Width, _bounds.Size.Y / _background.CurrentFrame.Height));
                _topEdge.SetScale(new Vector2(_bounds.Size.X / _topEdge.CurrentFrame.Width, 1));
                _rightEdge.SetScale(new Vector2(1, _bounds.Size.Y / _rightEdge.CurrentFrame.Height));
                _bottomEdge.SetScale(new Vector2(_bounds.Size.X / _bottomEdge.CurrentFrame.Width, 1));
                _leftEdge.SetScale(new Vector2(1, _bounds.Size.Y / _leftEdge.CurrentFrame.Height));
            }
        }

        /// <summary>
        /// Check inputs for this Listbox.
        /// </summary>
        public void CheckInput()
        {
            for (int i = _displayButtonStart; i < _displayButtonEnd; i++)
            {
                if (i < _buttons.Count)
                    _buttons[i].CheckInput();
            }
        }


        /// <summary>
        /// Unload this Listbox.
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
            for (int i = _buttons.Count - 1; i >= 0; i--)
            {
                _buttons[i].Unload();
                _buttons.RemoveAt(i);
            }
            _parent.DeregisterControl(this);
        }

        /// <summary>
        /// Update this Listbox.
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
            if (Control.ScrollUp())
            {
                _displayButtonStart--;
                SanityCheckDisplay();
            }
            if (Control.ScrollDown())
            {
                if (_displayButtonEnd < _buttons.Count)
                {
                    _displayButtonStart++;
                    SanityCheckDisplay();
                }
            }

            for (int i = _displayButtonStart; i < _displayButtonEnd; i++)
            {
                if (i < _buttons.Count)
                    _buttons[i].Update();
            }
        }

        /// <summary>
        /// Draw this Listbox.
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
            for (int i = _displayButtonStart; i < _displayButtonEnd; i++)
            {
                if (i < _buttons.Count)
                    _buttons[i].Draw();
            }
        }
    }
}
