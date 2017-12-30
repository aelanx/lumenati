using System;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Lumenati
{
    public class GTX
    {
        public static int getBPP(PixelInternalFormat format)
        {
            switch (format)
            {
                case PixelInternalFormat.CompressedRgbaS3tcDxt3Ext:
                case PixelInternalFormat.CompressedRgbaS3tcDxt5Ext:
                case PixelInternalFormat.CompressedRgRgtc2:
                    return 0x80;
                case PixelInternalFormat.CompressedRgbaS3tcDxt1Ext:
                case PixelInternalFormat.CompressedRedRgtc1:
                    return 0x40;
                case PixelInternalFormat.Rgba:
                    return 0x20;
                default:
                    throw new NotImplementedException();
            }
        }

        public static byte[] swizzle(byte[] data, int width, int height, PixelInternalFormat format, int pitch, int swizzleIn)
        {
            byte[] swizzledData = new byte[data.Length];

            int swizzle = (swizzleIn & 0x700) >> 8;
            int bpp = getBPP(format);
            int blockSize = bpp / 8;

            if (format != PixelInternalFormat.Rgba)
            {
                width /= 4;
                height /= 4;
            }

            Parallel.For(0, width * height, i =>
            {
                int pos = surfaceAddrFromCoordMacroTiled(i % width, i / width, bpp, pitch, swizzle);
                Array.Copy(data, pos, swizzledData, i * blockSize, blockSize);
            });

            return swizzledData;
        }

        public static int surfaceAddrFromCoordMacroTiled(int x, int y, int bpp, int pitch, int swizzle)
        {
            int pipe = ((y ^ x) >> 3) & 1;
            int bank = (((y / 32) ^ (x >> 3)) & 1) | ((y ^ x) & 16) >> 3;
            int bankPipe = ((pipe + bank * 2) ^ swizzle) % 9;
            int macroTileBytes = (bpp * 512 + 7) >> 3;
            int macroTileOffset = (x / 32 + pitch / 32 * (y / 16)) * macroTileBytes;
            int unk = (bpp * getPixelIndex(x, y, bpp) + macroTileOffset) >> 3;

            return (bankPipe << 8) | ((bankPipe % 2) << 8) | ((unk & ~0xFF) << 3) | (unk & 0xFF);
        }

        static int getPixelIndex(int x, int y, int bpp)
        {
            if (bpp == 0x80)
                return ((x & 7) << 1) | ((y & 6) << 3) | (y & 1);
            else if (bpp == 0x40)
                return ((x & 6) << 1) | (x & 1) | ((y & 6) << 3) | ((y & 1) << 1);
            else if (bpp == 0x20)
                return ((x & 4) << 1) | (x & 3) | ((y & 6) << 3) | ((y & 1) << 2);

            return 0;
        }
    }
}