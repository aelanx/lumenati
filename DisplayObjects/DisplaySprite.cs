using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace Lumenati
{
    public class DisplaySprite : DisplayObject
    {
        public bool Playing = false;
        public int CurrentFrame = 0;
        public bool Visible = true;

        //
        SortedDictionary<int, DisplayObject> DisplayList = new SortedDictionary<int, DisplayObject>();
        public Lumen.Sprite Sprite;

        public DisplaySprite(LumenEditor editor, Lumen.Sprite sprite) : base(editor, sprite.CharacterId)
        {
            Sprite = sprite;
        }

        public override DisplayObject Clone()
        {
            return new DisplaySprite(Editor, Sprite);
        }

        public Lumen.Sprite.Label getPrecedingLabel(int frameId)
        {
            Lumen.Sprite.Label precedingLabel = null;

            foreach (var label in Sprite.labels)
            {
                if (label.StartFrame == frameId)
                    return label;

                if (label.StartFrame < frameId)
                    precedingLabel = label;
                else
                    break;
            }

            return precedingLabel;
        }

        public void GotoFrame(int frameId)
        {
            if (frameId == CurrentFrame)
                return;

            // To get an accurate state, we need to move *forward* to the target frame,
            // simulating each along the way. Otherwise, we might miss a placement,
            // deletion, or action, leaving the display list completely borked.

            // Keyframes contain the full state of their equivalent frame, so we
            // jump back to the nearest keyframe and move forward if frameId < CurrentFrame.

            DisplayList.Clear();
            if (Sprite.labels.Count > 0)
            {
                Lumen.Sprite.Label precedingLabel = null;
                int precedingLabelId = -1;

                for (int i = 0; i < Sprite.labels.Count; i++)
                {
                    var label = Sprite.labels[i];

                    if (label.StartFrame < frameId)
                    {
                        precedingLabel = label;
                        precedingLabelId = i;
                    }
                    else
                    {
                        break;
                    }
                }

                if (precedingLabel != null)
                {
                    handleFrame(Sprite.Keyframes[precedingLabelId]);
                    CurrentFrame = precedingLabel.StartFrame;
                }
                else
                {
                    CurrentFrame = 0;
                    handleFrame(Sprite.Frames[0]);
                }
            }
            else
            {
                CurrentFrame = 0;
                handleFrame(Sprite.Frames[0]);
            }

            for (int i = CurrentFrame; i < frameId; i++)
            {
                handleFrame(Sprite.Frames[i]);
            }
            CurrentFrame = frameId;
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
                if (!(obj is DisplaySprite))
                    continue;

                var sprite = (DisplaySprite)obj;

                if (sprite.name == name)
                    return sprite;

                var r = sprite.SearchChild(name);
                if (r != null)
                    return r;
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
                    if (obj is DisplaySprite && obj.name == name)
                    {
                        currentSprite = (DisplaySprite)obj;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    return null;
            }

            return currentSprite;
        }

        public void Reset()
        {
            foreach (var child in DisplayList.Values)
            {
                if (child is DisplaySprite)
                {
                    ((DisplaySprite)child).Reset();
                }
            }

            DisplayList.Clear();
            Playing = false;
            CurrentFrame = 0;

            Init();
        }

        public void Init()
        {
            handleFrame(Sprite.Frames[0]);

            foreach (var child in DisplayList.Values)
            {
                if (child is DisplaySprite)
                    ((DisplaySprite)child).Init();
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
                    obj = Editor.CharacterDict[placement.CharacterId].Clone();

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
                CurrentFrame++;
                if (CurrentFrame >= Sprite.Frames.Count)
                {
                    CurrentFrame = Sprite.Frames.Count - 1;
                    Stop();
                }

                handleFrame(Sprite.Frames[CurrentFrame]);
            }

            foreach (var obj in DisplayList.Values)
            {
                if (obj is DisplaySprite)
                    ((DisplaySprite)obj).Update();
            }
        }

        public override void Render(RenderState state)
        {
            if (!Visible)
                return;

            foreach (var obj in DisplayList.Values)
            {
                GL.PushMatrix();

                if (obj.hasPos)
                    GL.Translate(obj.pos.X, obj.pos.Y, 0);
                else if (obj.hasMatrix)
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

                obj.Render(newState);

                GL.PopMatrix();
            }
        }
    }
}
