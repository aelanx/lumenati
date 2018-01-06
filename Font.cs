using System.IO;
using System.Collections.Generic;

namespace Lumenati
{
    public class Font
    {
        public class Glyph
        {
            public float x;
            public float y;
            public short advance;
            public short unk1;

            public short unk2; // 21
            public short xBearing; // -6
            public short yBearing; // 5
            public short unk4; // usually 65 (A)
            public short width;
            public short height;

            public ushort codePoint; // BIG ENDIAN?
        }

        public float lineHeight;
        public float spaceWidth;
        public float defaultSize;
        public Nut Texture;
        public Dictionary<ushort, Glyph> Glyphs = new Dictionary<ushort, Glyph>();

        public Font(string filename)
        {
            var texName = Path.GetFileNameWithoutExtension(filename) + "_00000000.nut";
            texName = Path.Combine(Path.GetDirectoryName(filename), texName);
            Texture = new Nut(texName);
            Read(filename);
        }

        public void Read(string filename)
        {
            var file = new InputBuffer(filename);
            file.Endianness = Endian.Little;

            var magic = file.readInt();
            lineHeight = file.readShort();
            spaceWidth = file.readShort();
            var numGlyphs = file.readShort();
            defaultSize = file.readShort();
            file.skip(0x04); // pad

            for (int i = 0; i < numGlyphs; i++)
            {
                var glyph = new Glyph();
                file.skip(0x04); // pad
                glyph.x = file.readShort();
                glyph.y = file.readShort();
                glyph.advance = file.readShort();
                glyph.unk1 = file.readShort();
                glyph.unk2 = file.readShort();
                glyph.xBearing = file.readShort();
                glyph.yBearing = file.readShort();
                glyph.unk4 = file.readShort();
                glyph.width = file.readShort();
                glyph.height = file.readShort();
                glyph.codePoint = (ushort)file.readShortBE();
                file.skip(0x06); // pad

                Glyphs.Add(glyph.codePoint, glyph);
            }
        }
    }
}
