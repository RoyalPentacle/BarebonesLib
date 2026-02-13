using Barebones.Config;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Windows
{
    /// <summary>
    /// The handler for all window related functions.
    /// </summary>
    public static class WindowHandler
    {
        internal static readonly List<Window> _windows = new List<Window>();
        internal static Dictionary<string, Window> _windowDict = new Dictionary<string, Window>();
        private static bool _windowClicked = false;

        internal static bool WindowClicked
        {
            get { return _windowClicked; }
        }

        /// <summary>
        /// Register a new window.
        /// </summary>
        /// <remarks>Windows do this themselves when they are constructed.</remarks>
        /// <param name="window">The window to register.</param>
        public static void RegisterWindow(Window window)
        {
            if (!_windowDict.ContainsKey(window.Name))
            {
                _windowDict.Add(window.Name, window);
                _windows.Add(window);
            }
            else
            {
                Verbose.WriteErrorMajor($"WINDOW: Tried to create window with name '{window.Name}', but a window with that name already exists!");
                window.Unload();
            }
        }

        /// <summary>
        /// Deregister a window.
        /// </summary>
        /// <remarks>Windows do this themselves when <see cref="Window.Unload"/> is called.</remarks>
        /// <param name="window">The window to deregister.</param>
        public static void DeregisterWindow(Window window)
        {
            _windows.Remove(window);
            _windowDict.Remove(window.Name);
        }

        internal static void Update()
        {
            _windowClicked = false;
            for (int i = 0; i < _windows.Count; i++)
            {
                if (_windows[i].Bounds.Contains(Control.MousePosition)) // If the user clicks on a window, take the top most window clicked on and make it the active window.
                {
                    _windowClicked = true;
                    if (Control.LeftClickPressed())
                    {
                        Window win = _windows[i];
                        _windows.Remove(win);
                        _windows.Insert(0, win);
                        break;
                    }
                }
            }
            if (_windows.Count > 0)
            {
                _windows[0].CheckInput();
            }
            for (int i = _windows.Count - 1; i >= 0; i--)
            {
                _windows[i].Update();
            }
        }

        /// <summary>
        /// Draw all currently registered windows. This ignores camera settings and depth.
        /// </summary>
        public static void Draw()
        {
            Engine.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, null);
            for (int i = _windows.Count - 1; i >= 0; i--)
            {
                _windows[i].Draw();
            }
            Engine.SpriteBatch.End();
        }

    }
}
