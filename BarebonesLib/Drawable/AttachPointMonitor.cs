using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Drawable
{
    public class AttachPointMonitor
    {
        private ComplexSprite _sprite;
        private string _attachPoint;
        private bool _inheritRotation;

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
        
        public float Rotation
        {
            get 
            {
                if (_sprite != null)
                    return _sprite.Rotation;
                else
                    return 0f;
            }
        }

        public bool InheritRotation
        {
            get { return _inheritRotation; }
        }

        public AttachPointMonitor(ComplexSprite sprite, string attachPoint, bool inheritRotation)
        {
            _sprite = sprite;
            _attachPoint = attachPoint;
            _inheritRotation = inheritRotation;
        }
    }
}
