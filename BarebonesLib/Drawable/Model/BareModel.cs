using Barebones.Asset;
using Barebones.Asset.Scripts;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barebones.Drawable.Model
{
    public class BareModel
    {
        public class BareMeshInstance
        {
            public BareMeshInstance(BareMesh mesh)
            {

            }
        }
        
        private BareMesh _mesh;

        private Texture2D _texture;



        public BareModel(string scriptPath)
        {
            ModelScript script = ScriptFinder.FindScript<ModelScript>(scriptPath);
            _mesh = ModelHandler.GetMesh(script.ModelPath);
            _texture = Textures.GetTexture(script.TexturePath);
        }
    }
}
