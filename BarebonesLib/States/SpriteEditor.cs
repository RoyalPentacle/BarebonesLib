using Barebones.Asset;
using Barebones.Asset.Scripts;
using Barebones.Config;
using Barebones.Drawable;
using Barebones.Drawable.Particles;
using Barebones.Windows;
using Barebones.Windows.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Barebones.Drawable.ComplexSprite;

namespace Barebones.States
{
    internal static class SpriteEditor
    {
        private static Window? _stripWindow;

        private static Window? _mainWindow;

        private static Window? _animationWindow;

        private static Window? _paletteWindow;

        private static Window? _attachPointWindow;

        private static Window? _luaWindow;

        private static Window? _backgroundWindow;

        private static Window? _extraSpriteWindow;

        private static string _spritePath;

        private static ComplexSprite? _background;
        private static ComplexSprite? _sprite;

        private static bool _highlightCurrent = false;
        private static bool _highlightAll = false;


        private static OrderedDictionary<string, Color> _animColors;

        private static Rectangle _framePreview;
        private static Point _originPreview;


        internal static void Init()
        {
            Engine.Camera.MoveCamera(Vector2.Zero);
            Engine.Camera.Zoom = 1f;
            _animColors?.Clear();
            _animColors = new OrderedDictionary<string, Color>();
            if (_sprite == null)
            {
                _sprite = new ComplexSprite();
                _sprite.IgnoreCulling = true;
                _sprite.IgnoreLua = true;
            }

            if (_stripWindow == null)
            {
                _stripWindow = new Window("menuStrip", "scripts/sprites/ui/windows/default.sdf", new Rectangle(8, 8, 272, 40));
                TextButton file = new TextButton("fileButton", new Rectangle(8, 6, 80, 28), "File", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, ShowFileMenu);
                _stripWindow.RegisterControl(file);
                TextButton tools = new TextButton("toolButton", new Rectangle(96, 6, 80, 28), "Tools", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, ShowToolMenu);
                _stripWindow.RegisterControl(tools);
                TextButton extra = new TextButton("extraButton", new Rectangle(184, 6, 80, 28), "Other", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, ShowExtraMenu);
                _stripWindow.RegisterControl(extra);

                Dropdown fileDD = new Dropdown("fileDropdown", file, 200, _stripWindow);
                fileDD.AddButton(new TextButton("newSDF", Point.Zero, false, false, "New SDF", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, NewSDF));
                fileDD.AddButton(new TextButton("openSDF", Point.Zero, false, false, "Open SDF", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, OpenSDF));
                fileDD.AddButton(new TextButton("saveSDF", Point.Zero, false, false, "Save SDF", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, SaveSDF));
                fileDD.AddButton(new TextButton("saveSDFAs", Point.Zero, false, false, "Save SDF As", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, SaveSDFAs));
                fileDD.AddButton(new TextButton("exit", Point.Zero, false, false, "Exit", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, Exit));

                _stripWindow.RegisterControl(fileDD);

                Dropdown toolDD = new Dropdown("toolDropdown", tools, 200, _stripWindow);
                toolDD.AddButton(new TextButton("animButton", Point.Zero, false, false, "Animations", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, ShowAnimationWindow));
                toolDD.AddButton(new TextButton("attachPointButton", Point.Zero, false, false, "Attach Points", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, ShowAttachPointWindow));
                toolDD.AddButton(new TextButton("paletteButton", Point.Zero, false, false, "Palettes", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, ShowPaletteWindow));
                toolDD.AddButton(new TextButton("luaButton", Point.Zero, false, false, "Lua", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, ShowLuaWindow));
                _stripWindow.RegisterControl(toolDD);
                

                Dropdown extraDD = new Dropdown("extraDropdown", extra, 200, _stripWindow);
                extraDD.AddButton(new TextButton("backgroundButton", Point.Zero, false, false, "Background", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, ShowBackgroundWindow));
                extraDD.AddButton(new TextButton("extraSpriteButton", Point.Zero, false, false, "Extra", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, ShowExtraSpriteWindow));
                _stripWindow.RegisterControl(extraDD);
            }
            if (_mainWindow == null)
            {
                _mainWindow = new Window("mainWindow", "scripts/sprites/ui/windows/default.sdf", new Rectangle(8, 64, 272, 206));

                _mainWindow.RegisterControl(new Label("texNameLabel", new Rectangle(4, 4, 264, 28), true, "", "scripts/sprites/ui/font.sdf", 1f, Color.White, _mainWindow));
                _mainWindow.RegisterControl(new TextButton("browseTextures", new Rectangle(4, 38, 128, 28), "Browse", "scripts/sprites/ui/font.sdf", 1f, Color.White, _mainWindow, BrowseTextures));

                Checkbox cb1 = new Checkbox("showAll", new Rectangle(2, 74, 24, 24), _mainWindow, ToggleShowFullTexture);
                cb1.Checked = true;

                _mainWindow.RegisterControl(cb1);
                _mainWindow.RegisterControl(new Label("showAllLabel", new Rectangle(32, 72, 188 ,28), true, "Show Full Tex", "scripts/sprites/ui/font.sdf", 1f, Color.White, _mainWindow));
                _mainWindow.RegisterControl(new Checkbox("pauseAnimation", new Rectangle(2, 108, 24, 24), _mainWindow, TogglePauseAnimation));
                _mainWindow.RegisterControl(new Label("pauseAnimLabel", new Rectangle(32, 106, 216, 28), true, "Pause Animation", "scripts/sprites/ui/font.sdf", 1f, Color.White, _mainWindow));
                _mainWindow.RegisterControl(new Checkbox("highlightAll", new Rectangle(2, 142, 24, 24), _mainWindow, ToggleHighlightAllAnim));
                _mainWindow.RegisterControl(new Label("highlightAllLabel", new Rectangle(32, 140, 188, 28), true, "Lit All Anims", "scripts/sprites/ui/font.sdf", 1f, Color.White, _mainWindow));
                _mainWindow.RegisterControl(new Checkbox("highlightCurrent", new Rectangle(2, 176, 24, 24), _mainWindow, ToggleHighlightCurrentAnim));
                _mainWindow.RegisterControl(new Label("highlightCurrentLabel", new Rectangle(32, 174, 232, 28), true, "Lit Current Anim", "scripts/sprites/ui/font.sdf", 1f, Color.White, _mainWindow));
            }


        }

