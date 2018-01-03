using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumenati
{
    public class DisplayObject
    {
        public RuntimeShape shape;
        public RuntimeSprite sprite;
        //public RuntimeText text;

        // unlike swf, I don't think lm uses depth as the displaylist key.
        // Not positive, so fuck it.
        //public int depth;

        public string name;

        public bool hasMatrix;
        public bool hasPos;
        public Matrix4 matrix;
        public Vector2 pos;

        public bool hasColor;
        public Vector4 colorAdd;
        public Vector4 colorMult;
    }

    // this... uhhh. doesn't belong here...
    public class RenderState
    {
        public Vector4 colorAdd = Vector4.Zero;
        public Vector4 colorMult = Vector4.One;
    }
}
