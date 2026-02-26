using Barebones.Config;
using Barebones.Drawable;
using Barebones.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Windows.Controls
{
    /// <summary>
    /// A Dropdown is a collection list of TextButtons that can be shown and hidden.
    /// </summary>
    public class Dropdown : IControl
    {

        private Window _parent;
        private string _name;

        private Rectangle _bounds;

        private bool _wasActive;
        private bool _active;

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

        /// <summary>
        /// The parent window for this Dropdown
        /// </summary>
        public Window Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// The name of this Dropdown.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Is this Dropdown active?
        /// </summary>
        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        /// <summary>
        /// The bounds of this Dropdown.
        /// </summary>
        public Rectangle Bounds
        {
            get { return _bounds; }
        }


        /// <summary>
        /// Construct a new Dropdown with the specified arguments.
        /// </summary>
        /// <param name="name">The name of this Dropdown.</param>
        /// <param name="button">The button this Dropdown should be positioned for.</param>
        /// <param name="width">The width of this Dropdown.</param>
        /// <param name="parent">The parent window of this Dropdown.</param>
        public Dropdown(string name, Button button, int width, Window parent) : this(name, new Point(button.Bounds.Left, button.Bounds.Bottom), width, parent)
        {

        }

        /// <summary>
        /// Construct a new Dropdown with the specified arguments.
        /// </summary>
        /// <param name="name">The name of this Dropdown.</param>
        /// <param name="position">The position of this Dropdown.</param>
        /// <param name="width">The width of this Dropdown.</param>
        /// <param name="parent">The parent window of this Dropdown.</param>
        public Dropdown(string name, Point position, int width, Window parent)
        {
            _name = name;
            _bounds = new Rectangle(position.X, position.Y, width, 8);
            _parent = parent;
            _active = false;
            _buttons = new List<TextButton>();
            _background = new ComplexSprite(_parent.ScriptPath);
            _background.IgnoreCulling = true;
            _background.ChangeAnimation("DROPDOWNBACKGROUND");
            _background.SetScale(new Vector2(_bounds.Size.X / _background.CurrentFrame.Width, _bounds.Size.Y / _background.CurrentFrame.Height));
            _topEdge = new ComplexSprite(_parent.ScriptPath);
            _topEdge.IgnoreCulling = true;
            _topEdge.ChangeAnimation("DROPDOWNTOP");
            _topEdge.SetScale(new Vector2(_bounds.Size.X / _topEdge.CurrentFrame.Width, 1));
            _rightEdge = new ComplexSprite(_parent.ScriptPath);
            _rightEdge.IgnoreCulling = true;
            _rightEdge.ChangeAnimation("DROPDOWNRIGHT");
            _rightEdge.SetScale(new Vector2(1, _bounds.Size.Y / _rightEdge.CurrentFrame.Height));
            _bottomEdge = new ComplexSprite(_parent.ScriptPath);
            _bottomEdge.IgnoreCulling = true;
            _bottomEdge.ChangeAnimation("DROPDOWNBOTTOM");
            _bottomEdge.SetScale(new Vector2(_bounds.Size.X / _bottomEdge.CurrentFrame.Width, 1));
            _leftEdge = new ComplexSprite(_parent.ScriptPath);
            _leftEdge.IgnoreCulling = true;
            _leftEdge.ChangeAnimation("DROPDOWNLEFT");
            _leftEdge.SetScale(new Vector2(1, _bounds.Size.Y / _leftEdge.CurrentFrame.Height));
            _topLeftCorner = new ComplexSprite(_parent.ScriptPath);
            _topLeftCorner.IgnoreCulling = true;
            _topLeftCorner.ChangeAnimation("DROPDOWNTOPLEFT");
            _topRightCorner = new ComplexSprite(_parent.ScriptPath);
            _topRightCorner.IgnoreCulling = true;
            _topRightCorner.ChangeAnimation("DROPDOWNTOPRIGHT");
            _bottomLeftCorner = new ComplexSprite(_parent.ScriptPath);
            _bottomLeftCorner.IgnoreCulling = true;
            _bottomLeftCorner.ChangeAnimation("DROPDOWNBOTTOMLEFT");
            _bottomRightCorner = new ComplexSprite(_parent.ScriptPath);
            _bottomRightCorner.IgnoreCulling = true;
            _bottomRightCorner.ChangeAnimation("DROPDOWNBOTTOMRIGHT");

            WindowHandler._dropdowns.Add(this);
        }

        /// <summary>
        /// Add a TextButton to this Dropdown.
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
            _buttons.Add(button);
            RecalculateSize();
            return true;
        }

        /// <summary>
        /// Get a TextButton by name from this Dropdown.
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
        /// Remove a TextButton by name from this Dropdown.
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
        }

        /// <summary>
        /// Recalculates the height of the Dropdown, and the width of all TextButtons to match the width of the Dropdown.
        /// </summary>
        public void RecalculateSize()
        {
            _bounds.Height = 0;
            for (int i = 0; i < _buttons.Count; i++)
            {
                _bounds.Height += _buttons[i].Bounds.Height;
            }
            _background.SetScale(new Vector2(_bounds.Size.X / _background.CurrentFrame.Width, _bounds.Size.Y / _background.CurrentFrame.Height));
            _topEdge.SetScale(new Vector2(_bounds.Size.X / _topEdge.CurrentFrame.Width, 1));
            _rightEdge.SetScale(new Vector2(1, _bounds.Size.Y / _rightEdge.CurrentFrame.Height));
            _bottomEdge.SetScale(new Vector2(_bounds.Size.X / _bottomEdge.CurrentFrame.Width, 1));
            _leftEdge.SetScale(new Vector2(1, _bounds.Size.Y / _leftEdge.CurrentFrame.Height));

            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].Bounds = new Rectangle(_bounds.X, _bounds.Y + i * _buttons[i].Bounds.Height, _bounds.Width, _buttons[i].Bounds.Height);
            }
        }

        /// <summary>
        /// Check inputs for this Dropdown.
        /// </summary>
        public void CheckInput()
        {
            if (_active)
            {
                for (int i = 0; i < _buttons.Count; i++)
                {
                    _buttons[i].CheckInput();
                }
            }
        }


        /// <summary>
        /// Unload this Dropdown.
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
            WindowHandler._dropdowns.Remove(this);
        }

        /// <summary>
        /// Update this Dropdown.
        /// </summary>
        public void Update()
        {
            if (_active)
            {
                if (Control.LeftClickPressed())
                {
                    if (_wasActive)
                        _active = false;
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
                for (int i = 0; i < _buttons.Count; i++)
                {
                    _buttons[i].Update();
                }
            }
            _wasActive = _active;
        }

        /// <summary>
        /// Draw this Dropdown.
        /// </summary>
        public void Draw()
        {
            if (_active)
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
                for (int i = 0; i < _buttons.Count; i++)
                {
                    _buttons[i].Draw();
                }
            }
        }
    }
}
