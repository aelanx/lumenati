using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;

namespace Lumenati
{
    public class Lumen
    {
        public enum TagType : int
        {
            Invalid = 0x0000,

            Fonts = 0x000A,
            Symbols = 0xF001,
            Colors = 0xF002,
            Transforms = 0xF003,
            Bounds = 0xF004,
            ActionScript = 0xF005,
            ActionScript2 = 0xFF05,
            TextureAtlases = 0xF007,
            UnkF008 = 0xF008,
            UnkF009 = 0xF009,
            UnkF00A = 0xF00A,
            UnkF00B = 0xF00B,
            Properties = 0xF00C,
            Defines = 0xF00D,

            Shape = 0xF022,
            Graphic = 0xF024,
            ColorMatrix = 0xF037,
            Positions = 0xF103,

            DynamicText = 0x0025,
            DefineSprite = 0x0027,

            FrameLabel = 0x002B,
            ShowFrame = 0x0001,
            Keyframe = 0xF105,
            PlaceObject = 0x0004,
            RemoveObject = 0x0005,
            DoAction = 0x000C,

            End = 0xFF00
        }

        public enum PlaceFlag : short
        {
            Move = 0x01,
            Place = 0x02
        }

        public enum BlendMode : short
        {
            Normal = 0x00,
            Layer = 0x02,
            Multiply = 0x03,
            Screen = 0x04,
            Lighten = 0x05,
            Darken = 0x06,
            Difference = 0x07,
            Add = 0x08,
            Subtract = 0x09,
            Invert = 0x0A,
            Alpha = 0x0B,
            Erase = 0x0C,
            Overlay = 0x0D,
            Hardlight = 0x0E
        }

        public class UnhandledTag
        {
            public UnhandledTag()
            {
                Type = TagType.Invalid;
            }

            public UnhandledTag(TagType type, int size, InputBuffer f)
            {
                Type = type;
                Size = size;
                Data = f.read(size * 4);
            }

            public UnhandledTag(TagType type, int size, byte[] data)
            {
                Type = type;
                Size = size;
                Data = data;
            }

            public UnhandledTag(InputBuffer f)
            {
                Type = (TagType)f.readInt();
                Size = f.readInt();
                Data = f.read(Size * 4);
            }

            public void Write(OutputBuffer o)
            {
                o.writeInt((int)Type);
                o.writeInt(Size);
                o.write(Data);
            }

            public TagType Type;
            public int Size;

            byte[] Data;
        }

        public class Properties
        {
            public uint unk0;
            public uint unk1;
            public uint unk2;
            public uint maxCharacterId;
            public int unk4;
            public uint maxCharacterId2;
            public ushort maxDepth;
            public ushort unk7;
            public float framerate;
            public float width;
            public float height;
            public uint unk8;
            public uint unk9;

            public Properties() { }

            public Properties(InputBuffer f)
            {
                Read(f);
            }

            public void Read(InputBuffer f)
            {
                unk0 = (uint)f.readInt();
                unk1 = (uint)f.readInt();
                unk2 = (uint)f.readInt();
                maxCharacterId = (uint)f.readInt();
                unk4 = f.readInt();
                maxCharacterId2 = (uint)f.readInt();
                maxDepth = (ushort)f.readShort();
                unk7 = (ushort)f.readShort();
                framerate = f.readFloat();
                width = f.readFloat();
                height = f.readFloat();
                unk8 = (uint)f.readInt();
                unk9 = (uint)f.readInt();
            }

            public void Write(OutputBuffer o)
            {
                o.writeInt((int)TagType.Properties);
                o.writeInt(12);

                o.writeInt((int)unk0);
                o.writeInt((int)unk1);
                o.writeInt((int)unk2);
                o.writeInt((int)maxCharacterId);
                o.writeInt(unk4);
                o.writeInt((int)maxCharacterId2);
                o.writeShort((short)maxDepth);
                o.writeShort((short)unk7);
                o.writeFloat(framerate);
                o.writeFloat(width);
                o.writeFloat(height);
                o.writeInt((int)unk8);
                o.writeInt((int)unk9);
            }
        }

