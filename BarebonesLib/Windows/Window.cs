using Barebones.Asset;
using Barebones.Config;
using Barebones.Drawable;
using Barebones.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Windows
{
    /// <summary>
    /// This object is a window that can contain and display a variety of controls for user interaction.
    /// </summary>
    public class Window
    {
        private Rectangle _bounds;
        private Rectangle _handle;
        private Point _maximizedSize;

        private ComplexSprite _background;
        private ComplexSprite _topEdge;
        private ComplexSprite _rightEdge;
        private ComplexSprite _bottomEdge;
        private ComplexSprite _leftEdge;
        private ComplexSprite _topLeftCorner;
        private ComplexSprite _topRightCorner;
        private ComplexSprite _bottomLeftCorner;
        private ComplexSprite _bottomRightCorner;

        private ComplexSprite _handleBackground;
        private ComplexSprite _handleTopEdge;
        private ComplexSprite _handleRightEdge;
        private ComplexSprite _handleBottomEdge;
        private ComplexSprite _handleLeftEdge;
        private ComplexSprite _handleTopLeftCorner;
        private ComplexSprite _handleTopRightCorner;
        private ComplexSprite _handleBottomLeftCorner;
        private ComplexSprite _handleBottomRightCorner;

        private Text _title;

        private string _spriteScriptPath;

        /// <summary>
        /// The path to the spritescript used for this windows assets.
        /// </summary>
        public string ScriptPath
        {
            get { return _spriteScriptPath; }
        }

        private List<IControl> _controls;
        private Button _closeButton;
        private Button _minimizeButton;

        private bool _isMinimized = false;

        private bool _hasHandle = false;
        private bool _isDragged;
        private bool _wasDragged;

        /// <summary>
        /// The current mouse position, relative to the window.
        /// </summary>
        public Point LocalMousePosition
        {
            get { return Control.MousePosition - _bounds.Location; }
        }

        /// <summary>
        /// The bounds of the window.
        /// </summary>
        public Rectangle Bounds
        {
            get { return _bounds; }
        }

        /// <summary>
        /// Construct a new window with the specified arguments.
        /// </summary>
        /// <param name="spriteScript">The path to the SpriteScript to load for the window resources.</param>
        /// <param name="bounds">The bounds of the window</param>
        /// <param name="title">The title of the window</param>
        /// <param name="fontScriptPath">The path to the SpriteScript for the font in the window.</param>
        public Window(string spriteScript, Rectangle bounds, string title, string fontScriptPath)
        {
            _spriteScriptPath = spriteScript;
            _bounds = bounds;
            _maximizedSize = _bounds.Size;
            if (!string.IsNullOrEmpty(title))
            {
                _hasHandle = true;
                _title = new Text(title, fontScriptPath);
                _title.Font.IgnoreCulling = true;
            }

            _background = new ComplexSprite(spriteScript);
            _background.IgnoreCulling = true;
            _background.SetScale(new Vector2(Bounds.Size.X / _background.CurrentFrame.Width, Bounds.Size.Y / _background.CurrentFrame.Height));
            _topEdge = new ComplexSprite(spriteScript);
            _topEdge.IgnoreCulling = true;
            _topEdge.ChangeAnimation("TOP");
            _topEdge.SetScale(new Vector2(Bounds.Size.X / _topEdge.CurrentFrame.Width, 1));
            _rightEdge = new ComplexSprite(spriteScript);
            _rightEdge.IgnoreCulling = true;
            _rightEdge.ChangeAnimation("RIGHT");
            _rightEdge.SetScale(new Vector2(1, Bounds.Size.Y / _rightEdge.CurrentFrame.Height));
            _bottomEdge = new ComplexSprite(spriteScript);
            _bottomEdge.IgnoreCulling = true;
            _bottomEdge.ChangeAnimation("BOTTOM");
            _bottomEdge.SetScale(new Vector2(Bounds.Size.X / _bottomEdge.CurrentFrame.Width, 1));
            _leftEdge = new ComplexSprite(spriteScript);
            _leftEdge.IgnoreCulling = true;
            _leftEdge.ChangeAnimation("LEFT");
            _leftEdge.SetScale(new Vector2(1, Bounds.Size.Y / _leftEdge.CurrentFrame.Height));
            _topLeftCorner = new ComplexSprite(spriteScript);
            _topLeftCorner.IgnoreCulling = true;
            _topLeftCorner.ChangeAnimation("TOPLEFT");
            _topRightCorner = new ComplexSprite(spriteScript);
            _topRightCorner.IgnoreCulling = true;
            _topRightCorner.ChangeAnimation("TOPRIGHT");
            _bottomLeftCorner = new ComplexSprite(spriteScript);
            _bottomLeftCorner.IgnoreCulling = true;
            _bottomLeftCorner.ChangeAnimation("BOTTOMLEFT");
            _bottomRightCorner = new ComplexSprite(spriteScript);
            _bottomRightCorner.IgnoreCulling = true;
            _bottomRightCorner.ChangeAnimation("BOTTOMRIGHT");

            if (_hasHandle)
            {
                _handle = new Rectangle(0, 0, bounds.Width, 32);
                _closeButton = new Button("CLOSE", new Rectangle(bounds.Width - 31, 1, 30, 30), this, Close);
                _minimizeButton = new Button("MINIMIZE", new Rectangle(bounds.Width - 63, 1, 30, 30), this, Minimize);
                _handleBackground = new ComplexSprite(spriteScript);
                _handleBackground.IgnoreCulling = true;
                _handleBackground.ChangeAnimation("HANDLEBACKGROUND");
                _handleBackground.SetScale(new Vector2(_handle.Size.X / _handleBackground.CurrentFrame.Width, _handle.Size.Y / _handleBackground.CurrentFrame.Height));
                _handleTopEdge = new ComplexSprite(spriteScript);
                _handleTopEdge.IgnoreCulling = true;
                _handleTopEdge.ChangeAnimation("HANDLETOP");
                _handleTopEdge.SetScale(new Vector2(_handle.Size.X / _handleTopEdge.CurrentFrame.Width, 1));
                _handleRightEdge = new ComplexSprite(spriteScript);
                _handleRightEdge.IgnoreCulling = true;
                _handleRightEdge.ChangeAnimation("HANDLERIGHT");
                _handleRightEdge.SetScale(new Vector2(1, _handle.Size.Y / _handleRightEdge.CurrentFrame.Height));
                _handleBottomEdge = new ComplexSprite(spriteScript);
                _handleBottomEdge.IgnoreCulling = true;
                _handleBottomEdge.ChangeAnimation("HANDLEBOTTOM");
                _handleBottomEdge.SetScale(new Vector2(_handle.Size.X / _handleBottomEdge.CurrentFrame.Width, 1));
                _handleLeftEdge = new ComplexSprite(spriteScript);
                _handleLeftEdge.IgnoreCulling = true;
                _handleLeftEdge.ChangeAnimation("HANDLELEFT");
                _handleLeftEdge.SetScale(new Vector2(1, _handle.Size.Y / _handleLeftEdge.CurrentFrame.Height));
                _handleTopLeftCorner = new ComplexSprite(spriteScript);
                _handleTopLeftCorner.IgnoreCulling = true;
                _handleTopLeftCorner.ChangeAnimation("HANDLETOPLEFT");
                _handleTopRightCorner = new ComplexSprite(spriteScript);
                _handleTopRightCorner.IgnoreCulling = true;
                _handleTopRightCorner.ChangeAnimation("HANDLETOPRIGHT");
                _handleBottomLeftCorner = new ComplexSprite(spriteScript);
                _handleBottomLeftCorner.IgnoreCulling = true;
                _handleBottomLeftCorner.ChangeAnimation("HANDLEBOTTOMLEFT");
                _handleBottomRightCorner = new ComplexSprite(spriteScript);
                _handleBottomRightCorner.IgnoreCulling = true;
                _handleBottomRightCorner.ChangeAnimation("HANDLEBOTTOMRIGHT");
            }


            _controls = new List<IControl>();
            WindowHandler.RegisterWindow(this);
        }

        /// <summary>
        /// Constructs a new window from the specified arguments.
        /// </summary>
        /// <remarks>This creates a window with no handle. Thus, you cannot move, close or minimize the window through the handle.</remarks>
        /// <param name="spriteScript">The path to the SpriteScript to load for the window resources.</param>
        /// <param name="bounds">The bounds of the window</param>
        public Window(string spriteScript, Rectangle bounds) : this(spriteScript, bounds, string.Empty, string.Empty)
        {

        }

        /// <summary>
        /// Update the window and the controls inside.
        /// </summary>
        public void Update()
        {
            if (_hasHandle)
            {
                _handleBackground?.Update();
                _handleTopEdge?.Update();
                _handleRightEdge?.Update();
                _handleBottomEdge?.Update();
                _handleLeftEdge?.Update();
                _handleTopLeftCorner?.Update();
                _handleTopRightCorner?.Update();
                _handleBottomLeftCorner?.Update();
                _handleBottomRightCorner?.Update();
                _title?.Update();
            }

            if (!_isMinimized)
            {
                _background?.Update();
                _topEdge?.Update();
                _rightEdge?.Update();
                _bottomEdge?.Update();
                _leftEdge?.Update();
                _topLeftCorner?.Update();
                _topRightCorner?.Update();
                _bottomLeftCorner?.Update();
                _bottomRightCorner?.Update();
                for (int i = _controls.Count - 1; i >= 0; i--)
                {
                    _controls[i].Update();
                }
            }


            _closeButton?.Update();
            _minimizeButton?.Update();
        }

        /// <summary>
        /// Check inputs for the window and the controls inside. Only if this is the active window.
        /// </summary>
        public void CheckInput()
        {
            if (Control.LeftClickPressed() && _handle.Contains(LocalMousePosition))
            {
                _isDragged = true;
            }
            else if (Control.LeftClickReleased())
            {
                _isDragged = false;
            }
            if (_wasDragged && _isDragged)
            {
                _bounds.Location += Control.MouseMovement;
            }

            _minimizeButton?.CheckInput();
            _closeButton?.CheckInput();

            if (!_isMinimized && !_isDragged)
            {
                for (int i = _controls.Count - 1; i >= 0; i--)
                {
                    _controls[i].CheckInput();
                }
            }

            _wasDragged = _isDragged;
        }

        /// <summary>
        /// Register a control to this window.
        /// </summary>
        /// <param name="control"></param>
        public void RegisterControl(IControl control)
        {
            _controls.Add(control);
        }

        /// <summary>
        /// Deregister a control from this window.
        /// </summary>
        /// <param name="control"></param>
        public void DeregisterControl(IControl control)
        {
            _controls.Remove(control);
        }
        
        
        /// <summary>
        /// Minimize the window.
        /// </summary>
        /// <param name="button">The button that invoked this function.</param>
        public void Minimize(Button button)
        {
            _isMinimized = !_isMinimized;
            if (!_isMinimized)
            {
                button.ChangeAnim("MINIMIZE");
                _bounds.Size = _maximizedSize;
            }
            else
            {
                button.ChangeAnim("MAXIMIZE");
                _bounds.Size = _handle.Size;
            }

        }

        /// <summary>
        /// Close the window.
        /// </summary>
        /// <param name="button">The button that invoked this function.</param>
        public void Close(Button button)
        {
            Unload();
        }

        /// <summary>
        /// Unload this window and all its controls.
        /// </summary>
        public void Unload()
        {
            for (int i = _controls.Count - 1; i >= 0;  i--)
            {
                _controls[i].Unload();
            }
            _title?.Unload();
            _background?.UnloadSprite();
            _topEdge?.UnloadSprite();
            _rightEdge?.UnloadSprite();
            _bottomEdge?.UnloadSprite();
            _leftEdge?.UnloadSprite();
            _topLeftCorner?.UnloadSprite();
            _topRightCorner?.UnloadSprite();
            _bottomLeftCorner?.UnloadSprite();
            _bottomRightCorner?.UnloadSprite();

            _handleBackground?.UnloadSprite();
            _handleTopEdge?.UnloadSprite();
            _handleRightEdge?.UnloadSprite();
            _handleBottomEdge?.UnloadSprite();
            _handleLeftEdge?.UnloadSprite();
            _handleTopLeftCorner?.UnloadSprite();
            _handleTopRightCorner?.UnloadSprite();
            _handleBottomLeftCorner?.UnloadSprite();
            _handleBottomRightCorner?.UnloadSprite();

            WindowHandler.DeregisterWindow(this);
        }

        /// <summary>
        /// Draw this window and all its controls.
        /// </summary>
        public void Draw()
        {
            if (!_isMinimized)
            {
                _background.Draw(Bounds.Center.ToVector2());
                if (!_hasHandle)
                {
                    _topEdge.Draw(new Vector2(Bounds.Center.X, Bounds.Top - (_topEdge.CurrentFrame.Height / 2)));
                    _topLeftCorner.Draw(new Vector2(Bounds.Left - (_topLeftCorner.CurrentFrame.Width / 2), Bounds.Top - (_topLeftCorner.CurrentFrame.Height / 2)));
                    _topRightCorner.Draw(new Vector2(Bounds.Right + (_topRightCorner.CurrentFrame.Width / 2), Bounds.Top - (_topRightCorner.CurrentFrame.Height / 2)));
                }
                _rightEdge.Draw(new Vector2(Bounds.Right + (_rightEdge.CurrentFrame.Width / 2), _bounds.Center.Y));
                _bottomEdge.Draw(new Vector2(Bounds.Center.X, Bounds.Bottom + (_bottomEdge.CurrentFrame.Height / 2)));
                _leftEdge.Draw(new Vector2(Bounds.Left - (_leftEdge.CurrentFrame.Width / 2), _bounds.Center.Y));
                _bottomLeftCorner.Draw(new Vector2(Bounds.Left - (_bottomLeftCorner.CurrentFrame.Width / 2), Bounds.Bottom + (_bottomLeftCorner.CurrentFrame.Height / 2)));
                _bottomRightCorner.Draw(new Vector2(Bounds.Right + (_bottomRightCorner.CurrentFrame.Width / 2), Bounds.Bottom + (_bottomRightCorner.CurrentFrame.Height / 2)));

                for (int i = 0; i < _controls.Count; i++)
                {
                    _controls[i].Draw();
                }
            }
            if (_hasHandle)
            {
                _handleBackground.Draw(new Vector2(Bounds.Center.X, _handle.Center.Y + Bounds.Top));
                _handleTopEdge.Draw(new Vector2(Bounds.Center.X, Bounds.Top - (_handleTopEdge.CurrentFrame.Height / 2)));
                _handleRightEdge.Draw(new Vector2(Bounds.Right + (_handleRightEdge.CurrentFrame.Width / 2), _handle.Center.Y + Bounds.Top));
                _handleBottomEdge.Draw(new Vector2(Bounds.Center.X, _handle.Bottom + Bounds.Top + (_handleBottomEdge.CurrentFrame.Height / 2)));
                _handleLeftEdge.Draw(new Vector2(Bounds.Left - (_handleLeftEdge.CurrentFrame.Width / 2), _handle.Center.Y + Bounds.Top));
                _handleTopLeftCorner.Draw(new Vector2(Bounds.Left - (_handleTopLeftCorner.CurrentFrame.Width / 2), Bounds.Top - (_handleTopLeftCorner.CurrentFrame.Height / 2)));
                _handleTopRightCorner.Draw(new Vector2(Bounds.Right + (_handleTopRightCorner.CurrentFrame.Width / 2), Bounds.Top - (_handleTopRightCorner.CurrentFrame.Height / 2)));
                _handleBottomLeftCorner.Draw(new Vector2(Bounds.Left - (_handleBottomLeftCorner.CurrentFrame.Width / 2), _handle.Bottom + Bounds.Top + (_handleBottomLeftCorner.CurrentFrame.Height / 2)));
                _handleBottomRightCorner.Draw(new Vector2(Bounds.Right + (_handleBottomRightCorner.CurrentFrame.Width / 2), _handle.Bottom + Bounds.Top + (_handleBottomRightCorner.CurrentFrame.Height / 2)));
                _minimizeButton?.Draw();
                _closeButton?.Draw();
                _title.Draw(new Vector2(_bounds.Left + 12, _bounds.Top + (_handle.Height / 2)));
            }
        }
        
    }
}
