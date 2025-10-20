using Barebones.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Drawable
{
    public abstract class Base
    {
        protected Texture2D _texture;


        public abstract void Update();


        public abstract void Draw(Vector2 position); // Any rectangle can be turned into a Vector2 with no loss in accuracy, but not the other way around.

    }
}
