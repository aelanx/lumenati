using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Lumenati
{
    public enum Endian
    {
        Big,
        Little
    }

    public class InputBuffer
    {
        private byte[] data;
        public uint ptr;
        public Endian Endianness = Endian.Big;

        public InputBuffer(string filename, Endian endian = Endian.Big)
        {
            Endianness = endian;
            data = File.ReadAllBytes(filename);
        }

        public byte[] read(int size)
        {
            if (size + ptr > data.Length)
                throw new IndexOutOfRangeException();

            var o = new byte[size];

            Array.Copy(data, ptr, o, 0, size);

            ptr += (uint)size;

            return o;
        }

        public string readSizedString()
        {
            uint length = (uint)readInt();

            byte[] d = new byte[length];
            Array.Copy(data, ptr, d, 0, length);

            ptr += length;

            return Encoding.UTF8.GetString(d);
        }

        public string readString()
        {
            uint start = ptr;

            while (data[ptr] != 0x00)
            {
                ptr++;
            }

            byte[] d = new byte[ptr - start];
            Array.Copy(data, start, d, 0, d.Length);

            return Encoding.UTF8.GetString(d);
        }

        public string readString(int offset)
        {
            string s = "";
            while (data[offset] != 0x00)
            {
                s += (char)data[offset];
                offset++;
            }
            return s;
        }

        public int readInt()
        {
            if (Endianness == Endian.Big)
                return readIntBE();
            else
                return readIntLE();
        }

        public int readIntBE()
        {
            return ((data[ptr++] & 0xFF) << 24) | ((data[ptr++] & 0xFF) << 16) | ((data[ptr++] & 0xFF) << 8) | (data[ptr++] & 0xFF);
        }

        public int readIntLE()
        {
            var num = BitConverter.ToInt32(data, (int)ptr);
            ptr += 4;
            return num;
        }

        public short readShort()
        {
            if (Endianness == Endian.Big)
                return readShortBE();
            else
                return readShortLE();
        }

        public short readShortBE()
        {
            int num = ((data[ptr++] & 0xFF) << 8) | (data[ptr++] & 0xFF);
            return (short)num;
        }

        public short readShortLE()
        {
            var num = BitConverter.ToInt16(data, (int)ptr);
            ptr += 2;
            return num;
        }

        public byte readByte()
        {
            return data[ptr++];
        }

        public float readFloat()
        {
            if (Endianness == Endian.Big)
                return readFloatBE();
            else
                return readFloatLE();
        }

        public float readFloatBE()
        {
            byte[] num = new byte[4] {
                data[ptr + 3],
                data[ptr + 2],
                data[ptr + 1],
                data[ptr]
            };
            ptr += 4;

            return BitConverter.ToSingle(num, 0);
        }

        public float readFloatLE()
        {
            var num = BitConverter.ToSingle(data, (int)ptr);
            ptr += 4;

            return num;
        }

        public byte[] slice(int offset, int size)
        {
            byte[] by = new byte[size];

            Array.Copy(data, offset, by, 0, size);

            return by;
        }

        public void skip(uint size)
        {
            if (size + ptr > data.Length)
                throw new IndexOutOfRangeException();

            ptr += size;
        }

        public void skipToNextWord()
        {
            skip((4 - (ptr % 4)) % 4);
        }
    }

    public class OutputBuffer
    {
        List<byte> data = new List<byte>();
        public Endian Endianness = Endian.Big;

        public void write(byte[] d)
        {
            data.AddRange(d);
        }

        public void write(OutputBuffer d)
        {
            data.AddRange(d.data);
        }

        public void writeString(string str)
        {
            char[] c = str.ToCharArray();
            for (int i = 0; i < c.Length; i++)
                data.Add((byte)c[i]);
        }

        public void writeStringUtf8(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            foreach (var b in bytes)
                data.Add(b);
        }

        public void writeInt(int d)
        {
            if (Endianness == Endian.Big)
                writeIntBE(d);
            else
                writeIntLE(d);
        }

        public void writeIntBE(int d)
        {
            byte[] b = BitConverter.GetBytes(d);
            Array.Reverse(b);
            write(b);
        }

        public void writeIntLE(int d)
        {
            write(BitConverter.GetBytes(d));
        }

        public void writeShort(short d)
        {
            if (Endianness == Endian.Big)
                writeShortBE(d);
            else
                writeShortLE(d);
        }

        public void writeShortBE(short d)
        {
            byte[] b = BitConverter.GetBytes(d);
            Array.Reverse(b);
            write(b);
        }

        public void writeShortLE(short d)
        {
            write(BitConverter.GetBytes(d));
        }

        public void writeFloat(float f)
        {
            if (Endianness == Endian.Big)
                writeFloatBE(f);
            else
                writeFloatLE(f);
        }

        public void writeFloatBE(float f)
        {
            byte[] b = BitConverter.GetBytes(f);
            Array.Reverse(b);
            write(b);
        }

        public void writeFloatLE(float f)
        {
            write(BitConverter.GetBytes(f));
        }

        public void writeByte(byte d)
        {
            data.Add(d);
        }

        public byte[] getBytes()
        {
            return data.ToArray();
        }

        public int Size
        {
            get
            {
                return data.Count;
            }
        }
    }

}