        private static void RefreshLists()
        {
            if (WindowHandler._windowDict.ContainsKey("animWindow"))
            {
                if (_animationWindow != null)
                {
                    if (_animationWindow.GetControl("animListbox", out Listbox? lb))
                    {
                        if (lb != null)
                        {
                            lb.RemoveAll();
                            if (_sprite != null)
                            {
                                foreach (string k in _sprite.Animations.Keys)
                                {
                                    lb.AddButton(new TextButton(k, Point.Zero, false, false, k, "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow, SelectAnimation));
                                }
                                if (!_sprite.Animations.ContainsKey(_selectedAnim))
                                {
                                    _selectedAnim = _sprite.DefaultAnim;
                                    if (lb.GetButton(_selectedAnim, out TextButton? tb))
                                    {
                                        if (tb != null)
                                            lb.SelectButton(tb);
                                    }
                                }
                                _sprite.ChangeAnimation(_selectedAnim);
                            }
                            RefreshFrameList();
                        }
                    }
                }
            }

        }

        private static void RefreshFrameList()
        {
            if (WindowHandler._windowDict.ContainsKey("animWindow"))
            {
                if (_animationWindow != null)
                {
                    if (_animationWindow.GetControl("frameListbox", out Listbox? lb))
                    {
                        if (lb != null)
                        {
                            lb.RemoveAll();
                            if (_sprite != null)
                            {
                                if (_sprite.GetAnimation(_selectedAnim, out ComplexSprite.Anim? a))
                                {
                                    if (a != null)
                                    {
                                        for (int i = 0; i < a.Frames.Count; i++)
                                        {
                                            lb.AddButton(new TextButton(i.ToString(), Point.Zero, false, false, i.ToString(), "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow, SelectFrame));
                                        }
                                        if (_selectedFrame > -1 && _selectedFrame < a.Frames.Count)
                                        {
                                            RefreshFrameControls(a.Frames[_selectedFrame]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void RefreshAnimColours()
        {
            if (_sprite != null)
            {
                _animColors.Clear();
                Random r;
                foreach (string s in _sprite.Animations.Keys)
                {
                    r = new Random(s.GetHashCode());
                    _animColors.Add(s, new Color(r.Next(50, 200), r.Next(50, 200), r.Next(50, 200), 50));
                }
            }
        }

        #region Menu Strip

        private static void ShowFileMenu(Button b)
        {
            if (_stripWindow != null && _stripWindow.GetControl("fileDropdown", out Dropdown? dd))
            {
                if (dd != null)
                    dd.Active = true;
            }
        }

        private static void NewSDF(Button b)
        {
            _sprite?.UnloadSprite();
            _sprite = new ComplexSprite();
            _sprite.IgnoreCulling = true;
            _sprite.IgnoreLua = true;
            RefreshLists();
        }

        private static void OpenSDF(Button b)
        {
            WindowHandler.ShowOpenFileDialog(OpenFile, _spritePath);
        }

        private static void OpenFile(Textbox tb)
        {
            _sprite?.UnloadSprite();
            _sprite = new ComplexSprite(tb.Text, true);
            _spritePath = tb.Text;
            _sprite.IgnoreCulling = true;
            _sprite.IgnoreLua = true;
            _sprite.ShowFullTexture = true;
            _sprite.PauseAnimation = true;
            if (_mainWindow != null)
                if (_mainWindow.GetControl("showAll", out Checkbox? c))
                {
                    if (c != null)
                        c.Checked = true;
                }
            WindowHandler.HideOpenFileDialog();
            RefreshLists();
            RefreshAnimColours();
            if (_mainWindow != null)
            {
                if (_mainWindow.GetControl("texNameLabel", out Label? l))
                {
                    if (l != null)
                    {
                        if (_sprite.TexturePath.Length > 0)
                        {
                            if (_sprite.TexturePath.LastIndexOf('/') + 1 < _sprite.TexturePath.Length)
                            {
                                l.LabelText = _sprite.TexturePath.Substring(_sprite.TexturePath.LastIndexOf('/') + 1);
                            }
                            else
                                l.LabelText = _sprite.TexturePath.Substring(_sprite.TexturePath.LastIndexOf('/'));
                        }
                        else
                            l.LabelText = "";
                    }
                }
            }
            if (_animationWindow != null)
            {
                if (_animationWindow.GetControl("frameListbox", out Listbox? fLb))
                {
                    if (fLb != null)
                    {
                        fLb.SelectedIndex = 0;
                        RefreshFrameControls(_sprite.CurrentFrame);
                    }
                }
            }
        }

        private static void SaveSDF(Button b)
        {
            if (!string.IsNullOrEmpty(_spritePath))
            {
                SaveFile(_spritePath);
            }
            else
                WindowHandler.ShowSaveFileDialog(SaveFileDelegate, _spritePath);
        }

        private static void SaveSDFAs(Button b)
        {
            WindowHandler.ShowSaveFileDialog(SaveFileDelegate, _spritePath);
        }

        private static void SaveFile(string path)
        {
            if (_sprite != null)
            {
                SpriteScript script = new SpriteScript(_sprite);
                string json = JsonConvert.SerializeObject(script, Formatting.Indented);
                string dir = path.Substring(0, path.LastIndexOf('/'));
                Directory.CreateDirectory(dir);
                try
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        Verbose.WriteLogMinor("Attempting to save SDF.");
                        sw.Write(json);
                        Verbose.WriteLogMinor("Successfully saved SDF.");
                        _spritePath = path;
                    }
                }
                catch (Exception ex)
                {
                    Verbose.WriteErrorMajor($"Something has gone drastically wrong! \n EX: {ex.Message}"); 
                }
            }
        }

        private static void SaveFileDelegate(Textbox tb)
        {
            SaveFile(tb.Text);
            WindowHandler.HideSaveFileDialog();
        }

        private static void Exit(Button b)
        {
            StateHandler.ChangeState(State.Select);
        }

        
        private static void ShowToolMenu(Button b)
        {
            if (_stripWindow != null && _stripWindow.GetControl("toolDropdown", out Dropdown? dd))
            {
                if (dd != null)
                    dd.Active = true;
            }
        }

        private static void ShowAnimationWindow(Button b)
        {
            if (!WindowHandler._windowDict.ContainsKey("animWindow"))
            {
                _animationWindow?.Unload();

                _animationWindow = new Window("animWindow", "scripts/sprites/ui/windows/default.sdf", new Rectangle(8, 278, 300, 516), "Animations", "scripts/sprites/ui/font.sdf");
                _animationWindow.RegisterControl(new Label("animListboxLabel", new Rectangle(4, 38, 100, 28), true, "Anims", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow));
                Listbox animLB = new Listbox("animListbox", new Rectangle(4, 72, 150, 168), 6, _animationWindow);
                _animationWindow.RegisterControl(animLB);
                Textbox animNameTB = new Textbox("animNameTextbox", new Rectangle(160, 72, 136, 28), "scripts/sprites/ui/font.sdf", 1f, Color.White, null, false, -1, _animationWindow, NullTextboxDelegate);
                _animationWindow.RegisterControl(animNameTB);
                _animationWindow.RegisterControl(new TextButton("addAnimButton", new Rectangle(160, 106, 136, 28), "Add Anim", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow, AddAnimation));
                _animationWindow.RegisterControl(new TextButton("updateAnimButton", new Rectangle(160, 140, 136, 28), "Update", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow, UpdateAnimation));
                _animationWindow.RegisterControl(new TextButton("removeAnimButton", new Rectangle(160, 174, 136, 28), "Remove", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow, RemoveAnimation));
                _animationWindow.RegisterControl(new Label("frameListboxLabel", new Rectangle(4, 246, 100, 28), true, "Frames", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow));
                Listbox frameLB = new Listbox("frameListbox", new Rectangle(4, 280, 50, 198), 7, _animationWindow);
                frameLB.FitMaxVert = false;
                _animationWindow.RegisterControl(frameLB);

                HashSet<char> integerWhitelist = new HashSet<char> {'-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
                _animationWindow.RegisterControl(new Label("boundsLabel", new Rectangle(60, 280, 100, 28), true, "Bounds", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow));
                _animationWindow.RegisterControl(new Label("frameX", new Rectangle(60, 314, 28, 28), true, "X", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow));
                _animationWindow.RegisterControl(new Textbox("frameXTextbox", new Rectangle(94, 314, 80, 28), "scripts/sprites/ui/font.sdf", 1f, Color.White, integerWhitelist, true, 5, _animationWindow, NullTextboxDelegate));
                _animationWindow.RegisterControl(new Label("frameY", new Rectangle(180, 314, 28, 28), true, "Y", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow));
                _animationWindow.RegisterControl(new Textbox("frameYTextbox", new Rectangle(214, 314, 80, 28), "scripts/sprites/ui/font.sdf", 1f, Color.White, integerWhitelist, true, 5, _animationWindow, NullTextboxDelegate));
                _animationWindow.RegisterControl(new Label("frameW", new Rectangle(60, 348, 28, 28), true, "W", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow));
                _animationWindow.RegisterControl(new Textbox("frameWTextbox", new Rectangle(94, 348, 80, 28), "scripts/sprites/ui/font.sdf", 1f, Color.White, integerWhitelist, true, 5, _animationWindow, NullTextboxDelegate));
                _animationWindow.RegisterControl(new Label("frameH", new Rectangle(180, 348, 28, 28), true, "H", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow));
                _animationWindow.RegisterControl(new Textbox("frameHTextbox", new Rectangle(214, 348, 80, 28), "scripts/sprites/ui/font.sdf", 1f, Color.White, integerWhitelist, true, 5, _animationWindow, NullTextboxDelegate));


                HashSet<char> floatWhitelist = new HashSet<char> { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };
                _animationWindow.RegisterControl(new Label("originLabel", new Rectangle(60, 382, 100, 28), true, "Origin", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow));
                _animationWindow.RegisterControl(new Label("frameOriginX", new Rectangle(60, 416, 28, 28), true, "X", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow));
                _animationWindow.RegisterControl(new Textbox("frameOriginXTextbox", new Rectangle(94, 416, 80, 28), "scripts/sprites/ui/font.sdf", 1f, Color.White, floatWhitelist, true, 5, _animationWindow, NullTextboxDelegate));
                _animationWindow.RegisterControl(new Label("frameOriginY", new Rectangle(180, 416, 28, 28), true, "Y", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow));
                _animationWindow.RegisterControl(new Textbox("frameOriginYTextbox", new Rectangle(214, 416, 80, 28), "scripts/sprites/ui/font.sdf", 1f, Color.White, floatWhitelist, true, 5, _animationWindow, NullTextboxDelegate));


                HashSet<char> posIntegerWhitelist = new HashSet<char> {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                _animationWindow.RegisterControl(new Label("delayLabel", new Rectangle(60, 450, 100, 28), true, "Delay", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow));
                _animationWindow.RegisterControl(new Textbox("delayTextbox", new Rectangle(166, 450, 100, 28), "scripts/sprites/ui/font.sdf", 1f, Color.White, posIntegerWhitelist, true, 6, _animationWindow, NullTextboxDelegate));

                _animationWindow.RegisterControl(new TextButton("addFrameButton", new Rectangle(4, 484, 68, 28), true, true, "Add", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow, AddFrame));
                _animationWindow.RegisterControl(new TextButton("removeFrameButton", new Rectangle(78, 484, 68, 28), true, true, "Del", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow, RemoveFrame));
                _animationWindow.RegisterControl(new TextButton("insertFrameButton", new Rectangle(152, 484, 68, 28), true, true, "Ins", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow, InsertFrame));
                _animationWindow.RegisterControl(new TextButton("updateFrameButton", new Rectangle(226, 484, 68, 28), true, true, "Upd", "scripts/sprites/ui/font.sdf", 1f, Color.White, _animationWindow, UpdateFrame));

                RefreshLists();

                if (_sprite != null)
                {
                    if (animLB.GetButton(_sprite.CurrentAnimationName, out TextButton? tb))
                    {
                        if (tb != null)
                        {
                            animLB.SelectButton(tb);
                            _selectedAnim = _sprite.CurrentAnimationName;
                            animNameTB.Text = _selectedAnim;
                        }
                    }
                    frameLB.SelectedIndex = _selectedFrame;
                    RefreshFrameControls(_sprite.CurrentAnimation.Frames[_selectedFrame]);
                }
            
            }
        }
        
        private static void ShowAttachPointWindow(Button b)
        {
            if (!WindowHandler._windowDict.ContainsKey("attachPointWindow"))
            {
                _attachPointWindow?.Unload();

                _attachPointWindow = new Window("attachPointWindow", "scripts/sprites/ui/windows/default.sdf", new Rectangle(300, 4, 300, 300), "Attach Points", "scripts/sprites/ui/font.sdf");
            }
        }

        private static void ShowPaletteWindow(Button b)
        {
            if (!WindowHandler._windowDict.ContainsKey("paletteWindow"))
            {
                _paletteWindow?.Unload();

                _paletteWindow = new Window("paletteWindow", "scripts/sprites/ui/windows/default.sdf", new Rectangle(504, 4, 200, 300), "Palettes", "scripts/sprites/ui/font.sdf");
            }
        }

        private static void ShowLuaWindow(Button b)
        {
            if (!WindowHandler._windowDict.ContainsKey("luaWindow"))
            {
                _luaWindow?.Unload();

                _luaWindow = new Window("luaWindow", "scripts/sprites/ui/windows/default.sdf", new Rectangle(704, 4, 200, 300), "Lua", "scripts/sprites/ui/font.sdf");
            }
        }



        private static void ShowExtraMenu(Button b)
        {
            if (_stripWindow != null && _stripWindow.GetControl("extraDropdown", out Dropdown? dd))
            {
                if (dd != null)
                    dd.Active = true;
            }
        }

        private static void ShowBackgroundWindow(Button b)
        {
            if (!WindowHandler._windowDict.ContainsKey("backgroundWindow"))
            {
                _backgroundWindow?.Unload();

                _backgroundWindow = new Window("backgroundWindow", "scripts/sprites/ui/windows/default.sdf", new Rectangle(400, 400, 250, 300), "Background", "scripts/sprites/ui/font.sdf");
            }
        }

        private static void ShowExtraSpriteWindow(Button b)
        {
            if (!WindowHandler._windowDict.ContainsKey("extraSpriteWindow"))
            {
                _extraSpriteWindow?.Unload();

                _extraSpriteWindow = new Window("extraSpriteWindow", "scripts/sprites/ui/windows/default.sdf", new Rectangle(604, 400, 200, 300), "Extra", "scripts/sprites/ui/font.sdf");
            }
        }

        #endregion

        #region Main Window

        private static void BrowseTextures(Button b)
        {
            string s = "";
            if (_sprite != null)
                s = _sprite.TexturePath;
            WindowHandler.ShowOpenFileDialog(LoadTexture, s);
        }

        private static void LoadTexture(Textbox tb)
        {
            if (_sprite != null)
            {
                _sprite.ChangeTexture(tb.Text);
                if (_mainWindow != null)
                {
                    if (_mainWindow.GetControl("texNameLabel", out Label? l))
                    {
                        if (l != null)
                        {
                            if (_sprite.TexturePath.Length > 0)
                            {
                                if (_sprite.TexturePath.LastIndexOf('/') + 1 < _sprite.TexturePath.Length)
                                {
                                    l.LabelText = _sprite.TexturePath.Substring(_sprite.TexturePath.LastIndexOf('/') + 1);
                                }
                                else
                                    l.LabelText = _sprite.TexturePath.Substring(_sprite.TexturePath.LastIndexOf('/'));
                            }
                            else
                                l.LabelText = "";
                        }
                    }
                }
            }
            WindowHandler.HideOpenFileDialog();
        }

        private static void ToggleShowFullTexture(Button b)
        {
            if (b is Checkbox c)
            {

                    if (_sprite != null)
                        _sprite.ShowFullTexture = c.Checked;
                
            }
        }

        private static void TogglePauseAnimation(Button b)
        {
            if (b is Checkbox c)
            {

                    if (_sprite != null)
                        _sprite.PauseAnimation = c.Checked;
                
            }
        }

        private static void ToggleHighlightAllAnim(Button b)
        {
            if (b is Checkbox c)
            {

                    _highlightAll = c.Checked;
                    if (c.Checked)
                    {
                        if (_mainWindow != null)
                            if (_mainWindow.GetControl("highlightCurrent", out Checkbox? c2))
                            {
                                if (c2 != null)
                                    c2.Checked = false;
                                _highlightCurrent = false;
                            }
                    }
                
            }
        }

        private static void ToggleHighlightCurrentAnim(Button b)
        {
            if (b is Checkbox c)
            {
                _highlightCurrent = c.Checked;
                if (c.Checked)
                {
                    if (_mainWindow != null)
                        if (_mainWindow.GetControl("highlightAll", out Checkbox? c2))
                        {
                            if (c2 != null)
                                c2.Checked = false;
                            _highlightAll = false;
                        }
                }
            }
        }

        #endregion

        #region Anim Window
        private static string _selectedAnim = "";
        private static int _selectedFrame = 0;

        private static void SelectAnimation(Button b)
        {
            if (_animationWindow != null)
            {
                if (_animationWindow.GetControl("animNameTextbox", out Textbox? tb))
                {
                    if (tb != null)
                    {
                        tb.Text = b.Name;
                        if (_selectedAnim != tb.Text)
                        {
                            _selectedAnim = tb.Text;


                            RefreshFrameList();
                            if (_animationWindow.GetControl("frameListbox", out Listbox? fLb))
                            {
                                if (fLb != null)
                                {
                                    if (_sprite != null)
                                    {
                                        if (_sprite.GetAnimation(_selectedAnim, out ComplexSprite.Anim? a))
                                        {
                                            if (a != null && a.Frames.Count > 0)
                                            {
                                                RefreshFrameControls(a.Frames[0]);
                                                fLb.SelectedIndex = 0;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (_sprite != null)
                        {
                            _sprite.ChangeAnimation(_selectedAnim);
                        }
                        if (_animationWindow.GetControl("animListbox", out Listbox? lb))
                        {
                            if (lb != null)
                                lb.SelectButton(b);
                        }
                    }
                }
            }
        }

        private static void AddAnimation(Button b)
        {
            if (_sprite != null)
            {
                if (_animationWindow != null)
                {
                    if (_animationWindow.GetControl("animNameTextbox", out Textbox? tb))
                    {
                        if (tb != null)
                        {
                            if (!_sprite.Animations.ContainsKey(tb.Text))
                            {
                                ComplexSprite.Anim a = new ComplexSprite.Anim();
                                a.AddFrame(new ComplexSprite.Frame(new Rectangle(0, 0, 32, 32), 1000f, new Vector2(16, 16)));
                                _sprite.Animations.Add(tb.Text, a);
                                _selectedAnim = tb.Text;
                                _sprite.ChangeAnimation(_selectedAnim);
                                RefreshLists();
                                if (_animationWindow.GetControl("frameListbox", out Listbox? fLb))
                                {
                                    if (fLb != null)
                                    {
                                        if (_sprite != null)
                                        {
                                            if (_sprite.GetAnimation(_selectedAnim, out ComplexSprite.Anim? anim))
                                            {
                                                if (anim != null && anim.Frames.Count > 0)
                                                {
                                                    RefreshFrameControls(anim.Frames[0]);
                                                    fLb.SelectedIndex = 0;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private static void UpdateAnimation(Button b)
        {
            if (_sprite != null && _animationWindow != null)
            {
                if (_selectedAnim != null)
                {
                    if (_sprite.GetAnimation(_selectedAnim, out ComplexSprite.Anim? anim))
                    {
                        if (anim != null)
                        {
                            if (_animationWindow.GetControl("animNameTextbox", out Textbox? tb))
                            {
                                if (tb != null)
                                { 
                                    if (_sprite.Animations.Remove(_selectedAnim) && _sprite.Animations.TryAdd(tb.Text, anim))
                                    {
                                        _selectedAnim = tb.Text;
                                        RefreshLists();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private static void RemoveAnimation(Button b)
        {
            if (_sprite != null && _animationWindow != null)
            {
                if (_selectedAnim != null)
                {
                    if (_sprite.Animations.Remove(_selectedAnim))
                        RefreshLists();
                }
            }
        }

        private static void SelectFrame(Button b)
        {
            if (_animationWindow != null)
            {
                int selectedFrame = int.Parse(b.Name);

                if (_sprite != null)
                {
                    if (selectedFrame < _sprite.CurrentAnimation.Frames.Count)
                    {
                        _selectedFrame = selectedFrame;
                        _sprite.PauseAnimation = true;
                        if (_mainWindow != null)
                        {
                            if (_mainWindow.GetControl("pauseAnimation", out Checkbox? c))
                            {
                                if (c != null)
                                    c.Checked = true;
                            }
                        }
                        RefreshFrameControls(_sprite.CurrentAnimation.Frames[_selectedFrame]);
                        _sprite.ChangeFrame(_selectedFrame, false);
                        if (_animationWindow.GetControl("frameListbox", out Listbox? lb))
                        {
                            if (lb != null)
                            {
                                lb.SelectButton(b);
                            }
                        }
                    }
                }
            }
        }

        private static void AddFrame(Button b)
        {
            if (_sprite != null && _animationWindow != null)
            {
                if(_sprite.GetAnimation(_selectedAnim, out ComplexSprite.Anim? a))
                {
                    if (a != null)
                    {
                        a.AddFrame(FrameFromControls(null));
                        _selectedFrame = a.Frames.Count - 1;
                        RefreshFrameList();
                    }
                }
            }
        }
        private static void RemoveFrame(Button b)
        {
            if (_sprite != null && _animationWindow != null)
            {
                if (_sprite.GetAnimation(_selectedAnim, out ComplexSprite.Anim? a))
                {
                    if (a != null)
                    {
                        if (a.Frames.Count > 1)
                        {
                            a.Frames.RemoveAt(_selectedFrame);
                            while (_selectedFrame >= a.Frames.Count)
                                _selectedFrame--;

                            RefreshFrameList();
                        }
                    }
                }
            }
        }
        private static void InsertFrame(Button b)
        {
            if (_sprite != null && _animationWindow != null)
            {
                if (_sprite.GetAnimation(_selectedAnim, out ComplexSprite.Anim? a))
                {
                    if (a != null)
                    {
                        a.Frames.Insert(_selectedFrame + 1, FrameFromControls(null));
                        _selectedFrame++;
                        RefreshFrameList();
                    }
                }
            }
        }
        private static void UpdateFrame(Button b)
        {
            if (_sprite != null && _animationWindow != null)
            {
                if (_sprite.GetAnimation(_selectedAnim, out ComplexSprite.Anim? a))
                {
                    if (a != null)
                    {
                        if (_selectedFrame < a.Frames.Count && a.Frames.Count > 0)
                        {
                            a.Frames[_selectedFrame] = FrameFromControls(a.Frames[_selectedFrame]);
                            RefreshFrameList();
                        }
                    }
                }
            }
        }

        private static ComplexSprite.Frame FrameFromControls(ComplexSprite.Frame? frame)
        {
            if (frame == null)
                frame = new ComplexSprite.Frame();
            if (_animationWindow != null)
            {
                if (_animationWindow.GetControl("frameXTextbox", out Textbox? xTb))
                {
                    if (xTb != null)
                    {
                        if (int.TryParse(xTb.Text, out int x))
                        {
                            frame.X = x;
                        }
                    }
                }
                if (_animationWindow.GetControl("frameYTextbox", out Textbox? yTb))
                {
                    if (yTb != null)
                    {
                        if (int.TryParse(yTb.Text, out int y))
                        {
                            frame.Y = y;
                        }
                    }
                }
                if (_animationWindow.GetControl("frameWTextbox", out Textbox? wTb))
                {
                    if (wTb != null)
                    {
                        if (int.TryParse(wTb.Text, out int w))
                        {
                            frame.Width = w;
                        }
                    }
                }
                if (_animationWindow.GetControl("frameHTextbox", out Textbox? hTb))
                {
                    if (hTb != null)
                    {
                        if (int.TryParse(hTb.Text, out int h))
                        {
                            frame.Height = h;
                        }
                    }
                }
                if (_animationWindow.GetControl("frameOriginXTextbox", out Textbox? oXTb))
                {
                    if (oXTb != null)
                    {
                        if (float.TryParse(oXTb.Text, out float oX))
                        {
                            frame.OriginX = oX;
                        }
                    }
                }
                if (_animationWindow.GetControl("frameOriginYTextbox", out Textbox? oYTb))
                {
                    if (oYTb != null)
                    {
                        if (float.TryParse(oYTb.Text, out float oY))
                        {
                            frame.OriginY = oY;
                        }
                    }
                }
                if (_animationWindow.GetControl("delayTextbox", out Textbox? dTb))
                {
                    if (dTb != null)
                    {
                        if (float.TryParse(dTb.Text, out float d))
                        {
                            frame.Speed = d;
                        }
                    }
                }
            }


            return frame;
        }

        private static void RefreshFrameControls(ComplexSprite.Frame frame)
        {
            if (_animationWindow != null)
            {
                if (_sprite != null)
                {
                    if (_animationWindow.GetControl("frameXTextbox", out Textbox? xTb))
                    {
                        if (xTb != null)
                        {
                            xTb.Text = frame.SourceRec.X.ToString();
                        }
                    }
                    if (_animationWindow.GetControl("frameYTextbox", out Textbox? yTb))
                    {
                        if (yTb != null)
                        {
                            yTb.Text = frame.SourceRec.Y.ToString();
                        }
                    }
                    if (_animationWindow.GetControl("frameWTextbox", out Textbox? wTb))
                    {
                        if (wTb != null)
                        {
                            wTb.Text = frame.SourceRec.Width.ToString();
                        }
                    }
                    if (_animationWindow.GetControl("frameHTextbox", out Textbox? hTb))
                    {
                        if (hTb != null)
                        {
                            hTb.Text = frame.SourceRec.Height.ToString();   
                        }
                    }
                    if (_animationWindow.GetControl("frameOriginXTextbox", out Textbox? oXtb))
                    {
                        if (oXtb != null)
                        {
                            oXtb.Text = frame.Origin.X.ToString();
                        }
                    }
                    if (_animationWindow.GetControl("frameOriginYTextbox", out Textbox? oYtb))
                    {
                        if (oYtb != null)
                        {
                            oYtb.Text = frame.Origin.Y.ToString();
                        }
                    }
                    if (_animationWindow.GetControl("delayTextbox", out Textbox? dTb))
                    {
                        if (dTb != null)
                        {
                            dTb.Text = frame.Speed.ToString();
                        }
                    }
                }
            }
        }


        #endregion

        private static void NullTextboxDelegate(Textbox tb)
        {

        }

        internal static void Update()
        {
            _sprite?.Update();
            if (!Control.WindowMousedOver)
            {
                if (Control.ScrollUp())
                {
                    Engine.Camera.Zoom += 0.1f;
                }
                if (Control.ScrollDown())
                {
                    Engine.Camera.Zoom -= 0.1f;
                    if (Engine.Camera.Zoom <= 0)
                        Engine.Camera.Zoom = 0.1f;
                }
                if (Control.LeftClickHeld() && (Control.KeyHeld(Microsoft.Xna.Framework.Input.Keys.LeftControl) || Control.KeyHeld(Microsoft.Xna.Framework.Input.Keys.RightControl)))
                {
                    Engine.Camera.MoveCamera(Engine.Camera.Position - (Control.MouseMovement.ToVector2() / Engine.Camera.Zoom));
                }
                else
                {
                    if (Control.KeyHeld(Microsoft.Xna.Framework.Input.Keys.LeftShift) || Control.KeyHeld(Microsoft.Xna.Framework.Input.Keys.RightShift))
                    {
                        if (Control.LeftClickHeld())
                        {
                            if (_sprite != null && _animationWindow != null)
                            {
                                if (_animationWindow.GetControl("frameXTextbox", out Textbox? xTb))
                                {
                                    if (xTb != null)
                                    {
                                        xTb.Text = ((int)(Engine.Camera.DeprojectScreenPosition(Control.MousePosition).X) + _sprite.TextureWidth / 2).ToString();
                                    }
                                }
                                if (_animationWindow.GetControl("frameYTextbox", out Textbox? yTb))
                                {
                                    if (yTb != null)
                                    {
                                        yTb.Text = ((int)(Engine.Camera.DeprojectScreenPosition(Control.MousePosition).Y + _sprite.TextureHeight / 2)).ToString();
                                    }
                                }
                            }
                        }
                        else if (Control.RightClickHeld())
                        {
                            if (_sprite != null && _animationWindow != null)
                            {
                                if (_animationWindow.GetControl("frameWTextbox", out Textbox? wTb))
                                {
                                    if (wTb != null)
                                    {
                                        wTb.Text = ((int)(Engine.Camera.DeprojectScreenPosition(Control.MousePosition).X) - _framePreview.X).ToString();
                                    }
                                }
                                if (_animationWindow.GetControl("frameHTextbox", out Textbox? hTb))
                                {
                                    if (hTb != null)
                                    {
                                        hTb.Text = ((int)(Engine.Camera.DeprojectScreenPosition(Control.MousePosition).Y - _framePreview.Y)).ToString();
                                    }
                                }
                            }
                        }
                        else if (Control.MiddleClickHeld())
                        {
                            if (_sprite != null && _animationWindow != null)
                            {
                                if (_animationWindow.GetControl("frameOriginXTextbox", out Textbox? oXTb))
                                {
                                    if (oXTb != null)
                                    {
                                        oXTb.Text = ((int)(Engine.Camera.DeprojectScreenPosition(Control.MousePosition).X) - _framePreview.X).ToString();
                                    }
                                }
                                if (_animationWindow.GetControl("frameOriginYTextbox", out Textbox? oYTb))
                                {
                                    if (oYTb != null)
                                    {
                                        oYTb.Text = ((int)(Engine.Camera.DeprojectScreenPosition(Control.MousePosition).Y - _framePreview.Y)).ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (_animationWindow != null)
            {
                if (_sprite != null)
                {
                    if (_animationWindow.GetControl("frameXTextbox", out Textbox? xTb))
                    {
                        if (xTb != null)
                        {
                            if (int.TryParse(xTb.Text, out int x))
                            {
                                _framePreview.X = x - _sprite.TextureWidth / 2;
                            }
                        }
                    }
                    if (_animationWindow.GetControl("frameYTextbox", out Textbox? yTb))
                    {
                        if (yTb != null)
                        {
                            if (int.TryParse(yTb.Text, out int y))
                            {
                                _framePreview.Y = y - _sprite.TextureHeight / 2;
                            }
                        }
                    }
                    if (_animationWindow.GetControl("frameWTextbox", out Textbox? wTb))
                    {
                        if (wTb != null)
                        {
                            if (int.TryParse(wTb.Text, out int w))
                            {
                                _framePreview.Width = w;
                            }
                        }
                    }
                    if (_animationWindow.GetControl("frameHTextbox", out Textbox? hTb))
                    {
                        if (hTb != null)
                        {
                            if (int.TryParse(hTb.Text, out int h))
                            {
                                _framePreview.Height = h;
                            }
                        }
                    }
                    if (_animationWindow.GetControl("frameOriginXTextbox", out Textbox? oXTb))
                    {
                        if (oXTb != null)
                        {
                            if (float.TryParse(oXTb.Text, out float oX))
                            {
                                _originPreview.X = (int)oX + _framePreview.Left;
                            }
                        }
                    }
                    if (_animationWindow.GetControl("frameOriginYTextbox", out Textbox? oYTb))
                    {
                        if (oYTb != null)
                        {
                            if (float.TryParse(oYTb.Text, out float oY))
                            {
                                _originPreview.Y = (int)oY + _framePreview.Top;
                            }
                        }
                    }
                }
            }
        }

        internal static void Unload()
        {
            _background?.UnloadSprite();
            _sprite?.UnloadSprite();
            _stripWindow?.Unload();
            _mainWindow?.Unload();
            _animationWindow?.Unload();
            _attachPointWindow?.Unload();
            _paletteWindow?.Unload();
            _backgroundWindow?.Unload();
            _extraSpriteWindow?.Unload();
            _luaWindow?.Unload();
            _background = null;
            _sprite = null;
            _stripWindow = null;
            _mainWindow = null;
            _animationWindow = null;
            _attachPointWindow = null;
            _paletteWindow = null;
            _backgroundWindow = null;
            _extraSpriteWindow = null;
            _luaWindow = null;
            Engine.SetBackbufferColour(Color.Black);
        }

        internal static void DrawBox(Rectangle bounds, int thickness, Color color)
        {
            Engine.SpriteBatch.Draw(Textures.Shared.Pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, thickness), color);
            Engine.SpriteBatch.Draw(Textures.Shared.Pixel, new Rectangle(bounds.Right - thickness, bounds.Y, thickness, bounds.Height), color);
            Engine.SpriteBatch.Draw(Textures.Shared.Pixel, new Rectangle(bounds.X, bounds.Y, thickness, bounds.Height), color);
            Engine.SpriteBatch.Draw(Textures.Shared.Pixel, new Rectangle(bounds.X, bounds.Bottom - thickness, bounds.Width, thickness), color);
        }

        internal static void DrawCross(Point point, int size, Color color)
        {
            Engine.SpriteBatch.Draw(Textures.Shared.Pixel, new Rectangle(point.X - size / 2, point.Y - size * 4, size, size * 8), color);
            Engine.SpriteBatch.Draw(Textures.Shared.Pixel, new Rectangle(point.X - size * 4, point.Y - size / 2, size * 8, size), color);
        }

        internal static void Draw()
        {
            Engine.Graphics.GraphicsDevice.Clear(Engine.BackBufferColour);

            Engine.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Engine.Camera.Transform);

            _background?.Draw(Vector2.Zero);
            if (_sprite != null)
            {
                _sprite.Draw(Vector2.Zero);
                if (_sprite.ShowFullTexture)
                {
                    if (_highlightAll)
                    {
                        for (int i = 0; i < _animColors.Count; i++)
                        {
                            Color c = _animColors.GetAt(i).Value;
                            string s = _animColors.GetAt(i).Key;

                            if (_sprite.Animations.TryGetValue(s, out ComplexSprite.Anim? a))
                            {
                                if (a != null)
                                {
                                    for (int j = 0; j < a.Frames.Count; j++)
                                    {
                                        Engine.SpriteBatch.Draw(Textures.Shared.Pixel, new Rectangle(a.Frames[j].SourceRec.Location - new Point(_sprite.TextureWidth / 2, _sprite.TextureHeight / 2), a.Frames[j].SourceRec.Size), c);
                                    }
                                }
                            }
                        }
                    }
                    
                    else if (_highlightCurrent)
                    {
                        if (_sprite.CurrentAnimation != null)
                        {
                            string s = _sprite.CurrentAnimationName;
                            if (_animColors.TryGetValue(s, out Color c))
                            {
                                for (int i = 0; i < _sprite.CurrentAnimation.Frames.Count; i++)
                                {
                                    Engine.SpriteBatch.Draw(Textures.Shared.Pixel, new Rectangle(_sprite.CurrentAnimation.Frames[i].SourceRec.Location - new Point(_sprite.TextureWidth / 2, _sprite.TextureHeight / 2), _sprite.CurrentAnimation.Frames[i].SourceRec.Size), c);
                                }
                            }
                        }
                    }
                    
                    if (_sprite.CurrentFrame != null)
                    {
                        DrawBox(new Rectangle(_sprite.CurrentFrame.SourceRec.Location - new Point(_sprite.TextureWidth / 2, _sprite.TextureHeight / 2), _sprite.CurrentFrame.SourceRec.Size), 1, Color.White);
                        Point p = _sprite.CurrentFrame.Origin.ToPoint();
                        p.X -= _sprite.TextureWidth / 2;
                        p.Y -= _sprite.TextureHeight / 2;
                        p += _sprite.CurrentFrame.SourceRec.Location;
                        DrawCross(p, 2, Color.White);
                    }
                    DrawBox(_framePreview, 2, new Color(255, 255, 255, 100));
                    DrawCross(_originPreview, 2, new Color(255, 255, 255, 100));
                }
            }

            Engine.SpriteBatch.End();

            WindowHandler.Draw();
        }

    }
}
