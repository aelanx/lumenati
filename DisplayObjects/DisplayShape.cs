using OpenTK.Graphics.OpenGL;

namespace Lumenati
{
    public class DisplayShape
    {
        public int CharacterId;

        Lumen.Shape Shape;
        Lumen.Rect Bounds;
        LumenEditor Editor;

        public DisplayShape(LumenEditor editor, Lumen.Shape shape)
        {
            Editor = editor;
            Shape = shape;
            Bounds = Editor.lm.Bounds[Shape.BoundsId];

            CharacterId = Shape.CharacterId;
        }

        public void Render()
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
