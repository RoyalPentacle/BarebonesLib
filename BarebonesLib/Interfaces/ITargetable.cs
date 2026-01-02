using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Interfaces
{
    /// <summary>
    /// Classes that implement this interface can determine their visibility for targetable things.
    /// </summary>
    public interface ITargetable : ISpatiallyObservable
    {
        /// <summary>
        /// Is this object currently targetable?
        /// </summary>
        public bool IsTargetable
        {
            get;
        }
    }
}