        public class Properties2
        {
            public uint numShapes;
            public uint unk1;
            public uint numSprites;
            public uint unk3;
            public uint numTexts;
            public uint unk5;
            public uint unk6;
            public uint unk7;

            public Properties2() { }

            public Properties2(InputBuffer f)
            {
                Read(f);
            }

            public void Read(InputBuffer f)
            {
                numShapes = (uint)f.readInt();
                unk1 = (uint)f.readInt();
                numSprites = (uint)f.readInt();
                unk3 = (uint)f.readInt();
                numTexts = (uint)f.readInt();
                unk5 = (uint)f.readInt();
                unk6 = (uint)f.readInt();
                unk7 = (uint)f.readInt();
            }

            public void Write(OutputBuffer o)
            {
                o.writeInt((int)TagType.Defines);
                o.writeInt(8);

                o.writeInt((int)numShapes);
                o.writeInt((int)unk1);
                o.writeInt((int)numSprites);
                o.writeInt((int)unk3);
                o.writeInt((int)numTexts);
                o.writeInt((int)unk5);
                o.writeInt((int)unk6);
                o.writeInt((int)unk7);
            }
        }

        //public class Color
        //{
        //    public float R;
        //    public float G;
        //    public float B;
        //    public float A;

        //    public Color()
        //    {
        //    }

        //    public Color(float r, float g, float b, float a)
        //    {
        //        R = r;
        //        G = g;
        //        B = b;
        //        A = a;
        //    }

        //    public Color(short r, short g, short b, short a)
        //    {
        //        R = r / 256.0f;
        //        G = g / 256.0f;
        //        B = b / 256.0f;
        //        A = a / 256.0f;
        //    }

        //    public Color(uint rgba)
        //    {
        //        R = ((rgba >> 24) & 0xFF) / 255.0f;
        //        G = ((rgba >> 16) & 0xFF) / 255.0f;
        //        B = ((rgba >> 8) & 0xFF) / 255.0f;
        //        A = ((rgba) & 0xFF) / 255.0f;
        //    }

        //    public override string ToString()
        //    {
        //        return $"[{R}, {G}, {B}, {A}]";
        //    }

        //    public override bool Equals(object obj)
        //    {
        //        var other = (Color)obj;
        //        return (R == other.R && G == other.G && B == other.B && A == other.A);
        //    }

        //    public override int GetHashCode()
        //    {
        //        return (int)R ^ (int)G ^ (int)B ^ (int)A;
        //    }
        //}

        public class Rect
        {
            public float Left;
            public float Top;
            public float Right;
            public float Bottom;

            public Rect() { }

            public Rect(float l, float t, float r, float b)
            {
                Left = l;
                Top = t;
                Right = r;
                Bottom = b;
            }

        }

        public struct TextureAtlas
        {
            public int id;
            public int nameId;

            public float width;
            public float height;
        }

        public struct Vertex
        {
            public float X;
            public float Y;
            public float U;
            public float V;

            public Vertex(float x, float y, float u, float v)
            {
                X = x;
                Y = y;
                U = u;
                V = v;
            }
        }

        public enum FillType : short
        {
            Solid = 0x00,
            LinearGradient = 0x10,
            RadialGradient = 0x12,
            FocalRadialGradient = 0x13,
            RepeatingBitmap = 0x40,
            ClippedBitmap = 0x41,
            NonSmoothedRepeatingBitmap = 0x42,
            NonSmoothedClippedBitmap = 0x43
        }
        public class Graphic
        {

            public int NameId;
            public int AtlasId;
            public FillType FillType;

            public Vertex[] Verts;
            public ushort[] Indices;

            public Graphic()
            {

            }

            public Graphic(InputBuffer f)
            {
                Read(f);
            }

