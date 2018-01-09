using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Lumenati
{
    public class DisplaySprite
    {
        public bool Playing = true;
        public int CurrentFrame = 0;
        public bool Visible = true;

        //
        public int CharacterId;
        SortedDictionary<int, DisplayObject> DisplayList = new SortedDictionary<int, DisplayObject>();
        LumenEditor Editor;
        Lumen.Sprite Sprite;

        public DisplaySprite(LumenEditor editor, Lumen.Sprite sprite)
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

        public DisplaySprite SearchChild(string name)
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

        public DisplaySprite GetPathMC(string path)
        {
            var names = path.Split('.');
            DisplaySprite currentSprite = this;

            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i];
                var found = false;

                foreach (var obj in currentSprite.DisplayList.Values)
                {
                    if (obj.name == name)
                    {
                        currentSprite = obj.sprite;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    return null;
            }

            return currentSprite;
        }

        public void Init()
        {
            if (Sprite.Keyframes.Count == 0)
                handleFrame(Sprite.Frames[0]);
            else
                handleFrame(Sprite.Keyframes[0]);

            foreach (var child in DisplayList.Values)
            {
                if (child.sprite != null)
                    child.sprite.Init();
            }
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

                if (placement.Flags == Lumen.PlaceFlag.Move)
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
            if (Playing)
            {
            handleFrame(Sprite.Frames[CurrentFrame]);

            CurrentFrame++;
            CurrentFrame %= Sprite.Frames.Count;

            }

            //if (CurrentFrame >= Sprite.Frames.Count)
            //    Stop();

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
