using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Interfaces
{
    /// <summary>
    /// Classes that implement this interface can have their position observed.
    /// </summary>
    public interface ISpatiallyObservable
    {
        /// <summary>
        /// The position of the observable thing.
        /// </summary>
        public abstract Vector2 Position
        {
            get;
        }
    }
}