            public void Read(InputBuffer f)
            {
                AtlasId = f.readInt();
                FillType = (FillType)f.readShort();

                int numVerts = f.readShort();
                int numIndices = f.readInt();

                Verts = new Vertex[numVerts];
                Indices = new ushort[numIndices];

                for (int i = 0; i < numVerts; i++)
                {
                    Verts[i] = new Vertex();
                    Verts[i].X = f.readFloat();
                    Verts[i].Y = f.readFloat();
                    Verts[i].U = f.readFloat();
                    Verts[i].V = f.readFloat();
                }

                for (int i = 0; i < numIndices; i++)
                {
                    Indices[i] = (ushort)f.readShort();
                }

                // indices are padded to word boundaries
                if ((numIndices % 2) != 0)
                {
                    f.skip(0x02);
                }
            }

            public void Write(OutputBuffer o)
            {
                OutputBuffer tag = new OutputBuffer();
                tag.writeInt(AtlasId);
                tag.writeShort((short)FillType);
                tag.writeShort((short)Verts.Length);
                tag.writeInt(Indices.Length);

                foreach (var vert in Verts)
                {
                    tag.writeFloat(vert.X);
                    tag.writeFloat(vert.Y);
                    tag.writeFloat(vert.U);
                    tag.writeFloat(vert.V);
                }

                foreach (var index in Indices)
                {
                    tag.writeShort((short)index);
                }

                if ((Indices.Length % 2) != 0)
                {
                    tag.writeShort(0);
                }

                o.writeInt((int)TagType.Graphic);
                o.writeInt(tag.Size / 4);
                o.write(tag);
            }
        }

        public class Shape
        {
            public int CharacterId;
            public int Unk1;
            public int BoundsId;
            public int Unk3;

            public Graphic[] Graphics = new Graphic[0];

            public Shape() { }

            public Shape(InputBuffer f)
            {
                Read(f);
            }

            public void Read(InputBuffer f)
            {
                CharacterId = f.readInt();
                Unk1 = f.readInt();
                BoundsId = f.readInt();
                Unk3 = f.readInt();

                int numGraphics = f.readInt();
                Graphics = new Graphic[numGraphics];

                for (int i = 0; i < numGraphics; i++)
                {
                    f.skip(0x08); // graphic tag header
                    Graphics[i] = new Graphic(f);
                }
            }

            public void Write(OutputBuffer o)
            {
                o.writeInt((int)TagType.Shape);
                o.writeInt(5);

                o.writeInt(CharacterId);
                o.writeInt(Unk1);
                o.writeInt(BoundsId);
                o.writeInt(Unk3);
                o.writeInt(Graphics.Length);

                foreach (var graphic in Graphics)
                {
                    graphic.Write(o);
                }
            }
        }

        public struct DynamicText
        {
            public enum Alignment : short
            {
                Left = 0,
                Right = 1,
                Center = 2
            }

            public int characterId;
            public int unk1;
            public int placeholderTextId;
            public int unk2;
            public int colorId;
            public int unk3;
            public int unk4;
            public int unk5;
            public Alignment alignment;
            public short unk6;
            public int unk7;
            public int unk8;
            public float size;
            public int unk9;
            public int unk10;
            public int unk11;
            public int unk12;

            public DynamicText(InputBuffer f) : this()
            {
                Read(f);
            }

            public void Read(InputBuffer f)
            {
                characterId = f.readInt();
                unk1 = f.readInt();
                placeholderTextId = f.readInt();
                unk2 = f.readInt();
                colorId = f.readInt();
                unk3 = f.readInt();
                unk4 = f.readInt();
                unk5 = f.readInt();
                alignment = (Alignment)f.readShort();
                unk6 = f.readShort();
                unk7 = f.readInt();
                unk8 = f.readInt();
                size = f.readFloat();
                unk9 = f.readInt();
                unk10 = f.readInt();
                unk11 = f.readInt();
                unk12 = f.readInt();
            }

            public void Write(OutputBuffer o)
            {
                o.writeInt((int)TagType.DynamicText);
                o.writeInt(16);

                o.writeInt(characterId);
                o.writeInt(unk1);
                o.writeInt(placeholderTextId);
                o.writeInt(unk2);
                o.writeInt(colorId);
                o.writeInt(unk3);
                o.writeInt(unk4);
                o.writeInt(unk5);
                o.writeShort((short)alignment);
                o.writeShort(unk6);
                o.writeInt(unk7);
                o.writeInt(unk8);
                o.writeFloat(size);
                o.writeInt(unk9);
                o.writeInt(unk10);
                o.writeInt(unk11);
                o.writeInt(unk12);
            }
        }

