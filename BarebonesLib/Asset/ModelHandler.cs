using Barebones.Drawable.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Asset
{
    /// <summary>
    /// Handler for all models.
    /// </summary>
    public static class ModelHandler
    {
        /// <summary>
        /// Models that are shared across all classes, independent of the model system.
        /// </summary>
        public static class Shared
        {
            // need a fallback model in here.
        }

        private class MeshMap
        {
            private BareMesh _mesh;

            private int _count;

            private long _fileSize;

            public BareMesh Mesh
            {
                get { return _mesh; }
            }

            public int Count
            {
                get { return _count; }
                set { _count = value; }
            }

            public long FileSize
            {
                get { return _fileSize; }
            }

            public MeshMap(string modelPath)
            {
                try
                {
                    _fileSize = new FileInfo(modelPath).Length;
                    // _mesh = new BareMesh(modelPath);
                    Verbose.WriteLogMinor($"Loaded!: {modelPath}");
                }
                catch (Exception ex)
                {
                    // _mesh = Shared.FallbackMesh;
                    if (modelPath != "fallback")
                        Verbose.WriteErrorMajor($"MODEL: Error loading file at: {modelPath}\n Loading fallback model. EX: {ex.Message}");
                }

                _count = 1;
            }

            public void Unload()
            {
                if (_mesh != null) //&& _mesh != Shared.FallbackMesh)
                    _mesh.Unload();
            }
        }

        private static Dictionary<string, MeshMap> _meshDict = new Dictionary<string, MeshMap>();

        private static OrderedDictionary<string, MeshMap> _meshCache = new OrderedDictionary<string, MeshMap>();

        private static long _cacheSize = 0L;

        private static Mutex _mutex = new Mutex();

        public static long CacheSize
        {
            get { return _cacheSize; }
        }

        public static BareMesh GetMesh(string modelPath)
        {
            try
            {
                _mutex.WaitOne();
                if (_meshDict.TryGetValue(modelPath, out MeshMap? mesh))
                {
                    mesh.Count++;
                    return mesh.Mesh;
                }
                else
                {
                    LoadMesh(modelPath);
                    return _meshDict[modelPath].Mesh;
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        private static void LoadMesh(string modelPath)
        {
            if (!GetMeshFromCache(modelPath))
            {
                MeshMap newMesh = new MeshMap(modelPath);
                _meshDict.Add(modelPath, newMesh);
            }
        }

        public static void UnloadMesh(string modelPath)
        {
            try
            {
                _mutex.WaitOne();
                MeshMap mesh = _meshDict[modelPath];
                mesh.Count--;
                if (mesh.Count <= 0)
                {
                    AddMeshToCache(modelPath, mesh);
                }
                return;
            }
            catch (Exception ex)
            {
                Verbose.WriteErrorMinor($"MODEL: Error unloading model: {modelPath}\n Doing nothing about this? EX: {ex.Message}");
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        private static void AddMeshToCache(string modelPath, MeshMap mesh)
        {
            _meshDict.Remove(modelPath);
            _meshCache.Add(modelPath, mesh);
            _cacheSize += mesh.FileSize;
            TrimMeshCache();
        }

        private static bool GetMeshFromCache(string modelPath)
        {
            if (_meshCache.TryGetValue(modelPath, out MeshMap? mesh))
            {
                _meshCache.Remove(modelPath);
                _cacheSize -= mesh.FileSize;
                mesh.Count++;
                _meshDict.Add(modelPath, mesh);
                return true;
            }
            else
                return false;
        }

        private static void TrimMeshCache()
        {
            while (_cacheSize > Engine.MeshCacheMaxSize)
            {
                MeshMap mesh = _meshCache.GetAt(0).Value;
                _cacheSize -= mesh.FileSize;
                mesh.Unload();
                _meshCache.RemoveAt(0);
            }
        }
        
    }
}
