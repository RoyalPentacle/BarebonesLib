using Barebones.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Config
{
    /// <summary>
    /// Contains functions for checking for player input.
    /// </summary>
    public static class Control
    {

        private static readonly ConcurrentDictionary<string, Keys> _boundControls = new ConcurrentDictionary<string, Keys>();


        private static Action<char>? _inputDelegate;

        internal static Action<char>? InputDelegate
        {
            get { return _inputDelegate; }
        }

        /// <summary>
        /// Set the input delegate to hijack the text input for the window.
        /// </summary>
        /// <param name="inputDelegate"></param>
        public static void SetInputDelegate(Action<char> inputDelegate)
        {
            _inputDelegate = inputDelegate;
        }

        /// <summary>
        /// Clear the input delegate when you no longer need text input.
        /// </summary>
        public static void ClearInputDelegate()
        {
            _inputDelegate = null;
        }

        internal static void TextInputHandler(object? sender, TextInputEventArgs e)
        {
            char c = e.Character;
            if (_inputDelegate != null)
            {
                _inputDelegate.Invoke(c);
            }
        }

        /// <summary>
        /// Is text input currently being hijacked.
        /// </summary>
        public static bool InputHijacked
        {
            get { return _inputDelegate != null; }
        }

        /// <summary>
        /// The location of the mouse relative to the game window.
        /// </summary>
        /// <remarks>Use the <see cref="Camera2D.DeprojectScreenPosition(Point)"/> function to get the position relative to the game world.</remarks>
        public static Point MousePosition
        {
            get { return Engine.NewMouseState.Position; }
        }

        /// <summary>
        /// The distance the mouse has moved since the last frame.
        /// </summary>
        public static Point MouseMovement
        {
            get { return Engine.NewMouseState.Position - Engine.OldMouseState.Position; }
        }

        /// <summary>
        /// Is a window currently blocking the mouse cursor from interacting with the game underneath?
        /// </summary>
        public static bool WindowClicked
        {
            get { return WindowHandler.WindowClicked; }
        }

        /// <summary>
        /// Loads the keybinds from the path.
        /// Uses AddOrUpdate logic to overwrite default keybinds.
        /// </summary>
        public static void LoadKeybinds(string path)
        {
            try
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    string json = sr.ReadToEnd();
                    sr.Close();
                    Dictionary<string, Keys> _boundTemp = JsonConvert.DeserializeObject<Dictionary<string, Keys>>(json) ?? new Dictionary<string, Keys>();
                    foreach (KeyValuePair<string, Keys> pair in _boundTemp)
                    {
                        _boundControls.AddOrUpdate(pair.Key, pair.Value, UpdateKey);
                    }
                }
            }
            catch (Exception ex)
            {
                Verbose.WriteErrorMajor($"Failed to load control config: {path}\n  Ex: {ex.Message}");
            }
        }
        private static Keys UpdateKey(string key, Keys value)
        {
            return value;
        }

        /// <summary>
        /// Saves the current keybinds to the specified file.
        /// </summary>
        public static void SaveKeybinds(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
                using (StreamWriter sw = File.CreateText(path))
                {
                    string json = JsonConvert.SerializeObject(_boundControls, Formatting.Indented);
                    sw.WriteLine(json);
                    Verbose.WriteLogMinor($"Successfully saved controls.");
                }
            }
            catch (Exception ex)
            {
                Verbose.WriteErrorMajor($"Failed to save control config!\n  Ex: {ex.Message}");
            }
        }

        /// <summary>
        /// Registers a new keybind to the dictionary of controls.
        /// Only do this at launch, ideally in a function that will only register default controls if no bind exists for that control.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="key"></param>
        public static void RegisterKeybind(string control, Keys key)
        {
            if (!_boundControls.TryAdd(control, key))
            {
                Verbose.WriteErrorMinor($"Failed to register keybind({control}, {key.ToString()})");
            }
        }

        /// <summary>
        /// Checks if the specified key associated with this string was pressed this frame.
        /// Only if the key was not pressed the previous frame.
        /// </summary>
        /// <param name="key">The string to check.</param>
        /// <returns>True if the key has just been pressed. False otherwise.</returns>
        public static bool KeyPressed(string key)
        {
            if (_boundControls.TryGetValue(key, out Keys value))
                return KeyPressed(value);
            else
                return false;
        }

        /// <summary>
        /// Checks if the specified key was pressed this frame.
        /// Only if the key was not pressed the previous frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key has just been pressed. False otherwise.</returns>
        public static bool KeyPressed(Keys key)
        {
            return Engine.OldKeyboardState[key] == KeyState.Up && Engine.NewKeyboardState[key] == KeyState.Down;
        }

        /// <summary>
        /// Checks if the specified key associated with this string is being held down.
        /// Specifically, is the key being pressed at all.
        /// </summary>
        /// <param name="key">The string to check.</param>
        /// <returns>True if the key is being held. False otherwise.</returns>
        public static bool KeyHeld(string key)
        {
            if (_boundControls.TryGetValue(key, out Keys value))
                return KeyHeld(value);
            else
                return false;
        }

        /// <summary>
        /// Checks if the specified key is being held down.
        /// Specifically, is the key being pressed at all.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key is being held. False otherwise.</returns>
        public static bool KeyHeld(Keys key)
        {
            return Engine.NewKeyboardState[key] == KeyState.Down;
        }

        /// <summary>
        /// Checks if the specified key associated with this string was released this frame.
        /// Only if the key was pressed the previous frame.
        /// </summary>
        /// <param name="key">The string to check.</param>
        /// <returns>True if the key was released. False otherwise.</returns>
        public static bool KeyReleased(string key)
        {
            if (_boundControls.TryGetValue(key, out Keys value))
                return KeyReleased(value);
            else
                return false;
        }

        /// <summary>
        /// Checks if the specified key was released this frame.
        /// Only if the key was pressed the previous frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was released. False otherwise.</returns>
        public static bool KeyReleased(Keys key)
        {
            return Engine.OldKeyboardState[key] == KeyState.Down && Engine.NewKeyboardState[key] == KeyState.Up;
        }

        /// <summary>
        /// Checks if the left mouse button was pressed this frame.
        /// </summary>
        /// <remarks> I'd like to replace this with something more modular like how key presses are set up, but they don't make it easy.</remarks>
        /// <returns>True if the left mouse button was just clicked. False otherwise.</returns>
        public static bool LeftClickPressed()
        {
            return Engine.OldMouseState.LeftButton == ButtonState.Released && Engine.NewMouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the left mouse button is being held.
        /// </summary>
        /// <remarks> I'd like to replace this with something more modular like how key presses are set up, but they don't make it easy.</remarks>
        /// <returns>True if the left mouse button is being held. False otherwise.</returns>
        public static bool LeftClickHeld()
        {
            return Engine.NewMouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the left mouse button was released this frame.
        /// Only if the button was pressed the previous frame.
        /// </summary>
        /// <remarks> I'd like to replace this with something more modular like how key presses are set up, but they don't make it easy.</remarks>
        /// <returns>True if the button was released. False otherwise.</returns>
        public static bool LeftClickReleased()
        {
            return Engine.OldMouseState.LeftButton == ButtonState.Pressed && Engine.NewMouseState.LeftButton == ButtonState.Released;
        }


        /// <summary>
        /// Checks if the Right mouse button was pressed this frame.
        /// </summary>
        /// <remarks> I'd like to replace this with something more modular like how key presses are set up, but they don't make it easy.</remarks>
        /// <returns>True if the Right mouse button was just clicked. False otherwise.</returns>
        public static bool RightClickPressed()
        {
            return Engine.OldMouseState.RightButton == ButtonState.Released && Engine.NewMouseState.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the Right mouse button is being held.
        /// </summary>
        /// <remarks> I'd like to replace this with something more modular like how key presses are set up, but they don't make it easy.</remarks>
        /// <returns>True if the Right mouse button is being held. False otherwise.</returns>
        public static bool RightClickHeld()
        {
            return Engine.NewMouseState.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the Right mouse button was released this frame.
        /// Only if the button was pressed the previous frame.
        /// </summary>
        /// <remarks> I'd like to replace this with something more modular like how key presses are set up, but they don't make it easy.</remarks>
        /// <returns>True if the button was released. False otherwise.</returns>
        public static bool RightClickReleased()
        {
            return Engine.OldMouseState.RightButton == ButtonState.Pressed && Engine.NewMouseState.RightButton == ButtonState.Released;
        }

        /// <summary>
        /// Checks if the mouse has scrolled down.
        /// </summary>
        /// <returns>True if the mouse has scrolled down, false otherwise.</returns>
        public static bool ScrollDown()
        {
            return Engine.OldMouseState.ScrollWheelValue > Engine.NewMouseState.ScrollWheelValue;
        }

        /// <summary>
        /// Checks if the mouse has scrolled up.
        /// </summary>
        /// <returns>True if the mouse has scrolled up, false otherwise.</returns>
        public static bool ScrollUp()
        {
            return Engine.OldMouseState.ScrollWheelValue < Engine.NewMouseState.ScrollWheelValue;
        }

    }
}