        public class Sprite
        {
            public class Label
            {
                public int NameId;
                public int StartFrame;
                public int Unk1;

                public Label() { }

                public Label(InputBuffer f)
                {
                    Read(f);
                }

                public void Read(InputBuffer f)
                {
                    NameId = f.readInt();
                    StartFrame = f.readInt();
                    Unk1 = f.readInt();
                }

                public void Write(OutputBuffer o)
                {
                    o.writeInt((int)TagType.FrameLabel);
                    o.writeInt(3);
                    o.writeInt(NameId);
                    o.writeInt(StartFrame);
                    o.writeInt(Unk1);
                }
            }

            public class PlaceObject
            {
                public int CharacterId;
                public int PlacementId;
                public int Unk1;
                public int NameId;
                public PlaceFlag Flags;
                public BlendMode BlendMode;
                public short Depth;
                public short Unk4;

                public short Unk5;
                public short Unk6;
                public ushort PositionFlags;
                public short PositionId;
                public int ColorMultId;
                public int ColorAddId;

                public UnhandledTag ColorMatrix = null;
                public UnhandledTag UnkF014 = null;

                public PlaceObject() { }

                public PlaceObject(InputBuffer f) : this()
                {
                    Read(f);
                }

                public void Read(InputBuffer f)
                {
                    CharacterId = f.readInt();
                    PlacementId = f.readInt();
                    Unk1 = f.readInt();
                    NameId = f.readInt();
                    Flags = (PlaceFlag)f.readShort();
                    BlendMode = (BlendMode)f.readShort();
                    Depth = f.readShort();
                    Unk4 = f.readShort();
                    Unk5 = f.readShort();
                    Unk6 = f.readShort();
                    PositionFlags = (ushort)f.readShort();
                    PositionId = f.readShort();
                    ColorMultId = f.readInt();
                    ColorAddId = f.readInt();

                    bool hasColorMatrix = (f.readInt() == 1);
                    bool hasUnkF014 = (f.readInt() == 1);

                    if (hasColorMatrix)
                        ColorMatrix = new UnhandledTag(f);

                    if (hasUnkF014)
                        UnkF014 = new UnhandledTag(f);
                }

                public void Write(OutputBuffer o)
                {
                    o.writeInt((int)TagType.PlaceObject);
                    o.writeInt(12);

                    o.writeInt(CharacterId);
                    o.writeInt(PlacementId);
                    o.writeInt(Unk1);
                    o.writeInt(NameId);
                    o.writeShort((short)Flags);
                    o.writeShort((short)BlendMode);
                    o.writeShort(Depth);
                    o.writeShort(Unk4);
                    o.writeShort(Unk5);
                    o.writeShort(Unk6);
                    o.writeShort((short)PositionFlags);
                    o.writeShort(PositionId);
                    o.writeInt(ColorMultId);
                    o.writeInt(ColorAddId);

                    o.writeInt((ColorMatrix != null) ? 1 : 0);
                    o.writeInt((UnkF014 != null) ? 1 : 0);

                    if (ColorMatrix != null)
                        ColorMatrix.Write(o);

                    if (UnkF014 != null)
                        UnkF014.Write(o);
                }
            }

            public class RemoveObject
            {
                public int Unk1;
                public short Depth;
                public short Unk2;

                public RemoveObject(InputBuffer f)
                {
                    Read(f);
                }

                public void Read(InputBuffer f)
                {
                    Unk1 = f.readInt();
                    Depth = f.readShort();
                    Unk2 = f.readShort();
                }

                public void Write(OutputBuffer o)
                {
                    o.writeInt((int)TagType.RemoveObject);
                    o.writeInt(2);
                    o.writeInt(Unk1);
                    o.writeShort(Depth);
                    o.writeShort(Unk2);
                }
            }

            public class DoAction
            {
                public int ActionId;
                public int Unk1;

