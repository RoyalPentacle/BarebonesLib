using Barebones.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.States
{
    internal enum State : byte
    {
        None,
        Select,
        Sprite,
        Music,
        Sound,
        Particle,
        Bundle
    }

    internal static class StateHandler
    {
        internal static State State = State.None;
        internal static void ChangeState(State state)
        {

            if (state != State)
            {
                Language.SetLanguage("");
                Unload();
                State = state;
                switch (State)
                {
                    case State.None:
                        break;
                    case State.Select:
                        EditorSelect.Init();
                        break;
                    case State.Sprite:
                        SpriteEditor.Init();
                        break;
                    case State.Music:
                        MusicEditor.Init();
                        break;
                    case State.Sound:
                        SoundEditor.Init();
                        break;
                    case State.Bundle:
                        BundleEditor.Init();
                        break;
                    case State.Particle:
                        ParticleEditor.Init();
                        break;
                }
                
            }
        }
        

        internal static void Unload()
        {
            switch (State)
            {
                case State.None:
                    break;
                case State.Select:
                    EditorSelect.Unload();
                    break;
                case State.Sprite:
                    SpriteEditor.Unload();
                    break;
                case State.Music:
                    MusicEditor.Unload();
                    break;
                case State.Sound:
                    SoundEditor.Unload();
                    break;
                case State.Bundle:
                    BundleEditor.Unload();
                    break;
                case State.Particle:
                    ParticleEditor.Unload();
                    break;
            }
        }

        internal static void Update()
        {
            switch (State)
            {
                case State.None:
                    break;
                case State.Select: // The menu doesn't actually have any update logic, it's done entirely through the window system.
                    break;
                case State.Sprite:
                    SpriteEditor.Update();
                    break;
                case State.Music:
                    MusicEditor.Update();
                    break;
                case State.Sound:
                    SoundEditor.Update();
                    break;
                case State.Bundle: // The bundle editor doesn't actually have any update logic, it's done entirely through the window system.
                    break;
                case State.Particle:
                    ParticleEditor.Update();
                    break;
            }
        }

        internal static void Draw()
        {
            switch (State)
            {
                case State.None:
                    break;
                case State.Select:
                    EditorSelect.Draw();
                    break;
                case State.Sprite:
                    SpriteEditor.Draw();
                    break;
                case State.Music:
                    MusicEditor.Draw();
                    break;
                case State.Sound:
                    SoundEditor.Draw();
                    break;
                case State.Bundle:
                    BundleEditor.Draw();
                    break;
                case State.Particle:
                    ParticleEditor.Draw();
                    break;
            }
        }
    }
}
