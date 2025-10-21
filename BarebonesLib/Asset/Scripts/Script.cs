using Newtonsoft.Json;

namespace Barebones.Asset.Scripts
{
    /// <summary>
    /// The base class for all scripts.
    /// </summary>
    public abstract class Script
    {
        [JsonIgnore]
        private long _fileSize;

        /// <summary>
        /// Gets the size of the file on disk. Must be set when the Script is created.
        /// </summary>
        [JsonIgnore]
        public long FileSize
        {
            get { return _fileSize; }
        }

        /// <summary>
        /// Set the size on disk of the Script.
        /// </summary>
        /// <param name="filesize">The size of the file.</param>
        public virtual void SetFilesize(long filesize)
        {
            _fileSize = filesize;
        }

        /// <summary>
        /// Unloads the current script.
        /// Has no default behaviour, just here to be overridden.
        /// </summary>
        public virtual void Unload()
        {
            // By default, does nothing, but child classes should have the option of doing unloading logic if necessary.
        }
    }
}
