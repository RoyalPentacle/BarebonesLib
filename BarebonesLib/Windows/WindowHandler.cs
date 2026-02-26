using Barebones.Config;
using Barebones.Windows.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
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
        internal static readonly List<Dropdown> _dropdowns = new List<Dropdown>();
        private static bool _windowClicked = false;
        private static bool _dropdownMouseOver;

        private static Window? _openFileDialog;
        private static Window? _saveFileDialog;

        internal static bool WindowClicked
        {
            get { return _windowClicked; }
        }
        internal static bool DropdownMouseover
        {
            get { return _dropdownMouseOver; }
        }

        private static Window? _forceActiveWindow;

        /// <summary>
        /// Show the OpenFileDialog.
        /// </summary>
        /// <param name="action">The action to perform when the dialog is finished.</param>
        /// <param name="path">The path to show in the textbox by default.</param>
        public static void ShowOpenFileDialog(Action<Textbox> action, string path)
        {
            if (_openFileDialog == null)
            {
                _openFileDialog = new Window("openFileDialog", "scripts/sprites/ui/windows/default.sdf", new Rectangle(Engine.Graphics.GraphicsDevice.Viewport.Width / 2 - 250, Engine.Graphics.GraphicsDevice.Viewport.Height / 2 - 25, 500, 68), "Open File", "scripts/sprites/ui/font.sdf");
                Textbox tb = new Textbox("openTextbox", new Rectangle(4, 40, 492, 24), "scripts/sprites/ui/font.sdf", 1f, Color.White, null, false, -1, _openFileDialog, action);
                _openFileDialog.RegisterControl(tb);
                tb.Text = path;
                _forceActiveWindow = _openFileDialog;
                _openFileDialog.HasMinimize = false;
            }
        }

        /// <summary>
        /// Hide the OpenFileDialog, must be invoked manually when you're done with it.
        /// </summary>
        public static void HideOpenFileDialog()
        {
            if (_openFileDialog != null)
            {
                _openFileDialog.Unload();
                _openFileDialog = null;
            }
        }

        /// <summary>
        /// Show the SaveFileDialog.
        /// </summary>
        /// <param name="action">The action to perform when the dialog is finished.</param>
        /// <param name="path">The path to show in the textbox by default.</param>
        public static void ShowSaveFileDialog(Action<Textbox> action, string path)
        {
            if (_saveFileDialog == null)
            {
                _saveFileDialog = new Window("saveFileDialog", "scripts/sprites/ui/windows/default.sdf", new Rectangle(Engine.Graphics.GraphicsDevice.Viewport.Width / 2 - 250, Engine.Graphics.GraphicsDevice.Viewport.Height / 2 - 25, 500, 68), "Save File", "scripts/sprites/ui/font.sdf");
                Textbox tb = new Textbox("saveTextbox", new Rectangle(4, 40, 492, 24), "scripts/sprites/ui/font.sdf", 1f, Color.White, null, false, -1, _saveFileDialog, action);
                _saveFileDialog.RegisterControl(tb);
                tb.Text = path;
                _forceActiveWindow = _saveFileDialog;
                _saveFileDialog.HasMinimize = false;
            }
        }

        /// <summary>
        /// Hide the SaveFileDialog, must be invoked manually when you're done with it.
        /// </summary>
        public static void HideSaveFileDialog()
        { 
            if (_saveFileDialog != null)
            {
                _saveFileDialog.Unload();
                _saveFileDialog = null;
            }    
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
            _dropdownMouseOver = false;
            for (int i = 0; i < _dropdowns.Count; i++)
            {
                if (_dropdowns[i].Active && _dropdowns[i].Bounds.Contains(_dropdowns[i].Parent.LocalMousePosition))
                {
                    _dropdownMouseOver = true;
                    break;
                }
            }

            if (_windows.Count > 0)
            {
                for (int i = 0; i < _windows.Count; i++)
                {
                    if (_windows[i].Bounds.Contains(Control.MousePosition) || i == 0)
                    {
                        _windows[i].CheckInput();
                        break;
                    }
                }
            }
            if (_forceActiveWindow != null)
            {
                _windows.Remove(_forceActiveWindow);
                _windows.Insert(0, _forceActiveWindow);
                _forceActiveWindow = null;
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