                public DoAction() { }

                public DoAction(InputBuffer f)
                {
                    Read(f);
                }

                public void Read(InputBuffer f)
                {
                    ActionId = f.readInt();
                    Unk1 = f.readInt();
                }

                public void Write(OutputBuffer o)
                {
                    o.writeInt((int)TagType.DoAction);
                    o.writeInt(2);
                    o.writeInt(ActionId);
                    o.writeInt(Unk1);
                }
            }

            public class Frame
            {
                public int Id;

                public List<RemoveObject> Removals = new List<RemoveObject>();
                public List<DoAction> Actions = new List<DoAction>();
                public List<PlaceObject> Placements = new List<PlaceObject>();

                public Frame() { }

                public Frame(InputBuffer f) : this()
                {
                    Read(f);
                }

                public void Read(InputBuffer f)
                {
                    Id = f.readInt();
                    int numChildren = f.readInt();

                    for (int childId = 0; childId < numChildren; childId++)
                    {
                        TagType childType = (TagType)f.readInt();
                        int childSize = f.readInt();

                        if (childType == TagType.RemoveObject)
                        {
                            Removals.Add(new RemoveObject(f));
                        }
                        else if (childType == TagType.DoAction)
                        {
                            Actions.Add(new DoAction(f));
                        }
                        else if (childType == TagType.PlaceObject)
                        {
                            Placements.Add(new PlaceObject(f));
                        }
                    }
                }

                // NOTE: unlike other tag write functions, this does not include the header
                // so it can be used for both frames and keyframes.
                public void Write(OutputBuffer o)
                {
                    o.writeInt(Id);
                    o.writeInt(Removals.Count + Actions.Count + Placements.Count);

                    foreach (var deletion in Removals)
                        deletion.Write(o);

                    foreach (var action in Actions)
                        action.Write(o);

                    foreach (var placement in Placements)
                        placement.Write(o);
                }
            }

            public int CharacterId;
            public int unk1;
            public int unk2;
            public int unk3;

            public List<Label> labels = new List<Label>();
            public List<Frame> Frames = new List<Frame>();
            public List<Frame> Keyframes = new List<Frame>();

            public Sprite()
            {
            }

            public Sprite(InputBuffer f) : this()
            {
                Read(f);
            }

            public void Read(InputBuffer f)
            {
                CharacterId = f.readInt();
                unk1 = f.readInt();
                unk2 = f.readInt();

                int numLabels = f.readInt();
                int numFrames = f.readInt();
                int numKeyframes = f.readInt();

                unk3 = f.readInt();

                for (int i = 0; i < numLabels; i++)
                {
                    f.skip(0x08);

                    labels.Add(new Label(f));
                }

                int totalFrames = numFrames + numKeyframes;
                for (int frameId = 0; frameId < totalFrames; frameId++)
                {
                    TagType frameType = (TagType)f.readInt();
                    f.skip(0x04); // size

                    Frame frame = new Frame(f);

                    if (frameType == TagType.Keyframe)
                        Keyframes.Add(frame);
                    else
                        Frames.Add(frame);
                }
            }

            public void Write(OutputBuffer o)
            {
                o.writeInt((int)TagType.DefineSprite);
                o.writeInt(7);
                o.writeInt(CharacterId);
                o.writeInt(unk1);
                o.writeInt(unk2);
                o.writeInt(labels.Count);
                o.writeInt(Frames.Count);
                o.writeInt(Keyframes.Count);
                o.writeInt(unk3);

                foreach (var label in labels)
                {
                    label.Write(o);
                }

                foreach (var frame in Frames)
                {
                    o.writeInt((int)TagType.ShowFrame);
                    o.writeInt(2);
                    frame.Write(o);
                }

                foreach (var frame in Keyframes)
                {
                    o.writeInt((int)TagType.Keyframe);
                    o.writeInt(2);
                    frame.Write(o);
                }
            }
        }

        public class Header
        {
            public int magic;
            public int unk0;
            public int unk1;
            public int unk2;
            public int unk3;
            public int unk4;
            public int unk5;
            public int filesize;
            public int unk6;
            public int unk7;
            public int unk8;
            public int unk9;
            public int unk10;
            public int unk11;
            public int unk12;
            public int unk13;

