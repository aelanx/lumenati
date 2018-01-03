using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace Lumenati
{
    public class SelectedVertex
    {
        public Lumen.Graphic graphic;
        public int vertId;
    }

    public enum EditorMode
    {
        Vertex,
        UV
    }

    public class LumenEditor
    {
        string originalFilename;
        string filePath;
        public Lumen lm;
        public Texlist texlist;
        Dictionary<int, Nut> atlases = new Dictionary<int, Nut>();
        public List<RuntimeSprite> RuntimeSprites = new List<RuntimeSprite>();
        public List<RuntimeShape> RuntimeShapes = new List<RuntimeShape>();
        public LumenShader Shader;

        // mode shit
        public List<SelectedVertex> SelectedVerts = new List<SelectedVertex>();
        public EditorMode Mode = EditorMode.UV;

        // short term
        public Vector3 dragPosition = Vector3.Zero;

        public void LoadFile(string filename)
        {
            originalFilename = filename;
            filePath = Path.GetDirectoryName(filename);
            lm = new Lumen(filename);
            texlist = new Texlist(Path.Combine(filePath, "texlist.lst"));
            Shader = new LumenShader();
            Shader.EnableAttrib();
            GL.UseProgram(Shader.ProgramID);
            GL.Uniform4(Shader.uColorAdd, new Vector4(0, 0, 0, 0));
            GL.Uniform4(Shader.uColorMul, new Vector4(1, 1, 1, 1));
            GL.Uniform1(Shader.uTex, 0);
            //GL.UseProgram(0);

            foreach (var sprite in lm.Sprites)
            {
                var rs = new RuntimeSprite(this, sprite);
                RuntimeSprites.Add(rs);
            }

            foreach (var shape in lm.Shapes)
            {
                var rs = new RuntimeShape(this, shape);
                RuntimeShapes.Add(rs);
            }

            atlases.Clear();
        }

        public Lumen.Shape GetShapeByCharacterId(int characterId)
        {
            foreach (var shape in lm.Shapes)
            {
                if (shape.CharacterId == characterId)
                    return shape;
            }

            return null;
        }

        public Lumen.Sprite GetSpriteByCharacterId(int characterId)
        {
            foreach (var sprite in lm.Sprites)
            {
                if (sprite.CharacterId == characterId)
                    return sprite;
            }

            return null;
        }

        public RuntimeSprite GetRuntimeSpriteByCharacterId(int characterId)
        {
            foreach (var sprite in RuntimeSprites)
            {
                if (sprite.CharacterId == characterId)
                    return sprite;
            }

            return null;
        }

        public RuntimeShape GetRuntimeShapeByCharacterId(int characterId)
        {
            foreach (var shape in RuntimeShapes)
            {
                if (shape.CharacterId == characterId)
                    return shape;
            }

            return null;
        }

        public Nut GetAtlas(int id)
        {
            if (!atlases.ContainsKey(id))
            {
                var filename = Path.Combine(Path.GetDirectoryName(lm.Filename), $"img-{id:d5}.nut");

                if (File.Exists(filename))
                    atlases[id] = new Nut(filename);
                else
                    atlases[id] = null;
            }

            return atlases[id];
        }

        public void SaveFile(string filename = null)
        {
            if (filename == null)
                filename = originalFilename;

            using (var stream = new FileStream(filename, FileMode.Create))
            using (var writer = new BinaryWriter(stream))
                writer.Write(lm.Rebuild());

            var texlistFilename = Path.Combine(Path.GetDirectoryName(filename), "texlist.lst");
            using (var stream = new FileStream(texlistFilename, FileMode.Create))
            using (var writer = new BinaryWriter(stream))
                writer.Write(texlist.Rebuild());
        }

        public void MoveSelectionBy(float x, float y)
        {
            foreach (var v in SelectedVerts)
            {
                if (Mode == EditorMode.Vertex)
                {
                    v.graphic.Verts[v.vertId].X += x;
                    v.graphic.Verts[v.vertId].Y += y;
                }
                else if (Mode == EditorMode.UV)
                {
                    v.graphic.Verts[v.vertId].U += (x / lm.Atlases[v.graphic.AtlasId].width);
                    v.graphic.Verts[v.vertId].V += (y / lm.Atlases[v.graphic.AtlasId].height);
                }
            }
        }

        public void DrawSquare(float size, float x, float y)
        {
            float hs = size / /*scale / */2;
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(x - hs, y - hs);
            GL.Vertex2(x + hs, y - hs);
            GL.Vertex2(x + hs, y + hs);
            GL.Vertex2(x - hs, y + hs);
            GL.End();
        }

        public void DrawBounds(Lumen.Rect rect)
        {
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(rect.Left, rect.Top);
            GL.Vertex2(rect.Right, rect.Top);
            GL.Vertex2(rect.Right, rect.Bottom);
            GL.Vertex2(rect.Left, rect.Bottom);
            GL.End();

            DrawSquare(8, rect.Left, rect.Top);
            DrawSquare(8, rect.Right, rect.Top);
            DrawSquare(8, rect.Right, rect.Bottom);
            DrawSquare(8, rect.Left, rect.Bottom);
        }

        public void DrawGraphic(Lumen.Graphic gfx, PrimitiveType primitiveType)
        {
            GL.Begin(primitiveType);
            foreach (var idx in gfx.Indices)
            {
                var vert = gfx.Verts[idx];
                GL.TexCoord2(vert.U, vert.V);
                GL.Vertex2(vert.X, vert.Y);
            }
            GL.End();
            //GL.UseProgram(0);
        }

        public void DrawGraphicHandles(Lumen.Graphic graphic)
        {
            if (Mode == EditorMode.Vertex)
                DrawGraphicVerts(graphic);
            else if (Mode == EditorMode.UV)
                DrawGraphicUVs(graphic);
        }

        void DrawGraphicVerts(Lumen.Graphic graphic)
        {
            var nut = GetAtlas(graphic.AtlasId);

            if (nut != null)
            {
                GL.Color3(Color.White);
                GL.BindTexture(TextureTarget.Texture2D, nut.glId);
                DrawGraphic(graphic, PrimitiveType.Triangles);
            }

            GL.Color3(Color.Green);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            DrawGraphic(graphic, PrimitiveType.Triangles);

            GL.Color3(Color.White);
            foreach (var vert in graphic.Verts)
            {
                DrawSquare(8, vert.X, vert.Y);
            }

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            foreach (var vertSelection in SelectedVerts)
            {
                var v = vertSelection.graphic.Verts[vertSelection.vertId];
                DrawSquare(8, v.X + dragPosition.X, v.Y + dragPosition.Y);
            }
        }

        void DrawGraphicUVs(Lumen.Graphic graphic)
        {
            var nut = GetAtlas(graphic.AtlasId); // if a shape uses multiple atlases, fuck it.

            if (nut != null)
            {
                // Draw nut
                GL.Color3(Color.White);
                GL.BindTexture(TextureTarget.Texture2D, nut.glId);
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, 0);
                GL.Vertex2(0, 0);
                GL.TexCoord2(1, 0);
                GL.Vertex2(nut.width, 0);
                GL.TexCoord2(1, 1);
                GL.Vertex2(nut.width, nut.height);
                GL.TexCoord2(0, 1);
                GL.Vertex2(0, nut.height);
                GL.End();

                // 
                GL.Color3(Color.White);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.Begin(PrimitiveType.Triangles);
                foreach (var idx in graphic.Indices)
                {
                    var vert = graphic.Verts[idx];
                    GL.Vertex2(vert.U * nut.width, vert.V * nut.height);
                }
                GL.End();

                GL.Color3(Color.White);
                foreach (var vert in graphic.Verts)
                {
                    DrawSquare(8, vert.U * nut.width, vert.V * nut.height);
                }

                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

                foreach (var vertSelection in SelectedVerts)
                {
                    var v = vertSelection.graphic.Verts[vertSelection.vertId];
                    var x = v.U * lm.Atlases[graphic.AtlasId].width;
                    var y = v.V * lm.Atlases[graphic.AtlasId].height;
                    DrawSquare(8, x + dragPosition.X, y + dragPosition.Y);
                }
            }
        }
    }
}
