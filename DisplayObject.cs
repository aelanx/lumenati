using OpenTK;

namespace Lumenati
{
    public class DisplayObject
    {
        //public DisplayShape shape;
        //public DisplaySprite sprite;
        //public DisplayText text;

        // unlike swf, I don't think lm uses depth as the displaylist key.
        // Not positive, so fuck it.
        //public int depth;

        protected LumenEditor Editor;
        public int CharacterId;
        public string name;

        public bool hasMatrix;
        public bool hasPos;
        public Matrix4 matrix;
        public Vector2 pos;

        public bool hasColor;
        public Vector4 colorAdd;
        public Vector4 colorMult;

        public Lumen.BlendMode BlendMode;

        public DisplayObject(LumenEditor editor, int characterId)
        {
            Editor = editor;
            CharacterId = characterId;
        }

        public virtual void Render(RenderState state) { }
        public virtual DisplayObject Clone()
        {
            return new DisplayObject(Editor, CharacterId);
        }

    }

    // this... uhhh. doesn't belong here...
    public class RenderState
    {
        public Vector4 colorAdd = Vector4.Zero;
        public Vector4 colorMult = Vector4.One;
    }
}
