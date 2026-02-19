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
    /// The base class for image and text buttons.
    /// </summary>
    public abstract class Button : IControl
    {
        /// <summary>
        /// The bounds of the button
        /// </summary>
        protected Rectangle _bounds;
        /// <summary>
        /// The parent window for the button
        /// </summary>
        protected Window _parent;

        /// <summary>
        /// The action to perform when the button is activated.
        /// </summary>
        protected Action<Button> _action;

        /// <summary>
        /// Is the button being hovered over.
        /// </summary>
        protected bool _isHover = false;

        /// <summary>
        /// Was the button activated, resets when the mouse is released or the mouse leaves the bounds of the button.
        /// </summary>
        protected bool _wasClicked = false;

        /// <summary>
        /// The name of the button.
        /// </summary>
        protected string _name;

        /// <summary>
        /// The parent window for this button.
        /// </summary>
        public Window Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// The name of this button
        /// </summary>
        public string Name
        {
            get { return _name; }
        }



        /// <summary>
        /// Check for inputs for this button.
        /// </summary>
        public virtual void CheckInput()
        {
            if (Control.LeftClickPressed())
            {
                if (_bounds.Contains(_parent.LocalMousePosition))
                {
                    _wasClicked = true;
                    _action?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Unload this button.
        /// </summary>
        public abstract void Unload();

        /// <summary>
        /// Update this button.
        /// </summary>
        public virtual void Update()
        {
            if (_wasClicked)
                if (!Control.LeftClickHeld())
                    _wasClicked = false;
            if (_bounds.Contains(_parent.LocalMousePosition))
            {
                _isHover = true;
            }
            else
            {
                _wasClicked = false;
                _isHover = false;
            }
        }

        /// <summary>
        /// Draw the overlay for hovering over this button.
        /// </summary>
        public virtual void Draw()
        {
            if (_wasClicked && _isHover)
            {
                Engine.SpriteBatch.Draw(Textures.Shared.Pixel, new Rectangle(_parent.Bounds.Location + _bounds.Location, _bounds.Size), new Color(50, 50, 50, 150));
            }
            else if (_isHover)
            {
                Engine.SpriteBatch.Draw(Textures.Shared.Pixel, new Rectangle(_parent.Bounds.Location + _bounds.Location, _bounds.Size), new Color(200, 200, 200, 100));
            }
        }

    }
}
