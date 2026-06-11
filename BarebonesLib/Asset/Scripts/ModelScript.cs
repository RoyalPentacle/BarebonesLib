using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Asset.Scripts
{
    public enum CurveType
    {
        Linear,
        Quadratic,
        Bezier,
        Square,
        Sine
    }

    public struct TransformPattern
    {
        public Vector3? Start;
        public Vector3 End;
        public bool Relative;
        public double StartTime;
        public double Length;
        public double Compression;
        public CurveType CurveType;
        public Vector2[] Points;
        public string BoneName;
    }


    public struct AnimationPattern
    {
        public double Length;
        public TransformPattern[] Rotations;
        public TransformPattern[] Translations;
        public TransformPattern[] Scalars;
    }

    public class ModelScript : Script
    {
        public struct BoneAttach
        {
            public string Child;

            public string Parent;

            public Vector3 Offset;
        }

        private string _modelPath;
        private string _texturePath;

        private Dictionary<string, byte> _boneAlias;

        private List<BoneAttach> _attachments;

        private Dictionary<string, AnimationPattern> _animations;

        public string ModelPath
        {
            get { return _modelPath; }
        }

        public string TexturePath
        {
            get { return _texturePath; }
        }

        public Dictionary<string, byte> BoneAlias
        {
            get { return _boneAlias; }
        }

        public ModelScript()
        {

        }
    }
}
