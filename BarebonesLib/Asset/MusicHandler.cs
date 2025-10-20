
namespace Barebones.Asset
{
    /// <summary>
    /// Handler for all music.
    /// </summary>
    public static class Music
    {
        /// <summary>
        /// An object that represents a single loaded song and supporting info.
        /// </summary>
        private class MusicMap
        {
            private OggSong _track;

            private int _count;

            private bool _permanent = false;

            public OggSong Track
            {
                get { return _track; }
            }

            public int Count
            {
                get { return _count; }
                set { _count = value; }
            }
        }

    }

    public class OggSong
    {

    }

    public class OggInstance
    {

    }
}
