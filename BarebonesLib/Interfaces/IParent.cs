using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Interfaces
{
    /// <summary>
    /// This interface allows children to point to their parent object, if applicable.
    /// </summary>
    public interface IParent
    {
        /// <summary>
        /// Forces the parent to recalculate its size, should only be invoked when the child changes size.
        /// </summary>
        public abstract void RecalculateSize();
    }
}