            public void Write(OutputBuffer o)
            {
                o.writeInt(magic);
                o.writeInt(unk0);
                o.writeInt(unk1);
                o.writeInt(unk2);
                o.writeInt(unk3);
                o.writeInt(unk4);
                o.writeInt(unk5);
                o.writeInt(filesize);
                o.writeInt(unk6);
                o.writeInt(unk7);
                o.writeInt(unk8);
                o.writeInt(unk9);
                o.writeInt(unk10);
                o.writeInt(unk11);
                o.writeInt(unk12);
                o.writeInt(unk13);
            }
        }

        public string Filename;
        public Header header = new Header();
        public List<string> Strings = new List<string>();
        public List<Vector4> Colors = new List<Vector4>();
        public List<Matrix4> Transforms = new List<Matrix4>();
        public List<Vector2> Positions = new List<Vector2>();
        public List<Rect> Bounds = new List<Rect>();
        public List<TextureAtlas> Atlases = new List<TextureAtlas>();
        public List<Shape> Shapes = new List<Shape>();
        public List<DynamicText> Texts = new List<DynamicText>();
        public List<Sprite> Sprites = new List<Sprite>();

        public Properties properties = new Properties();
        public UnhandledTag Actionscript;
        public UnhandledTag Actionscript2;
        public UnhandledTag unkF008;
        public UnhandledTag unkF009;
        public UnhandledTag unkF00A;
        public UnhandledTag unk000A;
        public UnhandledTag unkF00B;
        public Properties2 Defines = new Properties2();

        public Lumen() { }

        public Lumen(string filename)
        {
            Filename = filename;
            Read(filename);
        }

        public int AddPosition(Vector2 pos)
        {
            int index = -1;

            if (Positions.Contains(pos))
            {
                index = Positions.IndexOf(pos);
            }
            else
            {
                index = Positions.Count;
                Positions.Add(pos);
            }

            return index;
        }

        public int AddString(string str)
        {
            int index = -1;

            if (Strings.Contains(str))
            {
                index = Strings.IndexOf(str);
            }
            else
            {
                index = Strings.Count;
                Strings.Add(str);
            }

            return index;
        }

        public int AddColor(Vector4 color)
        {
            int index = -1;

            if (Colors.Contains(color))
            {
                index = Colors.IndexOf(color);
            }
            else
            {
                index = Colors.Count;
                Colors.Add(color);
            }

            return index;
        }

        public int AddTransform(Matrix4 xform)
        {
            int index = -1;

            if (Transforms.Contains(xform))
            {
                index = Transforms.IndexOf(xform);
            }
            else
            {
                index = Transforms.Count;
                Transforms.Add(xform);
            }

            return index;
        }

