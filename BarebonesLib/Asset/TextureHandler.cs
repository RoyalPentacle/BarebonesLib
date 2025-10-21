using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Barebones.Config;

namespace Barebones.Asset
{
    /// <summary>
    /// Handler for all textures.
    /// </summary>
    public static class Textures
    {
        /// <summary>
        /// Textures that are shared across all classes, independent of the sprite system.
        /// </summary>
        public static class Shared
        {
            private static Texture2D _pixel; // A 2x2 white square, used for drawing primitive rectangles.
            private static Texture2D _fallbackTexture; // A 32x32 purple and black square texture, used as a fallback if a texture fails to load.

            /// <summary>
            /// A 2x2 white square, used for drawing primitive rectangles.
            /// </summary>
            public static Texture2D Pixel
            {
                get { return _pixel; }
            }

            /// <summary>
            /// A 32x32 purple and black square texture, used as a fallback if a texture fails to load.
            /// </summary>
            public static Texture2D FallbackTexture
            {
                get { return _fallbackTexture; }
            }

            /// <summary>
            /// Initialize the shared texture class.
            /// </summary>
            public static void Init()
            {
                // Create the shared textures
                _pixel = new Texture2D(Engine.Graphics.GraphicsDevice, 2, 2, false, SurfaceFormat.Color);
                _fallbackTexture = new Texture2D(Engine.Graphics.GraphicsDevice, 32, 32, false, SurfaceFormat.Color);


                // Create the Color arrays to fill the textures with
                Color[] pixCol = { Color.White, Color.White, Color.White, Color.White };

                Color p = new Color(255, 0, 255);
                Color b = Color.Black;
                Color[] fallCol = {
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p,
                b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, b, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p, p
                };

                // Set the data of the textures to the color arrays
                _fallbackTexture.SetData(fallCol);
                _pixel.SetData(pixCol);
            }

        }

        /// <summary>
        /// An object that represents a single loaded Texture2D and supporting info.
        /// </summary>
        private class TextureMap
        {
            // The actual loaded texture.
            private Texture2D _texture;

            // The number of things currently using this texture.
            private int _count;

            // The size of the asset loaded.
            private long _fileSize;


            /// <summary>
            /// The Texture2D stored in this TextureMap.
            /// </summary>
            public Texture2D Texture
            {
                get { return _texture; }
            }

            /// <summary>
            /// The use count of this TextureMap.
            /// </summary>
            public int Count
            {
                get { return _count; }
                set { _count = value; }
            }

            /// <summary>
            /// The size of the asset loaded.
            /// </summary>
            public long FileSize
            {
                get { return _fileSize; }
            }

            /// <summary>
            /// Constructs a new TextureMap from the specified arguments.
            /// </summary>
            /// <param name="texturePath">The path to the texture to load.</param>
            public TextureMap(string texturePath)
            {
                try // Attempt to load the specified texture
                {
                    _fileSize = new FileInfo(texturePath).Length;
                    _texture = Texture2D.FromFile(Engine.Graphics.GraphicsDevice, texturePath);
                }
                catch (Exception ex) // But if we can't, use the fallback and print to console.
                {
                    _texture = Shared.FallbackTexture;
                    Verbose.WriteErrorMajor($"TEXTURE: Error loading file at: {texturePath}\n Loading fallback texture. EX: {ex.Message}");
                }

                _count = 1; // A brand new texture is being used by one thing.
            }

            /// <summary>
            /// If the texture isn't null, dispose of the texture.
            /// </summary>
            public void Unload()
            {
                // Make sure we don't dispose the shared textures.
                if (_texture != null && _texture != Shared.FallbackTexture && _texture != Shared.Pixel)
                {
                    _texture.Dispose();
                }
            }
        }

        // A dictionary that contains all stored textures.
        private static Dictionary<string, TextureMap> _textureDict = new Dictionary<string, TextureMap>();

        private static Dictionary<string, TextureMap> _textureCache = new Dictionary<string, TextureMap>();
        private static List<string> _sortedCache = new List<string>();
        private static long _cacheSize = 0L;
        private static Mutex _mutex = new Mutex();

        /// <summary>
        /// Ask the handler to return a Texture2D with a given name. If we don't have it, try to load it.
        /// </summary>
        /// <param name="texturePath">The name of the texture to get.</param>
        /// <returns>A Texture2D.</returns>
        public static Texture2D GetTexture(string texturePath)
        {
            try // Try to get the texture from the dictionary
            {
                _mutex.WaitOne();
                TextureMap tex = _textureDict[texturePath];
                tex.Count++;
                return tex.Texture;
            }
            catch // If we can't, load it instead and return that.
            {
                LoadTexture(texturePath);
                return _textureDict[texturePath].Texture;
            }
            finally
            {
                _mutex.ReleaseMutex();
            }

        }

        /// <summary>
        /// Loads a texture into our texture dictionary.
        /// </summary>
        /// <param name="texturePath">The name of the texture to load.</param>
        private static void LoadTexture(string texturePath)
        {
            // Create sprite definition
            try
            {
                GetTextureFromCache(texturePath);
            }
            catch
            {
                TextureMap newTex = new TextureMap(texturePath);
                _textureDict.Add(texturePath, newTex);
            }
        }

        /// <summary>
        /// Let the handler know that an object is no longer using a given texture.
        /// If no other objects are using the texture, dispose of it.
        /// </summary>
        /// <param name="texturePath">The name of the texture to unload.</param>
        public static void UnloadTexture(string texturePath)
        {
            try // Try to unload the texture
            {
                _mutex.WaitOne();
                TextureMap tex = _textureDict[texturePath];
                tex.Count--;
                if (tex.Count <= 0)
                {
                    AddTextureToCache(texturePath, tex);
                }
                return;
            }
            catch (Exception ex) // If something goes wrong, which it could, spit out a minor error.
            {
                Verbose.WriteErrorMinor($"TEXTURE: Error unloading texture: {texturePath}\n Doing nothing about this? EX: {ex.Message}");
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Removes the specified texture from the active dictionary and adds it to the cache.
        /// Then trims the cache.
        /// </summary>
        /// <param name="texturePath">The name of the texture, for the dictionary.</param>
        /// <param name="tex">The TextureMap from the dictionary.</param>
        private static void AddTextureToCache(string texturePath, TextureMap tex)
        {
            _textureDict.Remove(texturePath);
            _textureCache.Add(texturePath,tex);
            _sortedCache.Add(texturePath);
            _cacheSize += tex.FileSize;
            TrimTextureCache();
        }

        /// <summary>
        /// Retrieves the specified texture from the cache.
        /// </summary>
        /// <param name="texturePath">The name of the texture to get.</param>
        private static void GetTextureFromCache(string texturePath)
        {
            TextureMap tex = _textureCache[texturePath];
            _textureCache.Remove(texturePath);
            _sortedCache.Remove(texturePath);
            _cacheSize -= tex.FileSize;
            tex.Count++;
            _textureDict.Add(texturePath, tex);
        }

        /// <summary>
        /// If the cache is over capacity, remove the oldest entry until it isn't.
        /// </summary>
        private static void TrimTextureCache()
        {
            while (_cacheSize > Engine.TextureCacheMaxSize)
            {
                string pathRemove = _sortedCache[0];
                TextureMap tex = _textureCache[pathRemove];
                _cacheSize -= tex.FileSize;
                tex.Unload();
                _textureCache.Remove(pathRemove);
                _sortedCache.RemoveAt(0);
            }
        }
    }
}
