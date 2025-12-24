using Barebones.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barebones
{
    /// <summary>
    /// An object that handles the camera for the 2d gamespace.
    /// Access this from Engine.Camera
    /// </summary>
    public class Camera2D
    {
        private float _zoom;
        private Vector2 _position;
        private Vector2 _previousPosition;
        private Rectangle _visibleArea;
        private Matrix _transform;
        private ISpatiallyObservable? _observedPosition;
        private ISpatiallyObservable? _previouslyObservedPosition;
        private Matrix _inverseViewMatrix;

        private static Camera2D _camera = new Camera2D();

        internal static Camera2D Camera
        {
            get { return _camera; }
        }

        /// <summary>
        /// The zoom level of the camera.
        /// Clamped between 0f and 2.0f, values above 2.0f currently causes rendering issues.
        /// Also updates the transformation matrix when set.
        /// </summary>
        public float Zoom
        {
            get { return _zoom; }
            set 
            { 
                _zoom = Math.Clamp(value, 0f, 2.0f);
                UpdateMatrix();
            }
        }

        /// <summary>
        /// The current position of the camera.
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
        }

        /// <summary>
        /// The bounds of the viewport from the game.
        /// </summary>
        public Rectangle Bounds
        {
            get { return Engine.Game.GraphicsDevice.Viewport.Bounds; }
        }

        /// <summary>
        /// A rectangle that represents the section of the gamespace that is currently visible.
        /// Test intersection with this to determine if something is on screen.
        /// </summary>
        public Rectangle VisibleArea
        {
            get { return _visibleArea; }
        }

        /// <summary>
        /// The Matrix to transform the spritebatch with, to move and zoom the camera.
        /// </summary>
        public Matrix Transform
        {
            get { return _transform; }
        }

        /// <summary>
        /// Bind the camera to an object that implements ISpatiallyObservable.
        /// Stores the current target if there is one.
        /// </summary>
        /// <param name="target">The target to observe</param>
        public void BindCamera(ISpatiallyObservable target)
        {
            if (_observedPosition != null)
                _previouslyObservedPosition = _observedPosition;
            _observedPosition = target;
            _position = target.Position;
            UpdateMatrix();
        }

        /// <summary>
        /// Unbinds the camera from its target.
        /// Stores the current target if there is one.
        /// </summary>
        public void UnbindCamera()
        {
            if (_observedPosition != null)
                _previouslyObservedPosition = _observedPosition;
            _observedPosition = null;
        }

        private Camera2D()
        {
            _zoom = 1f;
            _position = new Vector2(0, 0);
        }

        private void UpdateVisibleArea()
        {
            _inverseViewMatrix = Matrix.Invert(_transform);
            Vector2 tl = Vector2.Transform(Vector2.Zero, _inverseViewMatrix);
            Vector2 tr = Vector2.Transform(new Vector2(Bounds.X, 0), _inverseViewMatrix);
            Vector2 bl = Vector2.Transform(new Vector2(0, Bounds.Y), _inverseViewMatrix);
            Vector2 br = Vector2.Transform(new Vector2(Bounds.Width, Bounds.Height), _inverseViewMatrix);

            Vector2 min = new Vector2(Math.Min(tl.X, Math.Min(tr.X, Math.Min(bl.X, br.X))), Math.Min(tl.Y, Math.Min(tr.Y, Math.Min(bl.Y, br.Y))));
            Vector2 max = new Vector2(Math.Max(tl.X, Math.Max(tr.X, Math.Max(bl.X, br.X))), Math.Max(tl.Y, Math.Max(tr.Y, Math.Max(bl.Y, br.Y))));

            _visibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        private void UpdateMatrix()
        {
            _transform = Matrix.CreateTranslation(new Vector3(-_position.X, -_position.Y, 0)) * Matrix.CreateScale(_zoom) * Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0));
            UpdateVisibleArea();
        }

        /// <summary>
        /// Updates the camera, if the position has changed, recalcuates the transformation matrix and visible area.
        /// </summary>
        public void UpdateCamera()
        {
            if (_observedPosition != null)
            {
                _position = _observedPosition.Position;
            }
            if (_previousPosition != _position)
                UpdateMatrix();
            _previousPosition = _position;
        }

        /// <summary>
        /// Moves the camera to the specified position.
        /// Unbinds the camera if it is currently bound.
        /// </summary>
        /// <param name="position">The position to move the camera to.</param>
        public void MoveCamera(Vector2 position)
        {
            UnbindCamera();
            _position = position;
        }

        /// <summary>
        /// Translates a screen position to a world position based on the current camera transformation.
        /// </summary>
        /// <param name="position">The screenspace position to deproject.</param>
        /// <returns>The worldspace equivalent position.</returns>
        public Vector2 DeprojectScreenPosition(Vector2 position)
        {
            return Vector2.Transform(position, _inverseViewMatrix);
        }

        /// <summary>
        /// Translates a screen position to a world position based on the current camera transformation.
        /// </summary>
        /// <param name="position">The screenspace position to deproject.</param>
        /// <returns>The worldspace equivablent position.</returns>
        public Vector2 DeprojectScreenPosition(Point position)
        {
            return Vector2.Transform(new Vector2(position.X, position.Y), _inverseViewMatrix);
        }

    }
}
