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
    /// This is a button, but with an image acting as the visual element.
    /// </summary>
    public class ImageButton : Button
    {
        private ComplexSprite _sprite;

        /// <summary>
        /// Construct a new image button from the specified arguments, inheriting the sprite script of the parent window.
        /// </summary>
        /// <param name="name">The name of the button.</param>
        /// <param name="anim">The animation of the button.</param>
        /// <param name="bounds">The bounds of the button.</param>
        /// <param name="parent">The parent window of the button.</param>
        /// <param name="action">The action performed when the button is activated.</param>
        public ImageButton(string name, string anim, Rectangle bounds, Window parent, Action<Button> action) : this (name, parent.ScriptPath, anim, bounds, parent, action)
        {

        }

        /// <summary>
        /// Construct a new image button from the specified arguments.
        /// </summary>
        /// <param name="name">The name of the button.</param>
        /// <param name="scriptPath">The path to the sprite script for this button.</param>
        /// <param name="anim">The animation of the button.</param>
        /// <param name="bounds">The bounds of the button.</param>
        /// <param name="parent">The parent window of the button.</param>
        /// <param name="action">The action performed when the button is activated.</param>
        public ImageButton(string name, string scriptPath, string anim, Rectangle bounds, Window parent, Action<Button> action)
        {
            _name = name;
            _bounds = bounds;
            _parent = parent;
            _action = action;
            _sprite = new ComplexSprite(scriptPath);
            _sprite.IgnoreCulling = true;
            _sprite.ChangeAnimation(anim);
        }

        /// <summary>
        /// Change the animation of this button.
        /// </summary>
        /// <param name="anim">The animation to change to.</param>
        public virtual void ChangeAnim(string anim)
        {
            _sprite.ChangeAnimation(anim);
        }

        /// <summary>
        /// Unload this button.
        /// </summary>
        public override void Unload()
        {
            _sprite?.UnloadSprite();
            _parent.DeregisterControl(this);
        }

        /// <summary>
        /// Update this button.
        /// </summary>
        public override void Update()
        {
            _sprite?.Update();
            base.Update();
        }

        /// <summary>
        /// Draw this button.
        /// </summary>
        public override void Draw()
        {
            _parent.DrawLocal(_sprite, new Vector2(_bounds.Center.X, _bounds.Center.Y));
            base.Draw();
        }

    }
}
