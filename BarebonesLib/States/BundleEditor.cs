using Barebones.Asset;
using Barebones.Asset.Scripts;
using Barebones.Windows;
using Barebones.Windows.Controls;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;


namespace Barebones.States
{
    internal static class BundleEditor
    {

        private static Window? _bundleDisplay;

        private static Bundle _bundle;
        private static string _bundlePath;

        private static Listbox _bundleList;
        private static Textbox _keyTextbox;
        private static Textbox _valueTextbox;
        private static Label _keyLabel;
        private static Label _valueLabel;

        private static TextButton _removeButton;
        private static TextButton _addButton;
        private static TextButton _updateButton;

        private static string? _currentKey;


        internal static void Init()
        {
            _bundle = new Bundle();
            _bundlePath = "";
            _currentKey = null;
            if (_bundleDisplay == null)
            {


                _bundleDisplay = new Window("bundleDisplay", "scripts/sprites/ui/windows/default.sdf", new Rectangle(Engine.Graphics.GraphicsDevice.Viewport.Width / 2 - 200, Engine.Graphics.GraphicsDevice.Viewport.Height / 2 - 250, 400, 506), "Bundle Editor", "scripts/sprites/ui/font.sdf");
                _bundleDisplay.HasWindowControls = false;
                TextButton tb = new TextButton("fileButton", new Rectangle(8, 40, 80, 28), "File", "scripts/sprites/ui/font.sdf", 1f, Color.White, _bundleDisplay, ShowFileMenu);
                _bundleDisplay.RegisterControl(tb);

                _bundleList = new Listbox("bundleList", new Rectangle(8, 76, 180, 420), 15, _bundleDisplay);
                _bundleList.StretchToFit = true;
                _bundleDisplay.RegisterControl(_bundleList);
                _keyLabel = new Label("keyLabel", new Rectangle(8 + _bundleList.Bounds.Width + 8, _bundleList.Bounds.Top, 100, 28), true, "Key", "scripts/sprites/ui/font.sdf", 1f, Color.White, _bundleDisplay);
                _keyTextbox = new Textbox("keyTextbox", new Rectangle(8 + _bundleList.Bounds.Width + 8, _bundleList.Bounds.Top + 36, _bundleList.Bounds.Width, 28), "scripts/sprites/ui/font.sdf", 1f, Color.White, null, false, -1, _bundleDisplay, KeyTextboxEnter);
                _valueLabel = new Label("valueLabel", new Rectangle(8 + _bundleList.Bounds.Width + 8, _bundleList.Bounds.Top + 72, 100, 28), true, "Value", "scripts/sprites/ui/font.sdf", 1f, Color.White, _bundleDisplay);
                _valueTextbox = new Textbox("valueTextbox", new Rectangle(8 + _bundleList.Bounds.Width + 8, _bundleList.Bounds.Top + 108, _bundleList.Bounds.Width, 28), "scripts/sprites/ui/font.sdf", 1f, Color.White, null, false, -1, _bundleDisplay, ValueTextboxEnter);
                _bundleDisplay.RegisterControl(_keyLabel);
                _bundleDisplay.RegisterControl(_keyTextbox);
                _bundleDisplay.RegisterControl(_valueLabel);
                _bundleDisplay.RegisterControl(_valueTextbox);

                _addButton = new TextButton("addButton", new Rectangle(8 + _bundleList.Bounds.Width + 8, _bundleList.Bounds.Top + 144, 100, 28),"Add", "scripts/sprites/ui/font.sdf", 1f, Color.White, _bundleDisplay, AddEntry);
                _removeButton = new TextButton("removeButton", new Rectangle(8 + _bundleList.Bounds.Width + 8, _bundleList.Bounds.Top + 180, 100, 28), "Remove", "scripts/sprites/ui/font.sdf", 1f, Color.White, _bundleDisplay, RemoveEntry);
                _updateButton = new TextButton("updateButton", new Rectangle(8 + _bundleList.Bounds.Width + 8, _bundleList.Bounds.Top + 216, 100, 28), "Update", "scripts/sprites/ui/font.sdf", 1f, Color.White, _bundleDisplay, UpdateEntry);

                _bundleDisplay.RegisterControl(_addButton);
                _bundleDisplay.RegisterControl(_removeButton);
                _bundleDisplay.RegisterControl(_updateButton);


                Dropdown dropDown = new Dropdown("fileDropdown", tb, 202, _bundleDisplay);
                dropDown.AddButton(new TextButton("newBundle", Point.Zero, false, false, "New Bundle", "scripts/sprites/ui/font.sdf", 1f, Color.White, _bundleDisplay, NewBundle));
                dropDown.AddButton(new TextButton("openBundle", Point.Zero, false, false, "Open Bundle", "scripts/sprites/ui/font.sdf", 1f, Color.White, _bundleDisplay, OpenBundle));
                dropDown.AddButton(new TextButton("saveBundle", Point.Zero, false, false, "Save Bundle", "scripts/sprites/ui/font.sdf", 1f, Color.White, _bundleDisplay, SaveBundle));
                dropDown.AddButton(new TextButton("saveBundleAs", Point.Zero, false, false, "Save Bundle As", "scripts/sprites/ui/font.sdf", 1f, Color.White, _bundleDisplay, SaveBundleAs));
                dropDown.AddButton(new TextButton("exitButton", Point.Zero, false, false, "Exit", "scripts/sprites/ui/font.sdf", 1f, Color.White, _bundleDisplay, Exit));
                _bundleDisplay.RegisterControl(dropDown);
            }

        }