        public void Read(string filename)
        {
            InputBuffer f = new InputBuffer(filename);
            header.magic = f.readInt();
            header.unk0 = f.readInt();
            header.unk1 = f.readInt();
            header.unk2 = f.readInt();
            header.unk3 = f.readInt();
            header.unk4 = f.readInt();
            header.unk5 = f.readInt();
            header.filesize = f.readInt();
            header.unk6 = f.readInt();
            header.unk7 = f.readInt();
            header.unk8 = f.readInt();
            header.unk9 = f.readInt();
            header.unk10 = f.readInt();
            header.unk11 = f.readInt();
            header.unk12 = f.readInt();
            header.unk13 = f.readInt();

            bool done = false;
            while (!done)
            {
                uint tagOffset = f.ptr;

                TagType tagType = (TagType)f.readInt();
                int tagSize = f.readInt(); // in dwords!

                switch (tagType)
                {
                    case TagType.Invalid:
                    {
                        // uhhh. i think there's a specific exception for this
                        throw new Exception("Malformed file");
                    }

                    case TagType.Symbols:
                    {
                        int numSymbols = f.readInt();

                        while (Strings.Count < numSymbols)
                        {
                            int len = f.readInt();

                            Strings.Add(f.readString());
                            f.skip(4 - (f.ptr % 4));
                        }

                        break;
                    }

                    case TagType.Colors:
                    {
                        int numColors = f.readInt();

                        for (int i = 0; i < numColors; i++)
                        {
                            AddColor(new Vector4(f.readShort() / 256f, f.readShort() / 256f, f.readShort() / 256f, f.readShort() / 256f));
                        }

                        break;
                    }

                    case TagType.Fonts:
                    {
                        unk000A = new UnhandledTag(tagType, tagSize, f);
                        break;
                    }
                    case TagType.UnkF00A:
                    {
                        unkF00A = new UnhandledTag(tagType, tagSize, f);
                        break;
                    }
                    case TagType.UnkF00B:
                    {
                        unkF00B = new UnhandledTag(tagType, tagSize, f);
                        break;
                    }
                    case TagType.UnkF008:
                    {
                        unkF008 = new UnhandledTag(tagType, tagSize, f);
                        break;
                    }
                    case TagType.UnkF009:
                    {
                        unkF009 = new UnhandledTag(tagType, tagSize, f);
                        break;
                    }
                    case TagType.Defines:
                    {
                        Defines = new Properties2(f);
                        break;
                    }
                    case TagType.ActionScript:
                    {
                        Actionscript = new UnhandledTag(tagType, tagSize, f);
                        break;
                    }
                    case TagType.ActionScript2:
                    {
                        Actionscript2 = new UnhandledTag(tagType, tagSize, f);
                        break;
                    }

                    case TagType.End:
                    {
                        done = true;
                        break;
                    }

                    case TagType.Transforms:
                    {
                        int numTransforms = f.readInt();

                        for (int i = 0; i < numTransforms; i++)
                        {
                            float a = f.readFloat();
                            float b = f.readFloat();
                            float c = f.readFloat();
                            float d = f.readFloat();
                            float x = f.readFloat();
                            float y = f.readFloat();

                            var mat = new Matrix4(
                                a, b, 0, 0,
                                c, d, 0, 0,
                                x, y, 1, 0,
                                0, 0, 0, 1
                            );

                            Transforms.Add(mat);
                        }

                        break;
                    }

                    case TagType.Positions:
                    {
                        int numPositions = f.readInt();

                        for (int i = 0; i < numPositions; i++)
                        {
                            Positions.Add(new Vector2(f.readFloat(), f.readFloat()));
                        }

                        break;
                    }

                    case TagType.Bounds:
                    {
                        int numBounds = f.readInt();

                        for (int i = 0; i < numBounds; i++)
                        {
                            Bounds.Add(new Rect(f.readFloat(), f.readFloat(), f.readFloat(), f.readFloat()));
                        }
                        break;
                    }

                    case TagType.Properties:
                    {
                        properties = new Properties(f);
                        break;
                    }

                    case TagType.TextureAtlases:
                    {
                        int numAtlases = f.readInt();

                        for (int i = 0; i < numAtlases; i++)
                        {
                            TextureAtlas atlas = new TextureAtlas();
                            atlas.id = f.readInt();
                            atlas.nameId = f.readInt();
                            atlas.width = f.readFloat();
                            atlas.height = f.readFloat();

                            Atlases.Add(atlas);
                        }

                        break;
                    }

                    case TagType.Shape:
                    {
                        Shapes.Add(new Shape(f));
                        break;
                    }

                    case TagType.DynamicText:
                    {
                        Texts.Add(new DynamicText(f));
                        break;
                    }

                    case TagType.DefineSprite:
                    {
                        Sprites.Add(new Sprite(f));
                        break;
                    }

                    default:
                    {
                        throw new NotImplementedException($"Unhandled tag id: 0x{(uint)tagType:X} @ 0x{tagOffset:X}");
                    }
                }
            }
        } // Read()

