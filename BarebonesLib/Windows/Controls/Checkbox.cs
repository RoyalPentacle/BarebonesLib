using Barebones.Config;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Windows.Controls
{
    /// <summary>
    /// This is a checkbox control for toggling binary states.
    /// </summary>
    /// <remarks>Under the hood, this is really just an ImageButton with a little extra.</remarks>
    public class Checkbox : ImageButton
    {
        private bool _checked = false;

        /// <summary>
        /// The checked state of this checkbox.
        /// </summary>
        /// <remarks>If set, updates the display of the checkbox to match.</remarks>
        public bool Checked
        {
            get { return _checked; }
            set 
            { 
                _checked = value;
                SanityCheckAnimState();
            }
        }

        /// <summary>
        /// Construct a new checkbox from the specified arguments.
        /// </summary>
        /// <param name="name">The name of the checkbox.</param>
        /// <param name="bounds">The bounds of the checkbox.</param>
        /// <param name="parent">The parent window.</param>
        /// <param name="action">The action to perform when this checkbox is activated.</param>
        public Checkbox(string name, Rectangle bounds, Window parent, Action<Button> action) : base(name, "CHECKBOXOFF", bounds, parent, action)
        {
            
        }

        /// <summary>
        /// Check for inputs for this checkbox.
        /// </summary>
        public override void CheckInput()
        {
            if (Control.LeftClickPressed())
            {
                if (_bounds.Contains(_parent.LocalMousePosition))
                {
                    _wasClicked = true;
                    _checked = !_checked;
                    SanityCheckAnimState();
                    _action?.Invoke(this);
                }
            }
        }


        private void SanityCheckAnimState()
        {
            if (_checked)
                ChangeAnim("CHECKBOXON");
            else
                ChangeAnim("CHECKBOXOFF");
        }
    }
}
