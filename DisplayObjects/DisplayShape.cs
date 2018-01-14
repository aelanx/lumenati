using OpenTK.Graphics.OpenGL;

namespace Lumenati
{
    public class DisplayShape : DisplayObject
    {
        Lumen.Shape Shape;
        Lumen.Rect Bounds;

        public DisplayShape(LumenEditor editor, Lumen.Shape shape) : base(editor, shape.CharacterId)
        {
            Shape = shape;
            Bounds = Editor.lm.Bounds[Shape.BoundsId];
        }

        public override DisplayObject Clone()
        {
            return new DisplayShape(Editor, Shape);
        }

        public override void Render(RenderState state)
        {
            Editor.StencilRect(Bounds);

            foreach (var graphic in Shape.Graphics)
            {
                var atlas = Editor.GetAtlas(graphic.AtlasId);
                if (atlas != null)
                {
                    GL.BindTexture(TextureTarget.Texture2D, atlas.glId);
                    if (atlas.type == PixelInternalFormat.CompressedRedRgtc1)
                        GL.Uniform1(Editor.Shader.uTexFmt, 1);
                    else if (atlas.type == PixelInternalFormat.CompressedRgRgtc2)
                        GL.Uniform1(Editor.Shader.uTexFmt, 2);
                    else
                        GL.Uniform1(Editor.Shader.uTexFmt, 0);
                }
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    GL.Uniform1(Editor.Shader.uTexFmt, 0);
                }


                Editor.DrawGraphic(graphic, PrimitiveType.Triangles);
            }

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            //Editor.DrawBounds(Bounds);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
    }
}
