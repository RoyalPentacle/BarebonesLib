using Microsoft.Xna.Framework;

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

        /// <summary>
        /// The rotation of the observable thing.
        /// </summary>
        public abstract float Rotation
        {
            get;
        }

    }
}
