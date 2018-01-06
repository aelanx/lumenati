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

            public char codePoint; // BIG ENDIAN?
        }

        public float lineHeight;
        public float spaceWidth;
        public float defaultSize;
        public Nut Texture { get; }
        public Dictionary<char, Glyph> Glyphs { get; }

        public Font(string filename)
        {
            Glyphs = new Dictionary<char, Glyph>();

            var texName = Path.GetFileNameWithoutExtension(filename) + "_00000000.nut";
            texName = Path.Combine(Path.GetDirectoryName(filename), texName);
            Texture = new Nut(texName);
            Read(filename);

            //Texture = new Nut(Plugin.GetAsset($"data/ui/font/lumen/static/{name}/{name}_00000000.nut"));
            //Read(Plugin.GetAsset($"data/ui/font/lumen/static/{name}/{name}.fgb"));
        }

        public void Read(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open))
            using (var reader = new BinaryReader(stream))
            {
                stream.Seek(0x04, SeekOrigin.Current); // "FGB\0"
                lineHeight = reader.ReadInt16();
                spaceWidth = reader.ReadInt16();
                short numGlyphs = reader.ReadInt16();
                defaultSize = reader.ReadInt16();
                stream.Seek(0x04, SeekOrigin.Current); // pad

                for (int i = 0; i < numGlyphs; i++)
                {
                    var glyph = new Glyph();
                    stream.Seek(0x04, SeekOrigin.Current); // pad
                    glyph.x = reader.ReadInt16();
                    glyph.y = reader.ReadInt16();
                    glyph.advance = reader.ReadInt16();
                    glyph.unk1 = reader.ReadInt16();
                    glyph.unk2 = reader.ReadInt16();
                    glyph.xBearing = reader.ReadInt16();
                    glyph.yBearing = reader.ReadInt16();
                    glyph.unk4 = reader.ReadInt16();
                    glyph.width = reader.ReadInt16();
                    glyph.height = reader.ReadInt16();
                    glyph.codePoint = (char)reader.ReadInt16();
                    stream.Seek(0x06, SeekOrigin.Current); // pad

                    glyph.codePoint = (char)(((glyph.codePoint >> 8) & 0xFF) | (glyph.codePoint << 8));

                    Glyphs.Add(glyph.codePoint, glyph);
                }
            }
        }
    }
}
