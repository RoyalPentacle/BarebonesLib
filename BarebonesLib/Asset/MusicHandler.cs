
using Barebones.Asset.Scripts;
using Microsoft.Xna.Framework.Audio;
using NVorbis;
using System.Collections;

namespace Barebones.Asset
{
    /// <summary>
    /// Handler for all music.
    /// </summary>
    public static class Music
    {
        private static OggSong _song;

        private static List<DynamicSoundEffectInstance> _instanceList = new List<DynamicSoundEffectInstance>();

        /// <summary>
        /// Play the music from the specified music script.
        /// </summary>
        /// <param name="musicScript">The path to the music script to play.</param>
        public static void Play(string musicScript)
        {
            _song?.Stop();
            _song = new OggSong(musicScript);
            _song.Play();
        }

        /// <summary>
        /// Stop the specified music from playing.
        /// </summary>
        /// <param name="musicScript"></param>
        public static void Stop(string musicScript)
        {
            _song.Stop();
        }

        internal static void DeclareMusicInstance(DynamicSoundEffectInstance instance)
        {
            _instanceList.Add(instance);
        }

        internal static void DisposeStoppedInstances()
        {
            for (int i = _instanceList.Count - 1; i >= 0; i--)
            {
                if (_instanceList[i].State == SoundState.Stopped)
                {
                    _instanceList[i].Dispose();
                    _instanceList.RemoveAt(i);
                }
            }
        }
    }

    /// <summary>
    /// A music track to play in ogg vorbis format.
    /// </summary>
    public class OggSong
    {

        private TimeSpan _loopEnd;
        private TimeSpan _loopLength;


        private int _channels;
        private int _sampleRate;


        private DynamicSoundEffectInstance _dynamicOgg;

        private EventHandler<EventArgs> _handler;

        private VorbisReader _reader;

        /// <summary>
        /// Get and Set the volume of the music track.
        /// Defaults to Engine.MusicVolume.
        /// When set, multiplies the value by Engine.MusicVolume.
        /// </summary>
        public float Volume
        {
            get
            {
                if (_dynamicOgg != null)
                    return _dynamicOgg.Volume;
                else
                    return Engine.MusicVolume;
            }
            set
            {
                if (_dynamicOgg != null)
                    _dynamicOgg.Volume = (float)Math.Clamp(value * Engine.MusicVolume, 0.0, 1.0);
            }
        }

        /// <summary>
        /// Get and Set the Pitch of the music track.
        /// </summary>
        public float Pitch
        {
            get { return _dynamicOgg.Pitch; }
            set { _dynamicOgg.Pitch = (float)Math.Clamp(value, -1.0, 1.0); }
        }
        
        /// <summary>
        /// Create a new OggSong from the specified music script.
        /// </summary>
        /// <param name="scriptPath">The path to the music script.</param>
        public OggSong(string scriptPath)
        {
            MusicScript script = ScriptFinder.FindScript<MusicScript>(scriptPath);
            _reader = new VorbisReader(script.MusicPath);
            _channels = _reader.Channels;
            _sampleRate = _reader.SampleRate;

            _loopEnd = script.LoopEnd;
            _loopLength = script.LoopEnd - script.LoopStart;
            _dynamicOgg = new DynamicSoundEffectInstance(_sampleRate, (AudioChannels)_channels);
            Music.DeclareMusicInstance(_dynamicOgg);

            _handler = new EventHandler<EventArgs>(UpdateBuffer);
            _dynamicOgg.BufferNeeded += _handler;
        }

        private byte[] ReadOgg()
        {
            float[] buffer = new float[_channels * _sampleRate / 5];
            List<byte> byteList = new List<byte>();
            int count = _reader.ReadSamples(buffer, 0, buffer.Length);
            for (int i = 0; i < count; i++)
            {
                short temp = (short)(32767f * buffer[i]);
                if (temp > 32767)
                {
                    byteList.Add(0xFF);
                    byteList.Add(0x7F);
                }
                else if (temp < -32768)
                {
                    byteList.Add(0x80);
                    byteList.Add(0x00);
                }
                byteList.Add((byte)temp);
                byteList.Add((byte)(temp >> 8));
            }


            return byteList.ToArray();
        }

        /// <summary>
        /// Play the OggSong.
        /// </summary>
        public void Play()
        {
            _dynamicOgg.Pitch = 0;
            _dynamicOgg.Volume = Engine.MusicVolume;
            _dynamicOgg.Play();
        }

        /// <summary>
        /// Stop the OggSong.
        /// </summary>
        public void Stop()
        {
            if (_dynamicOgg != null)
            {
                _dynamicOgg.Stop();
                _dynamicOgg.BufferNeeded -= _handler;
                _reader.Dispose();
                _reader = null;
                _dynamicOgg = null;
            }
        }

        /// <summary>
        /// Set the playhead of the OggSong.
        /// </summary>
        /// <param name="time">The TimeSpan to set the playhead to.</param>
        public void SetPlayHead(TimeSpan time)
        {
            _reader.TimePosition = time;
        }

        /// <summary>
        /// Get the current playhead of the OggSong
        /// </summary>
        /// <returns>A TimeSpan representing the current playhead position.</returns>
        public TimeSpan GetPlayHead()
        {
            return _reader.TimePosition;
        }

        private void UpdateBuffer(object sender, EventArgs e)
        {
            byte[] buffer = ReadOgg();
            if (buffer != null && buffer.Length > 0)
            {
                _dynamicOgg.SubmitBuffer(buffer, 0, buffer.Length / 2);
                _dynamicOgg.SubmitBuffer(buffer, buffer.Length / 2, buffer.Length / 2);
            } 
            else 
            {
                Stop();
                return;
            }


            if (_loopEnd != TimeSpan.Zero && _reader.TimePosition > _loopEnd)
            {
                TimeSpan currentHead = _reader.TimePosition;
                currentHead -= _loopLength;
                _reader.TimePosition = currentHead;
            }

        }
    }

}
