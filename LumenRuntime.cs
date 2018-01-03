using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Drawing;

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
            foreach (var graphic in Shape.Graphics)
            {
                var atlas = Editor.GetAtlas(graphic.AtlasId);
                if (atlas != null)
                    GL.BindTexture(TextureTarget.Texture2D, atlas.glId);
                else
                    GL.BindTexture(TextureTarget.Texture2D, 0);

                Editor.DrawGraphic(graphic, PrimitiveType.Triangles);
            }

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            //Editor.DrawBounds(Bounds);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
    }

    public class RuntimeSprite
    {
        public bool Playing = true;
        public int CurrentFrame = 0;
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

        public void Update()
        {
            if (!Playing)
                return;

            var frame = Sprite.Frames[CurrentFrame];

            foreach (var removal in frame.Removals)
            {
                if (DisplayList.ContainsKey(removal.Depth))
                    DisplayList.Remove(removal.Depth);
            }

            foreach (var placement in frame.Placements)
            {
                DisplayObject obj;

                if (DisplayList.ContainsKey(placement.Depth))
                {
                    obj = DisplayList[placement.Depth];
                }
                else
                {
                    obj = new DisplayObject();
                    obj.shape = Editor.GetRuntimeShapeByCharacterId(placement.CharacterId);
                    if (obj.shape == null)
                        obj.sprite = Editor.GetRuntimeSpriteByCharacterId(placement.CharacterId);

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
                    obj.colorAdd = Editor.lm.Colors[placement.ColorAddId];
                if (placement.ColorMultId != -1)
                    obj.colorMult = Editor.lm.Colors[placement.ColorMultId];

                DisplayList[placement.Depth] = obj;
            }

            foreach (var action in frame.Actions)
            {
                // HACK: this is true for *most* files. lol
                if (action.ActionId == 0)
                    Playing = false;
            }

            CurrentFrame++;
            CurrentFrame %= Sprite.Frames.Count;

            foreach (var obj in DisplayList.Values)
            {
                if (obj.sprite != null)
                    obj.sprite.Update();
            }
        }

        public void Render()
        {
            foreach (var obj in DisplayList.Values)
            {
                RenderDisplayObject(obj);
            }
        }

        void RenderDisplayObject(DisplayObject obj)
        {
            GL.PushMatrix();

            if (obj.hasPos)
                GL.Translate(obj.pos.X, obj.pos.Y, 0);
            if (obj.hasMatrix)
                GL.MultMatrix(ref obj.matrix);

            if (obj.colorAdd != null)
                GL.Uniform4(Editor.Shader.uColorAdd, obj.colorAdd);
            if (obj.colorMult != null)
                GL.Uniform4(Editor.Shader.uColorMul, obj.colorMult);

            if (obj.sprite != null)
                obj.sprite.Render();
            else if (obj.shape != null)
                obj.shape.Render();

            GL.PopMatrix();
        }
    }
}
