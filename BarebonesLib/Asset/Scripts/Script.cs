
namespace Barebones.Asset.Scripts
{
    public abstract class Script
    {
        // much work to do here
        protected long _fileSize;

        public long FileSize
        {
            get { return _fileSize; }
        }

        public virtual void Unload()
        {
            // By default, does nothing, but child classes should have the option of doing unloading logic if necessary.
        }
    }
}
