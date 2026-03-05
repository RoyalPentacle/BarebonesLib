using Barebones.Config;
using Barebones.Drawable;
using Barebones.Drawable.Particles;
using Barebones.Windows;
using Barebones.Windows.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.States
{
    internal static class SpriteEditor
    {
        private static Window? _stripWindow;

        private static Window? _mainWindow;

        private static Window? _frameWindow;

        private static Window? _paletteWindow;


        private static ComplexSprite? _background;
        private static ComplexSprite? _sprite;

        private static Color _bufferColor = Color.Black;


        internal static void Init()
        {
            Engine.Camera.MoveCamera(Vector2.Zero);
            if (_sprite == null)
            {
                _sprite = new ComplexSprite();
                _sprite.IgnoreCulling = true;
                _sprite.ChangeTexture("textures/actors/cid.png");
            }

            if (_stripWindow == null)
            {
                _stripWindow = new Window("menuStrip", "scripts/sprites/ui/windows/default.sdf", new Rectangle(8, 8, 272, 40));
                TextButton file = new TextButton("fileButton", new Rectangle(8, 6, 80, 28), "File", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, ShowFileMenu);
                _stripWindow.RegisterControl(file);
                TextButton tools = new TextButton("toolButton", new Rectangle(96, 6, 80, 28), "Tools", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, ShowToolMenu);
                _stripWindow.RegisterControl(tools);
                TextButton extra = new TextButton("extraButton", new Rectangle(184, 6, 80, 28), "Extra", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, ShowExtraMenu);
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
                //toolDD.AddButton(new TextButton(""))
                _stripWindow.RegisterControl(toolDD);
                

                Dropdown extraDD = new Dropdown("extraDropdown", extra, 200, _stripWindow);
                extraDD.AddButton(new TextButton("backgroundButton", Point.Zero, false, false, "Background", "scripts/sprites/ui/font.sdf", 1f, Color.White, _stripWindow, ShowBackgroundWindow));
                
                _stripWindow.RegisterControl(extraDD);
            }
        }

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

        }

        private static void OpenSDF(Button b)
        {

        }

        private static void SaveSDF(Button b)
        {

        }

        private static void SaveSDFAs(Button b)
        {

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

        }
        
        private static void ShowAttachPointWindow(Button b)
        {

        }

        private static void ShowPaletteWindow(Button b)
        {

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

        }

        internal static void Update()
        {
            _sprite?.Update();
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
        }

        internal static void Unload()
        {
            _background?.UnloadSprite();
            _sprite?.UnloadSprite();
            _stripWindow?.Unload();
            _mainWindow?.Unload();
            _background = null;
            _sprite = null;
            _stripWindow = null;
            _mainWindow = null;
            _bufferColor = Color.Black;
        }

        internal static void Draw()
        {
            Engine.Graphics.GraphicsDevice.Clear(_bufferColor);

            Engine.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Engine.Camera.Transform);

            _background?.Draw(Vector2.Zero);
            _sprite?.Draw(Vector2.Zero);

            Engine.SpriteBatch.End();

            WindowHandler.Draw();
        }

    }
}
