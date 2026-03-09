using Barebones.Config;
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
    internal static class EditorSelect
    {
        private static Texture2D? _logo;

        private static Window? _menu;

        private static int _windowWidth = 1280;
        private static int _windowHeight = 800;
        internal static void Init()
        {
            Engine.Game.Window.AllowUserResizing = true;
            Engine.Game.Window.ClientSizeChanged += Window_ClientSizeChanged;
            if (_logo == null)
            {
                _logo = new Texture2D(Engine.Graphics.GraphicsDevice, 256, 256);
                _logo.SetData<byte>(Logo.logoBytes());
            }
            Engine.Camera.MoveCamera(new Vector2(Engine.Graphics.GraphicsDevice.Viewport.Width / 2, Engine.Graphics.GraphicsDevice.Viewport.Height / 2));
            if (_menu == null)
            {
                _menu = new Window("editSelect", "scripts/sprites/ui/windows/default.sdf", new Rectangle(Engine.Graphics.GraphicsDevice.Viewport.Width / 2 - 314, (int)(Engine.Graphics.GraphicsDevice.Viewport.Height / 1.5f), 628, 50));
                _menu.RegisterControl(new TextButton("spriteEdit", new Rectangle(8, 7, 116, 36), true, true, "Sprite", "scripts/sprites/ui/font.sdf", 1f, Color.White, _menu, SpriteEditor));
                _menu.RegisterControl(new TextButton("musicEdit", new Rectangle(132, 7, 116, 36), true, true, "Music", "scripts/sprites/ui/font.sdf", 1f, Color.White, _menu, MusicEditor));
                _menu.RegisterControl(new TextButton("soundEdit", new Rectangle(256, 7, 116, 36), true, true, "Sound", "scripts/sprites/ui/font.sdf", 1f, Color.White, _menu, SoundEditor));
                _menu.RegisterControl(new TextButton("particleEdit", new Rectangle(380, 7, 116, 36), true, true, "Particle", "scripts/sprites/ui/font.sdf", 1f, Color.White, _menu, ParticleEditor));
                _menu.RegisterControl(new TextButton("bundleEdit", new Rectangle(504, 7, 116, 36), true, true, "Bundle", "scripts/sprites/ui/font.sdf", 1f, Color.White, _menu, BundleEditor));
            }
        }

        private static void Window_ClientSizeChanged(object? sender, EventArgs e)
        {
            if (!Engine.Game.Window.IsBorderless)
            {
                _windowWidth = Engine.Game.Window.ClientBounds.Width;
                _windowHeight = Engine.Game.Window.ClientBounds.Height;
                if (_menu != null)
                    _menu.ChangeSize(new Rectangle(new Point(Engine.Graphics.GraphicsDevice.Viewport.Width / 2 - _menu.Bounds.Width / 2, (int)(Engine.Graphics.GraphicsDevice.Viewport.Height / 1.5f)), _menu.Bounds.Size));
            }
        }


        private static void SpriteEditor(Button b)
        {
            StateHandler.ChangeState(State.Sprite);
        }

        private static void BundleEditor(Button b)
        {
            StateHandler.ChangeState(State.Bundle);
        }

        private static void MusicEditor(Button b)
        {
            StateHandler.ChangeState(State.Music);
        }

        private static void SoundEditor(Button b)
        {
            StateHandler.ChangeState(State.Sound);
        }

        private static void ParticleEditor(Button b)
        {
            StateHandler.ChangeState(State.Particle);
        }

        internal static void Update()
        {
            if (Control.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F1))
            {
                Engine.Game.Window.IsBorderless = !Engine.Game.Window.IsBorderless;
                if (Engine.Game.Window.IsBorderless)
                {
                    Engine.Graphics.PreferredBackBufferWidth = Engine.Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                    Engine.Graphics.PreferredBackBufferHeight = Engine.Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
                    Engine.Game.Window.Position = Point.Zero;
                }
                else
                {
                    Engine.Graphics.PreferredBackBufferWidth = _windowWidth;
                    Engine.Graphics.PreferredBackBufferHeight = _windowHeight;
                    Engine.Game.Window.Position = new Point(Engine.Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width / 2 - _windowWidth / 2, Engine.Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height / 2 - _windowHeight / 2);
                }
                Engine.Graphics.ApplyChanges();
                if (_menu != null)
                    _menu.ChangeSize(new Rectangle(new Point(Engine.Graphics.GraphicsDevice.Viewport.Width / 2 - _menu.Bounds.Width / 2, (int)(Engine.Graphics.GraphicsDevice.Viewport.Height / 1.5f)), _menu.Bounds.Size));
            }
        }

        internal static void Unload()
        {
            _logo?.Dispose();
            _logo = null;
            _menu?.Unload();
            _menu = null;

            Engine.Game.Window.AllowUserResizing = false;
        }

        internal static void Draw()
        {
            Engine.Graphics.GraphicsDevice.Clear(Color.Black);
            Engine.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
            if (_logo != null)
                Engine.SpriteBatch.Draw(_logo, new Vector2(Engine.Graphics.GraphicsDevice.Viewport.Width / 2, Engine.Graphics.GraphicsDevice.Viewport.Height / 4) - _logo.Bounds.Center.ToVector2(), Color.White);

            Engine.SpriteBatch.End();

            WindowHandler.Draw();
        }
    }
}
