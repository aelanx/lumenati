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
        public Lumen.Shape shape;
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

        public Color4 colorMult;
        public Color4 colorAdd;
    }
}
