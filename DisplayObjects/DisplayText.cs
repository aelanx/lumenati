using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace Lumenati
{
    public class DisplayText
    {
        public int CharacterId;

        Lumen.DynamicText Text;
        LumenEditor Editor;

        public string Content;

        public DisplayText(LumenEditor editor, Lumen.DynamicText text)
        {
            Editor = editor;
            Text = text;
            Content = Editor.lm.Strings[text.placeholderTextId];

            CharacterId = Text.characterId;
        }

        float getLineLength(string line)
        {
            float lineLength = 0;

            foreach (ushort c in line)
            {
                if (Editor.Font.Glyphs.ContainsKey(c))
                {
                    var glyph = Editor.Font.Glyphs[c];

                    lineLength += glyph.width + glyph.advance;
                }
                else if (c == ' ')
                {
                    lineLength += Editor.Font.spaceWidth;
                }
            }

            return lineLength;
        }

        float getTextLength(string text)
        {
            float length = 0;

            var lines = text.Split('\n');
            foreach (var line in lines)
            {
                float lineLength = getLineLength(line);

                if (lineLength > length)
                    length = lineLength;
            }

            return length;
        }

        public void Render()
        {
            // FIXME: there's probably a better place to handle this.
            if (Editor.Font == null)
                return;

            var len = getTextLength(Content);

            float x = 0;
            float y = 0;

            var verts = new List<Vector4>();
            float scale = Text.size / Editor.Font.defaultSize;

            var atlas = Editor.Font.Texture;
            GL.BindTexture(TextureTarget.Texture2D, atlas.glId);
            if (atlas.type == PixelInternalFormat.CompressedRedRgtc1)
                GL.Uniform1(Editor.Shader.uTexFmt, 1);
            else if (atlas.type == PixelInternalFormat.CompressedRgRgtc2)
                GL.Uniform1(Editor.Shader.uTexFmt, 2);
            else
                GL.Uniform1(Editor.Shader.uTexFmt, 0);

            GL.BindTexture(TextureTarget.Texture2D, atlas.glId);
            GL.Begin(PrimitiveType.Quads);

            var lines = Content.Split('\n');
            foreach (var line in lines)
            {
                if (Text.alignment == Lumen.TextAlignment.Right)
                    x = (len - getLineLength(line)) * scale;
                else if (Text.alignment == Lumen.TextAlignment.Center)
                {
                    var l = getLineLength(line);
                    x = (len - getLineLength(line)) / 2 * scale;
                }
                else
                    x = 0;

                foreach (ushort c in line)
                {
                    if (c == ' ')
                    {
                        x += Editor.Font.spaceWidth * scale;
                        continue;
                    }

                    // TODO: render box for missing glyphs?
                    if (!Editor.Font.Glyphs.ContainsKey(c))
                        continue;

                    var glyph = Editor.Font.Glyphs[c];

                    GL.TexCoord2(glyph.x / Editor.Font.Texture.width, glyph.y / Editor.Font.Texture.height);
                    GL.Vertex2(x, y + glyph.yBearing * scale);

                    GL.TexCoord2((glyph.x + glyph.width) / Editor.Font.Texture.width, glyph.y / Editor.Font.Texture.height);
                    GL.Vertex2(x + glyph.width * scale, y + glyph.yBearing * scale);

                    GL.TexCoord2((glyph.x + glyph.width) / Editor.Font.Texture.width, (glyph.y + glyph.height) / Editor.Font.Texture.height);
                    GL.Vertex2(x + glyph.width * scale, y + (glyph.yBearing + glyph.height) * scale);

                    GL.TexCoord2(glyph.x / Editor.Font.Texture.width, (glyph.y + glyph.height) / Editor.Font.Texture.height);
                    GL.Vertex2(x, y + (glyph.yBearing + glyph.height) * scale);

                    x += (glyph.width + glyph.advance) * scale;
                }

                y += Editor.Font.lineHeight * scale;
            }
            GL.End();
        }
    }
}
