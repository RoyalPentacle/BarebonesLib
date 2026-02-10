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

namespace Barebones.Windows
{
    public class Button : IControl
    {
        private Rectangle _bounds;
        private Window _parent;
        private Action<Button> _action;
        private ComplexSprite _sprite;

        public Window Parent
        {
            get { return _parent; }
        }

        public Button(string anim, Rectangle bounds, Window parent, Action<Button> action)
        {
            _bounds = bounds;
            _parent = parent;
            _action = action;
            _sprite = new ComplexSprite(_parent.ScriptPath);
            _sprite.IgnoreCulling = true;
            _sprite.ChangeAnimation(anim);
        }
        public void ChangeAnim(string anim)
        {
            _sprite.ChangeAnimation(anim);
        }

        public void CheckInput()
        {
            if (Control.LeftClickPressed())
            {
                if (_bounds.Contains(_parent.LocalMousePosition))
                {
                    _action?.Invoke(this);
                }
            }
        }


        public void Unload()
        {
            _sprite?.UnloadSprite();
            _parent.DeregisterControl(this);
        }

        public void Update()
        {
            _sprite?.Update();
        }

        public void Draw()
        {
            _sprite?.Draw(new Vector2(_parent.Bounds.Left + _bounds.Center.X, _parent.Bounds.Top + _bounds.Center.Y));
        }

    }
}
