using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Drawable.Model
{
    public class BareMesh
    {
        public class BareMeshPart
        {
            private Vector3 _origin;

            private Matrix _originMatrix;

            private ushort[] _indices;

            public ushort[] Indices
            {
                get { return _indices; }
            }

            private int _startIndex;

            public int StartIndex
            {
                get { return _startIndex; }
                internal set { _startIndex = value; }
            }

            public int PrimitiveCount
            {
                get { return _indices.Length - 2; }
            }

            private const int INDEX_BYTE_STRIDE = 2;
            private const int HEADER_OFFSET = 14;

            public BareMeshPart(byte[] bytes, int offset, out int jumpAhead)
            {
                ushort numIndices = BitConverter.ToUInt16(bytes, offset);
                _indices = new ushort[numIndices];
                _origin.X = BitConverter.ToSingle(bytes, offset + 2);
                _origin.Y = BitConverter.ToSingle(bytes, offset + 6);
                _origin.Z = BitConverter.ToSingle(bytes, offset + 10);
                for (int i = 0; i < numIndices; i++)
                {
                    _indices[i] = BitConverter.ToUInt16(bytes, offset + HEADER_OFFSET + i * INDEX_BYTE_STRIDE);
                }
                jumpAhead = HEADER_OFFSET + numIndices * INDEX_BYTE_STRIDE;
                _originMatrix = Matrix.CreateTranslation(-_origin);
            }
        }

        private Vector3 _origin;

        private Matrix _originMatrix;

        private VertexPositionNormalTexture[] _vertices;

        private ushort[] _indices;

        private VertexBuffer _vertexBuffer;

        private IndexBuffer _indexBuffer;

        private BareMeshPart[] _meshes;


        public Vector3 Origin
        {
            get { return _origin; }
        }

        public Matrix OriginMatrix
        {
            get { return _originMatrix; }
        }
        public BareMeshPart[] Meshes
        {
            get { return _meshes; }
        }

        public VertexBuffer VertexBuffer
        {
            get { return _vertexBuffer; }
        }

        public IndexBuffer IndexBuffer
        {
            get { return _indexBuffer; }
        }

        public BareMesh(string modelPath)
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(modelPath, FileMode.Open, FileAccess.Read)))
                {
                    byte[] bytes = reader.ReadBytes((int)reader.BaseStream.Length);
                    if (bytes.Length <= 0)
                    {
                        throw new Exception("Empty File.");
                    }
                    if (bytes[0] != 66 ||
                        bytes[1] != 66 ||
                        bytes[2] != 77 ||
                        bytes[3] != 68 ||
                        bytes[4] != 76)
                    {
                        throw new Exception("Invalid file format!");
                    }
                    switch (bytes[5])
                    {
                        default: // The current loader.
                            ushort numVertices = BitConverter.ToUInt16(bytes, 6);
                            _vertices = new VertexPositionNormalTexture[numVertices];
                            _meshes = new BareMeshPart[bytes[8]];
                            _origin.X = BitConverter.ToSingle(bytes, 9);
                            _origin.Y = BitConverter.ToSingle(bytes, 13);
                            _origin.Z = BitConverter.ToSingle(bytes, 17);
                            int vertexByteStride = 32; // convert to constant?
                            int headerOffset = 21;
                            int vertexOffset = 0;
                            for (int i = 0; i < numVertices; i++)
                            {
                                vertexOffset = headerOffset + i * vertexByteStride;

                                Vector3 pos;
                                pos.X = BitConverter.ToSingle(bytes, vertexOffset);
                                pos.Y = BitConverter.ToSingle(bytes, vertexOffset + 4);
                                pos.Z = BitConverter.ToSingle(bytes, vertexOffset + 8);

                                Vector3 normal;
                                normal.X = BitConverter.ToSingle(bytes, vertexOffset + 12);
                                normal.Y = BitConverter.ToSingle(bytes, vertexOffset + 16);
                                normal.Z = BitConverter.ToSingle(bytes, vertexOffset + 20);

                                Vector2 texCoord;
                                texCoord.X = BitConverter.ToSingle(bytes, vertexOffset + 24);
                                texCoord.Y = 1f - BitConverter.ToSingle(bytes, vertexOffset + 28);

                                _vertices[i].Position = pos;
                                _vertices[i].Normal = normal;
                                _vertices[i].TextureCoordinate = texCoord;
                            }
                            int meshOffset = headerOffset + numVertices * vertexByteStride;
                            int totalJumpAhead = 0;
                            int totalIndices = 0;
                            for (int i = 0; i < _meshes.Length; i++)
                            {
                                _meshes[i] = new BareMeshPart(bytes, meshOffset + totalJumpAhead, out int jumpAhead);
                                totalIndices = _meshes[i].Indices.Length;
                                totalJumpAhead += jumpAhead;
                            }
                            _indices = new ushort[totalIndices];
                            break;
                    }
                }
                _vertexBuffer = new VertexBuffer(Engine.Graphics.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, _vertices.Length, BufferUsage.WriteOnly);
                _vertexBuffer.SetData(_vertices);
                int indexOffset = 0;
                for (int i = 0; i < _meshes.Length; i++)
                {
                    _meshes[i].Indices.CopyTo(_indices, indexOffset);
                    _meshes[i].StartIndex = indexOffset;
                    indexOffset += _meshes[i].Indices.Length;
                }
                _indexBuffer = new IndexBuffer(Engine.Graphics.GraphicsDevice, typeof(ushort), _indices.Length, BufferUsage.WriteOnly);
                _originMatrix = Matrix.CreateTranslation(_origin);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load model.", ex);
            }
        }

        public void Unload()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }
    }
}