        #region serialization
        void writeSymbols(OutputBuffer o)
        {
            OutputBuffer tag = new OutputBuffer();
            tag.writeInt(Strings.Count);

            foreach (var symbol in Strings)
            {
                tag.writeInt(symbol.Length);
                tag.writeString(symbol);

                int padSize = 4 - (tag.Size % 4);
                for (int i = 0; i < padSize; i++)
                {
                    tag.writeByte(0);
                }
            }

            o.writeInt((int)TagType.Symbols);
            o.writeInt(tag.Size / 4);
            o.write(tag);
        }

        void writeColors(OutputBuffer o)
        {
            o.writeInt((int)TagType.Colors);
            o.writeInt(Colors.Count * 2 + 1);
            o.writeInt(Colors.Count);

            foreach (var color in Colors)
            {
                o.writeShort((short)(color.X * 256));
                o.writeShort((short)(color.Y * 256));
                o.writeShort((short)(color.Z * 256));
                o.writeShort((short)(color.W * 256));
            }
        }

        void writePositions(OutputBuffer o)
        {
            o.writeInt((int)TagType.Positions);
            o.writeInt(Positions.Count * 2 + 1);
            o.writeInt(Positions.Count);

            foreach (var position in Positions)
            {
                o.writeFloat(position.X);
                o.writeFloat(position.Y);
            }
        }

        void writeTransforms(OutputBuffer o)
        {
            o.writeInt((int)TagType.Transforms);
            o.writeInt(Transforms.Count * 6 + 1);
            o.writeInt(Transforms.Count);

            foreach (var transform in Transforms)
            {
                o.writeFloat(transform.M11);
                o.writeFloat(transform.M12);
                o.writeFloat(transform.M21);
                o.writeFloat(transform.M22);
                o.writeFloat(transform.M31);
                o.writeFloat(transform.M32);
            }
        }

        void writeBounds(OutputBuffer o)
        {
            o.writeInt((int)TagType.Bounds);
            o.writeInt(Bounds.Count * 4 + 1);
            o.writeInt(Bounds.Count);

            foreach (var extent in Bounds)
            {
                o.writeFloat(extent.Left);
                o.writeFloat(extent.Top);
                o.writeFloat(extent.Right);
                o.writeFloat(extent.Bottom);
            }
        }

        void writeAtlases(OutputBuffer o)
        {
            o.writeInt((int)TagType.TextureAtlases);
            o.writeInt(Atlases.Count * 4 + 1);
            o.writeInt(Atlases.Count);

            foreach (var atlas in Atlases)
            {
                o.writeInt(atlas.id);
                o.writeInt(atlas.nameId);
                o.writeFloat(atlas.width);
                o.writeFloat(atlas.height);
            }
        }

        void writeShapes(OutputBuffer o)
        {
            foreach (var shape in Shapes) shape.Write(o);
            {

            }
        }

        void writeSprites(OutputBuffer o)
        {
            foreach (var mc in Sprites)
            {
                mc.Write(o);
            }
        }

        void writeTexts(OutputBuffer o)
        {
            foreach (var text in Texts)
            {
                text.Write(o);
            }
        }

        #endregion

        public byte[] Rebuild()
        {
            OutputBuffer o = new OutputBuffer();

            // TODO: write correct filesize in header.
            // It isn't checked by the game, but what the hell, right?
            header.Write(o);

            writeSymbols(o);
            writeColors(o);
            writeTransforms(o);
            writePositions(o);
            writeBounds(o);

            Actionscript.Write(o);
            if (Actionscript2 != null)
                Actionscript2.Write(o);

            writeAtlases(o);

            unkF008.Write(o);
            unkF009.Write(o);
            unkF00A.Write(o);
            unk000A.Write(o);
            unkF00B.Write(o);
            properties.Write(o);

            Defines.numShapes = (uint)Shapes.Count;
            Defines.numSprites = (uint)Sprites.Count;
            Defines.numTexts = (uint)Texts.Count;
            Defines.Write(o);

            writeShapes(o);
            writeSprites(o);
            writeTexts(o);

            o.writeInt((int)TagType.End);
            o.writeInt(0);
            o.writeInt(0);

            int padSize = (4 - (o.Size % 4)) % 4;
            for (int i = 0; i < padSize; i++)
            {
                o.writeByte(0);
            }

            return o.getBytes();
        }
    }
}
