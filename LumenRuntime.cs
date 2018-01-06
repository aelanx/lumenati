using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Lumenati
{
    public class RuntimeShape
    {
        public int CharacterId;

        Lumen.Shape Shape;
        Lumen.Rect Bounds;
        LumenEditor Editor;

        public RuntimeShape(LumenEditor editor, Lumen.Shape shape)
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

    public class RuntimeText
    {
        public int CharacterId;

        Lumen.DynamicText Text;
        LumenEditor Editor;

        public string Content;

        public RuntimeText(LumenEditor editor, Lumen.DynamicText text)
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


    public class RuntimeSprite
    {
        public bool Playing = true;
        public int CurrentFrame = 0;
        public bool Visible = true;

        //
        public int CharacterId;
        SortedDictionary<int, DisplayObject> DisplayList = new SortedDictionary<int, DisplayObject>();
        LumenEditor Editor;
        Lumen.Sprite Sprite;

        public RuntimeSprite(LumenEditor editor, Lumen.Sprite sprite)
        {
            Editor = editor;
            Sprite = sprite;

            CharacterId = Sprite.CharacterId;
        }

        public void GotoLabel(string txt)
        {
            for (int keyframeId = 0; keyframeId < Sprite.labels.Count; keyframeId++)
            {
                var label = Sprite.labels[keyframeId];
                if (Editor.lm.Strings[label.NameId] == txt)
                {
                    handleFrame(Sprite.Keyframes[keyframeId]);
                    CurrentFrame = label.StartFrame;
                    return;
                }
            }
        }

        public RuntimeSprite SearchChild(string name)
        {
            foreach (var obj in DisplayList.Values)
            {
                if (obj.name == name)
                    return obj.sprite;

                if (obj.sprite != null)
                {
                    var r = obj.sprite.SearchChild(name);
                    if (r != null)
                        return r;
                }
            }

            return null;
        }

        public void Stop()
        {
            Playing = false;
        }

        void handleFrame(Lumen.Sprite.Frame frame)
        {
            foreach (var removal in frame.Removals)
            {
                if (DisplayList.ContainsKey(removal.Depth))
                    DisplayList.Remove(removal.Depth);
            }

            foreach (var placement in frame.Placements)
            {
                DisplayObject obj;

                if (placement.Flags == Lumen.PlaceFlag.Place)
                {
                    obj = DisplayList[placement.Depth];
                }
                else
                {
                    obj = new DisplayObject();
                    obj.shape = Editor.GetRuntimeShapeByCharacterId(placement.CharacterId);
                    if (obj.shape == null)
                    {
                        obj.sprite = Editor.GetRuntimeSpriteByCharacterId(placement.CharacterId);

                        if (obj.sprite == null)
                            obj.text = Editor.GetRuntimeTextByCharacterId(placement.CharacterId);
                    }


                    if (placement.NameId != -1)
                        obj.name = Editor.lm.Strings[placement.NameId];
                }

                if (placement.PositionFlags == 0x8000)
                {
                    obj.hasPos = true;
                    obj.pos = Editor.lm.Positions[placement.PositionId];
                }
                else if (placement.PositionFlags == 0x0000)
                {
                    obj.hasMatrix = true;
                    obj.matrix = Editor.lm.Transforms[placement.PositionId];
                }

                if (placement.ColorAddId != -1)
                {
                    obj.hasColor = true;
                    obj.colorAdd = Editor.lm.Colors[placement.ColorAddId];
                    obj.colorMult = Editor.lm.Colors[placement.ColorMultId];
                }

                obj.BlendMode = placement.BlendMode;

                DisplayList[placement.Depth] = obj;
            }

            foreach (var action in frame.Actions)
            {
                // HACK: this is true for *most* files. lol
                if (action.ActionId == 0)
                    Stop();
            }
        }

        public void Update()
        {
            if (!Playing)
                return;

            handleFrame(Sprite.Frames[CurrentFrame]);

            CurrentFrame++;
            CurrentFrame %= Sprite.Frames.Count;

            foreach (var obj in DisplayList.Values)
            {
                if (obj.sprite != null)
                    obj.sprite.Update();
            }
        }

        public void Render(RenderState state)
        {
            if (!Visible)
                return;

            foreach (var obj in DisplayList.Values)
            {
                GL.PushMatrix();

                if (obj.hasPos)
                    GL.Translate(obj.pos.X, obj.pos.Y, 0);
                if (obj.hasMatrix)
                    GL.MultMatrix(ref obj.matrix);

                var newState = new RenderState();
                newState.colorAdd = state.colorAdd;
                newState.colorMult = state.colorMult;

                if (obj.hasColor)
                {
                    newState.colorAdd = obj.colorAdd;
                    newState.colorMult = obj.colorMult;
                }

                GL.Uniform4(Editor.Shader.uColorAdd, newState.colorAdd);
                GL.Uniform4(Editor.Shader.uColorMul, newState.colorMult);

                Editor.SetBlendMode(obj.BlendMode);

                if (obj.sprite != null)
                    obj.sprite.Render(newState);
                else if (obj.shape != null)
                    obj.shape.Render();
                else if (obj.text != null)
                    obj.text.Render();

                GL.PopMatrix();
            }
        }
    }
}
