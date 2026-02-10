using Barebones.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Interfaces
{
    public interface IControl
    {

        public abstract Window Parent
        {
            get;
        }

        public abstract void ChangeAnim(string anim);

        public abstract void Update();

        public abstract void CheckInput();

        public abstract void Unload();

        public abstract void Draw();


    }
}
