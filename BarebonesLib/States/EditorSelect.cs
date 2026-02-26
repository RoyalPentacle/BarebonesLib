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


        internal static void Init()
        {
            if (_logo == null)
            {
                _logo = new Texture2D(Engine.Graphics.GraphicsDevice, 256, 256);
                _logo.SetData<byte>(Logo.logoBytes());
            }
            Engine.Camera.MoveCamera(new Vector2(Engine.Graphics.GraphicsDevice.Viewport.Width / 2, Engine.Graphics.GraphicsDevice.Viewport.Height / 2));
            if (_menu == null)
            {
                _menu = new Window("editSelect", "scripts/sprites/ui/windows/default.sdf", new Rectangle(Engine.Graphics.GraphicsDevice.Viewport.Width / 2 - 300, (int)(Engine.Graphics.GraphicsDevice.Viewport.Height / 1.5f), 628, 50));
                _menu.RegisterControl(new TextButton("spriteEdit", new Rectangle(8, 7, 116, 36), true, true, "Sprite", "scripts/sprites/ui/font.sdf", 1f, Color.White, _menu, SpriteEditor));
                _menu.RegisterControl(new TextButton("musicEdit", new Rectangle(132, 7, 116, 36), true, true, "Music", "scripts/sprites/ui/font.sdf", 1f, Color.White, _menu, MusicEditor));
                _menu.RegisterControl(new TextButton("soundEdit", new Rectangle(256, 7, 116, 36), true, true, "Sound", "scripts/sprites/ui/font.sdf", 1f, Color.White, _menu, SoundEditor));
                _menu.RegisterControl(new TextButton("particleEdit", new Rectangle(380, 7, 116, 36), true, true, "Particle", "scripts/sprites/ui/font.sdf", 1f, Color.White, _menu, ParticleEditor));
                _menu.RegisterControl(new TextButton("bundleEdit", new Rectangle(504, 7, 116, 36), true, true, "Bundle", "scripts/sprites/ui/font.sdf", 1f, Color.White, _menu, BundleEditor));
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

        internal static void Unload()
        {
            _logo?.Dispose();
            _logo = null;
            _menu?.Unload();
            _menu = null;
        }

        internal static void Draw()
        {
            Engine.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Engine.Camera.Transform);
            if (_logo != null)
            Engine.SpriteBatch.Draw(_logo, new Vector2(Engine.Graphics.GraphicsDevice.Viewport.Width / 2, Engine.Graphics.GraphicsDevice.Viewport.Height / 4) - _logo.Bounds.Center.ToVector2(), Color.White);

            Engine.SpriteBatch.End();

            WindowHandler.Draw();
        }
    }
}