        private static void ShowFileMenu(Button b)
        {
            if (_bundleDisplay != null && _bundleDisplay.GetControl("fileDropdown", out Dropdown? dd))
            {
                if (dd != null)
                    dd.Active = true;
            }
        }

        private static void NewBundle(Button b)
        {
            _bundleDisplay?.Unload();
            _bundleDisplay = null;
            Init();
        }

        private static void OpenBundle(Button b)
        {
            WindowHandler.ShowOpenFileDialog(OpenFile, _bundlePath);
        }

        private static void SaveBundle(Button b)
        {
            if (!string.IsNullOrEmpty(_bundlePath))
            {
                SaveFile(_bundlePath);
            }
            else
                WindowHandler.ShowSaveFileDialog(SaveFileDelegate, _bundlePath);
        }

        private static void SaveFileDelegate(Textbox tb)
        {
            SaveFile(tb.Text);
            WindowHandler.HideSaveFileDialog();
        }

        private static void SaveBundleAs(Button b)
        {
            WindowHandler.ShowSaveFileDialog(SaveFileDelegate, _bundlePath);
        }

        private static void SaveFile(string path)
        {
            string json = JsonConvert.SerializeObject(_bundle, Formatting.Indented);
            string dir = path.Substring(0, path.LastIndexOf('/'));
            Directory.CreateDirectory(dir);
            try
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    Verbose.WriteLogMinor("Attempting to save bundle file.");
                    sw.Write(json);
                    Verbose.WriteLogMinor("Successfully saved bundle file.");
                    _bundlePath = path;
                }
            }
            catch (Exception ex)
            {
                Verbose.WriteErrorMajor($"Something has gone drastically wrong! \n EX: {ex.Message}");
            }
            
        }

        private static void OpenFile(Textbox tb)
        {
            _bundle = ScriptFinder.FindScript<Bundle>(tb.Text);
            _bundlePath = tb.Text;
            WindowHandler.HideOpenFileDialog();
            RefreshList();
        }

        private static void RefreshList()
        {
            _bundleList.RemoveAll();
            if (_bundle.Pairs != null && _bundleDisplay != null)
            {
                foreach (KeyValuePair<string, string> pair in _bundle.Pairs)
                {
                    _bundleList.AddButton(new TextButton(pair.Key, Point.Zero, false, false, pair.Key, "scripts/sprites/ui/font.sdf", 1f, Color.White, _bundleDisplay, SelectListButton));
                }
                int width = (int)(_bundleList.Bounds.Width * 2f) + 24;
                _bundleDisplay.ChangeSize(new Rectangle(Engine.Graphics.GraphicsDevice.Viewport.Width / 2 - width / 2, _bundleDisplay.Bounds.Y, width, _bundleDisplay.Bounds.Height));
                _keyTextbox.ChangeSize(new Rectangle(_bundleList.Bounds.Right + 8, _keyTextbox.Bounds.Y, _bundleList.Bounds.Width, 28));
                _valueTextbox.ChangeSize(new Rectangle(_bundleList.Bounds.Right + 8, _valueTextbox.Bounds.Y, _bundleList.Bounds.Width, 28));
                _keyLabel.ChangeSize(new Rectangle(_keyTextbox.Bounds.X, _keyLabel.Bounds.Y, _keyLabel.Bounds.Width, _keyLabel.Bounds.Height));
                _valueLabel.ChangeSize(new Rectangle(_keyTextbox.Bounds.X, _valueLabel.Bounds.Y, _valueLabel.Bounds.Width, _valueLabel.Bounds.Height));
                _addButton.ChangeSize(new Rectangle(_keyTextbox.Bounds.X, _addButton.Bounds.Y, _addButton.Bounds.Width, _addButton.Bounds.Height));
                _removeButton.ChangeSize(new Rectangle(_keyTextbox.Bounds.X, _removeButton.Bounds.Y, _removeButton.Bounds.Width, _removeButton.Bounds.Height));
                _updateButton.ChangeSize(new Rectangle(_keyTextbox.Bounds.X, _updateButton.Bounds.Y, _updateButton.Bounds.Width, _updateButton.Bounds.Height));
            }
        }

        private static void Exit(Button b)
        {
            StateHandler.ChangeState(State.Select);
        }

        private static void SelectListButton(Button b)
        {
            if (b != null && b is TextButton)
            {
                TextButton? tb = b as TextButton;
                if (tb != null)
                {
                    _currentKey = tb.Text;
                    _keyTextbox.Text = _currentKey;
                    if (_bundle != null && _bundle.Pairs != null)
                    _valueTextbox.Text = _bundle.Pairs[_currentKey];
                }
            }
        }

        private static void KeyTextboxEnter(Textbox tb)
        {

        }

        private static void ValueTextboxEnter(Textbox tb)
        {

        }

        private static void RemoveEntry(Button b)
        {
            if (_bundle != null && _bundle.Pairs != null)
            {
                if (_bundle.Pairs.Remove(_keyTextbox.Text))
                {
                    _currentKey = null;
                    RefreshList();
                }
                else
                    Verbose.WriteErrorMajor("Failed to remove key from bundle!");
            }
        }

        private static void UpdateEntry(Button b)
        {
            if (_currentKey != null && _bundle != null && _bundle.Pairs != null)
            {
                if (_bundle.Pairs.Remove(_currentKey) && _bundle.Pairs.TryAdd(_keyTextbox.Text, _valueTextbox.Text))
                {
                    _currentKey = _keyTextbox.Text;
                    RefreshList();
                }
                else
                    Verbose.WriteErrorMajor("Failed to update key/value in bundle!");
            }
            
        }

        private static void AddEntry(Button b)
        {
            if (_bundle != null && _bundle.Pairs != null)
            {
                if (_bundle.Pairs.TryAdd(_keyTextbox.Text, _valueTextbox.Text))
                {
                    _currentKey = _keyTextbox.Text;
                    RefreshList();
                }
                else
                    Verbose.WriteErrorMajor("Failed to add value to bundle!");
            }
        }

        internal static void Update()
        {

        }

        internal static void Unload()
        {
            _bundleDisplay?.Unload();
            _bundleDisplay = null;
        }

        internal static void Draw()
        {
            WindowHandler.Draw();
        }
    }
}
