using Barebones.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Drawable
{
    /// <summary>
    /// This class acts as a monitor for an AttachPoint as defined inside a ComplexSprite.
    /// </summary>
    /// <remarks>
    /// Can be provided to anything that accepts <see cref="ISpatiallyObservable"/> as an argument.
    /// </remarks>
    public class AttachPointMonitor : ISpatiallyObservable
    {
        private ComplexSprite _sprite;
        private string _attachPoint;
        private bool _inheritRotation;

        /// <summary>
        /// The position of the attach point.
        /// </summary>
        /// <remarks>
        /// Gets the position of the attach point, taking into account the rotation of the sprite if applicable.
        /// Returns Vector2.Zero if unable to get the position for any reason.
        /// </remarks>
        public Vector2 Position
        {
            get 
            { 
                if (_sprite != null)
                {
                    if (_sprite.CurrentFrame != null)
                    {
                        if (_sprite.CurrentFrame.TryGetAttachPoint(_attachPoint, out Vector2 pos))
                        {
                            Matrix rotationMatrix = Matrix.CreateRotationZ(_sprite.Rotation);
                            pos -= _sprite.CurrentFrame.Origin;
                            pos = Vector2.Transform(pos, rotationMatrix);
                            return pos + _sprite.LastPosition;
                        }
                    }
                }
                return Vector2.Zero;
            }
        }
        
        /// <summary>
        /// The current rotation of the sprite that owns the attach point.
        /// </summary>
        /// <remarks>
        /// If <see cref="_inheritRotation"/> is false, returns 0f.
        /// Don't worry about needing to account for whether or not you inherit rotation,
        /// rotating by 0 radians does nothing.
        /// </remarks>
        public float Rotation
        {
            get 
            {
                if (_sprite != null && _inheritRotation)
                    return _sprite.Rotation;
                else
                    return 0f;
            }
        }

        /// <summary>
        /// Constructs a new AttachPointMonitor from the specified arguments.
        /// </summary>
        /// <param name="sprite">The ComplexSprite to monitor.</param>
        /// <param name="attachPoint">The name of the AttachPoint to track.</param>
        /// <param name="inheritRotation">Should we pass on the rotation of the sprite we are monitoring?</param>
        public AttachPointMonitor(ComplexSprite sprite, string attachPoint, bool inheritRotation)
        {
            _sprite = sprite;
            _attachPoint = attachPoint;
            _inheritRotation = inheritRotation;
        }
    }
}
