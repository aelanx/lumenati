using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using System.IO;

namespace Lumenati
{
    public class Nut
    {
        public byte[] data;
        public int id;
        public int width;
        public int height;
        public PixelInternalFormat type;
        public PixelFormat utype;
        public int glId = 0;
        public string filename;
        public Bitmap decompressedData = null;

        public Nut()
        {
        }

        public Nut(string filename)
        {
            Read(filename);
        }

        public void Read(string filename)
        {
            this.filename = filename;
            InputBuffer d = new InputBuffer(filename);

            int magic = d.readInt();

            if (magic == 0x4E545033)
                ReadNTP3(d);
            else if (magic == 0x4E545755)
                ReadNTWU(d);
            else
                throw new NotImplementedException("Unsupported Nut header");
        }

        public void setPixelFormatFromNutFormat(int typet)
        {
            switch (typet)
            {
                case 0x0:
                    type = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    break;
                case 0x1:
                    type = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    break;
                case 0x2:
                    type = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    break;
                case 14:
                    type = PixelInternalFormat.Rgba;
                    utype = PixelFormat.Rgba;
                    break;
                case 17:
                    type = PixelInternalFormat.Rgba;
                    utype = PixelFormat.Bgra;
                    break;
                case 21:
                    type = PixelInternalFormat.CompressedRedRgtc1;
                    break;
                case 22:
                    type = PixelInternalFormat.CompressedRgRgtc2;
                    break;
                default:
                    throw new NotImplementedException($"Unknown nut format {typet}");
            }
        }

        public int getNutFormat()
        {
            switch (type)
            {
                case PixelInternalFormat.CompressedRgbaS3tcDxt1Ext:
                    return 0;
                case PixelInternalFormat.CompressedRgbaS3tcDxt3Ext:
                    return 1;
                case PixelInternalFormat.CompressedRgbaS3tcDxt5Ext:
                    return 2;
                case PixelInternalFormat.Rgba:
                    return (utype == PixelFormat.Rgba) ? 14 : 17;
                case PixelInternalFormat.CompressedRedRgtc1:
                    return 21;
                case PixelInternalFormat.CompressedRgRgtc2:
                    return 22;
                default:
                    throw new NotImplementedException($"Unknown pixel format 0x{type:X}");
            }
        }

        public void ReadNTP3(InputBuffer d)
        {
            d.skip(0x14);
            int totalSize = d.readInt();
            int headerSize = d.readShort();

            d.skip(0x04); // numMips
            setPixelFormatFromNutFormat(d.readShort());
            width = d.readShort();
            height = d.readShort();

            d.skip(0x08);
            int dataOffset = d.readInt() + 0x10;

            d.skip((uint)headerSize - 0x50 + 0x24);
            id = d.readInt();
            data = d.slice(dataOffset, totalSize);

            BufferTexture();
        }

        public void ReadNTWU(InputBuffer buf)
        {
            buf.skip(0x18);

            int headerSize = buf.readShort();
            int numMips = buf.readInt();
            setPixelFormatFromNutFormat(buf.readShort());
            width = buf.readShort();
            height = buf.readShort();

            buf.skip(8); // mipmaps and padding
            int dataOffset = buf.readInt() + 0x10;

            buf.skip(0x04);
            int gtxHeaderOffset = buf.readInt() + 0x10;

            buf.skip(0x04);
            buf.skip((uint)headerSize - 0x50);

            buf.skip(0x18);
            id = buf.readInt();

            buf.ptr = (uint)gtxHeaderOffset;
            buf.skip(0x04); // dim
            buf.skip(0x04); // width
            buf.skip(0x04); // height
            buf.skip(0x04); // depth
            buf.skip(0x04); // numMips
            int format = buf.readInt();
            buf.skip(0x04); // aa
            buf.skip(0x04); // use
            int imageSize = buf.readInt();
            buf.skip(0x04); // imagePtr
            buf.skip(0x04); // mipSize
            buf.skip(0x04); // mipPtr
            buf.skip(0x04); // tileMode
            int swizzle = buf.readInt();
            buf.skip(0x04); // alignment
            int pitch = buf.readInt();

            data = GTX.swizzle(
                buf.slice(dataOffset, imageSize),
                width,
                height,
                type,
                pitch,
                swizzle
            );

            BufferTexture();
        }

        public byte[] Rebuild()
        {
            OutputBuffer o = new OutputBuffer();

            o.writeInt(0x4E545033); // "NTP3"
            o.writeShort(0x0200);
            o.writeShort(1);
            o.writeInt(0);
            o.writeInt(0);

            int size = data.Length;
            int headerSize = 0x60;

            // // headerSize 0x50 seems to crash with models
            //if (texture.mipmaps.Count == 1)
            //{
            //    headerSize = 0x50;
            //}

            o.writeInt(size + headerSize);
            o.writeInt(0x00);
            o.writeInt(size);
            o.writeShort((short)headerSize);
            o.writeShort(0);
            o.writeShort(1);
            o.writeShort((short)getNutFormat());
            o.writeShort((short)width);
            o.writeShort((short)height);
            o.writeInt(0);
            o.writeInt(0);
            o.writeInt(0x60);
            o.writeInt(0);
            o.writeInt(0);
            o.writeInt(0);

            o.writeInt(data.Length);
            o.writeInt(0);
            o.writeInt(0);
            o.writeInt(0);

            o.writeInt(0x65587400); // "eXt\0"
            o.writeInt(0x20);
            o.writeInt(0x10);
            o.writeInt(0x00);
            o.writeInt(0x47494458); // "GIDX"
            o.writeInt(0x10);
            o.writeInt(id);
            o.writeInt(0);

            o.write(data);

            return o.getBytes();
        }


        public void Destroy()
        {
            GL.DeleteTexture(glId);
        }

        public void BufferTexture()
        {
            if (glId != 0)
                GL.DeleteTexture(glId);

            glId = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, glId);

            if (type == PixelInternalFormat.Rgba)
                GL.TexImage2D(TextureTarget.Texture2D, 0, type, width, height, 0, utype, PixelType.UnsignedByte, data);
            else
                GL.CompressedTexImage2D(TextureTarget.Texture2D, 0, type, width, height, 0, data.Length, data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }
    }
}