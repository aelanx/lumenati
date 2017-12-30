using System;
using System.Collections.Generic;
using OpenTK;

namespace Lumenati
{
    public class Texlist
    {
        public enum AtlasFlag : int
        {
            None = 0x00000000,
            Dynamic = 0x01000000
        }

        public class Texture
        {
            public string name;

            public Vector2 topLeft;
            public Vector2 botRight;

            public short width;
            public short height;

            public short atlasId;
        }

        public List<Texture> textures = new List<Texture>();
        public List<AtlasFlag> atlases = new List<AtlasFlag>();

        public Texlist()
        {
        }

        public Texlist(string filename)
        {
            Read(filename);
        }

        public void Read(string filename)
        {
            InputBuffer buf = new InputBuffer(filename, Endian.Big);

            buf.ptr = 0x06;

            short numAtlases = buf.readShort();
            short numTextures = buf.readShort();
            short flagsOffset = buf.readShort();
            short entriesOffset = buf.readShort();
            short stringsOffset = buf.readShort();


            buf.ptr = (uint)flagsOffset;
            for (int i = 0; i < numAtlases; i++)
            {
                atlases.Add((AtlasFlag)buf.readInt());
            }

            buf.ptr = (uint)entriesOffset;
            for (int i = 0; i < numTextures; i++)
            {
                Texture entry = new Texture();
                int nameOffset = buf.readInt();
                int nameOffset2 = buf.readInt();

                // I have yet to see this.
                if (nameOffset != nameOffset2)
                {
                    throw new NotImplementedException("texlist name offsets don't match?");
                }

                entry.name = buf.readString(stringsOffset + nameOffset);

                entry.topLeft = new Vector2(buf.readFloat(), buf.readFloat());
                entry.botRight = new Vector2(buf.readFloat(), buf.readFloat());

                entry.width = buf.readShort();
                entry.height = buf.readShort();
                entry.atlasId = buf.readShort();

                textures.Add(entry);

                buf.skip(0x02); // Padding.
            }
        }

        public byte[] Rebuild()
        {
            OutputBuffer buf = new OutputBuffer();
            buf.Endianness = Endian.Big;

            var flagsOffset = 0x10;
            var entriesOffset = flagsOffset + (atlases.Count * 4);
            var stringsOffset = entriesOffset + (textures.Count * 0x20);

            buf.writeInt(0x544C5354); // TLST
            buf.writeShort(0); // idk
            buf.writeShort((short)atlases.Count);
            buf.writeShort((short)textures.Count);
            buf.writeShort((short)flagsOffset);
            buf.writeShort((short)entriesOffset);
            buf.writeShort((short)stringsOffset);

            // flags
            foreach (var flag in atlases)
            {
                buf.writeInt((int)flag);
            }

            // entries
            int namePtr = 0;
            foreach (var texture in textures)
            {
                buf.writeInt(namePtr);
                buf.writeInt(namePtr);
                namePtr += texture.name.Length + 1;

                buf.writeFloat(texture.topLeft.X);
                buf.writeFloat(texture.topLeft.Y);
                buf.writeFloat(texture.botRight.X);
                buf.writeFloat(texture.botRight.Y);

                buf.writeShort(texture.width);
                buf.writeShort(texture.height);
                buf.writeShort(texture.atlasId);
                buf.writeShort(0); // pad
            }

            //strings
            foreach (var texture in textures)
            {
                buf.writeString(texture.name);
                buf.writeByte(0);
            }

            buf.writeByte(0);

            return buf.getBytes();
        }
    }
}
