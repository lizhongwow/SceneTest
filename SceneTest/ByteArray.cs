using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace SceneTest
{
    public class ByteArray : DynamicArray<byte>
    {
        // Fields
        protected byte[] m_convertBuf;
        protected Define.Endian m_endian;
        protected int m_pos;

        // Methods
        public ByteArray()
        {
            this.m_pos = 0;
            this.m_endian = Define.Endian.LITTLE_ENDIAN;
            this.m_convertBuf = new byte[8];
        }

        public ByteArray(int cap)
            : base(cap)
        {
            this.m_pos = 0;
            this.m_endian = Define.Endian.LITTLE_ENDIAN;
            this.m_convertBuf = new byte[8];
        }

        public ByteArray(byte[] d)
        {
            this.m_pos = 0;
            this.m_endian = Define.Endian.LITTLE_ENDIAN;
            this.m_convertBuf = new byte[8];
            base.m_data = d.Clone() as byte[];
            base.m_size = d.Length;
            base.m_capcity = d.Length;
        }

        public ByteArray(byte[] d, int len)
        {
            this.m_pos = 0;
            this.m_endian = Define.Endian.LITTLE_ENDIAN;
            this.m_convertBuf = new byte[8];
            base.m_data = d.Clone() as byte[];
            base.m_size = len;
            base.m_capcity = len;
        }

        public bool compress()
        {
            byte[] buffer = new byte[Math.Max(0x20, base.m_data.Length)];
            DeflaterOutputStream stream = new DeflaterOutputStream(new MemoryStream(buffer));
            stream.Write(base.m_data, 0, base.m_size);
            stream.Finish();
            int num = Convert.ToInt32(stream.Position);
            stream.Close();
            base.clear();
            base.m_data = buffer;
            base.m_size = num;
            base.m_capcity = buffer.Length;
            return true;
        }

        public string dump()
        {
            string str = "";
            str = str + "[";
            for (int i = 0; i < base.m_size; i++)
            {
                if (i != 0)
                {
                    str = str + ",";
                }
                str = str + base.m_data[i];
            }
            return (str + "]");
        }

        public unsafe sbyte readByte()
        {
            if ((base.m_data == null) || (this.m_pos >= base.m_size))
            {
                return 0;
            }
            fixed (byte* numRef = &(base.m_data[this.m_pos++]))
            {
                return *(((sbyte*)numRef));
            }
        }

        public void readBytes(ByteArray o, int offset, int len)
        {
            if ((offset == -1) && (len == -1))
            {
                offset = this.m_pos;
                len = base.m_size - this.m_pos;
            }
            if (base.m_size >= (offset + len))
            {
                int num = Math.Min(len, base.m_size - this.m_pos);
                if (num > 0)
                {
                    int num2 = 0;
                    while (num2 < num)
                    {
                        o.writeUnsignedByte(base.m_data[this.m_pos++]);
                        num2++;
                        if (num2 >= num)
                        {
                            o.m_pos = 0;
                        }
                    }
                }
            }
        }

        public unsafe double readDouble()
        {
            if ((base.m_data == null) || (this.m_pos >= (base.m_size - 7)))
            {
                return 0.0;
            }
            double num = 0.0;
            if (BitConverter.IsLittleEndian == (this.m_endian == Define.Endian.LITTLE_ENDIAN))
            {
                fixed (byte* numRef = &(base.m_data[this.m_pos]))
                {
                    num = *((double*)numRef);
                    //numRef = null;
                }
            }
            else
            {
                this.m_convertBuf[7] = base.m_data[this.m_pos];
                this.m_convertBuf[6] = base.m_data[this.m_pos + 1];
                this.m_convertBuf[5] = base.m_data[this.m_pos + 2];
                this.m_convertBuf[4] = base.m_data[this.m_pos + 3];
                this.m_convertBuf[3] = base.m_data[this.m_pos + 4];
                this.m_convertBuf[2] = base.m_data[this.m_pos + 5];
                this.m_convertBuf[1] = base.m_data[this.m_pos + 6];
                this.m_convertBuf[0] = base.m_data[this.m_pos + 7];
                fixed (byte* numRef = this.m_convertBuf)
                {
                    num = *((double*)numRef);
                }
            }
            this.m_pos += 8;
            return num;
        }

        public unsafe float readFloat()
        {
            if ((base.m_data == null) || (this.m_pos >= (base.m_size - 3)))
            {
                return 0f;
            }
            float num = 0f;
            if (BitConverter.IsLittleEndian == (this.m_endian == Define.Endian.LITTLE_ENDIAN))
            {
                fixed (byte* numRef = &(base.m_data[this.m_pos]))
                {
                    num = *((float*)numRef);
                    //numRef = null;
                }
            }
            else
            {
                this.m_convertBuf[3] = base.m_data[this.m_pos];
                this.m_convertBuf[2] = base.m_data[this.m_pos + 1];
                this.m_convertBuf[1] = base.m_data[this.m_pos + 2];
                this.m_convertBuf[0] = base.m_data[this.m_pos + 3];
                fixed (byte* numRef = this.m_convertBuf)
                {
                    num = *((float*)numRef);
                }
            }
            this.m_pos += 4;
            return num;
        }

        public unsafe int readInt()
        {
            if ((base.m_data == null) || (this.m_pos >= (base.m_size - 3)))
            {
                return 0;
            }
            int num = 0;
            if (BitConverter.IsLittleEndian == (this.m_endian == Define.Endian.LITTLE_ENDIAN))
            {
                fixed (byte* numRef = &(base.m_data[this.m_pos]))
                {
                    num = *((int*)numRef);
                    //numRef = null;
                }
            }
            else
            {
                this.m_convertBuf[3] = base.m_data[this.m_pos];
                this.m_convertBuf[2] = base.m_data[this.m_pos + 1];
                this.m_convertBuf[1] = base.m_data[this.m_pos + 2];
                this.m_convertBuf[0] = base.m_data[this.m_pos + 3];
                fixed (byte* numRef = this.m_convertBuf)
                {
                    num = *((int*)numRef);
                }
            }
            this.m_pos += 4;
            return num;
        }

        public unsafe short readShort()
        {
            if ((base.m_data == null) || (this.m_pos >= (base.m_size - 1)))
            {
                return 0;
            }
            short num = 0;
            if (BitConverter.IsLittleEndian == (this.m_endian == Define.Endian.LITTLE_ENDIAN))
            {
                fixed (byte* numRef = &(base.m_data[this.m_pos]))
                {
                    num = *((short*)numRef);
                    //numRef = null;
                }
            }
            else
            {
                this.m_convertBuf[1] = base.m_data[this.m_pos];
                this.m_convertBuf[0] = base.m_data[this.m_pos + 1];
                fixed (byte* numRef = this.m_convertBuf)
                {
                    num = *((short*)numRef);
                }
            }
            this.m_pos += 2;
            return num;
        }

        public byte readUnsignedByte()
        {
            if ((base.m_data == null) || (this.m_pos >= base.m_size))
            {
                return 0;
            }
            return base.m_data[this.m_pos++];
        }

        public unsafe uint readUnsignedInt()
        {
            if ((base.m_data == null) || (this.m_pos >= (base.m_size - 3)))
            {
                return 0;
            }
            uint num = 0;
            uint* p = &num;
            if (BitConverter.IsLittleEndian == (this.m_endian == Define.Endian.LITTLE_ENDIAN))
            {

                fixed (byte* numRef = &(base.m_data[this.m_pos]))
                {
                    num = *((uint*)numRef);
                    //numRef = null;
                }
            }
            else
            {
                this.m_convertBuf[3] = base.m_data[this.m_pos];
                this.m_convertBuf[2] = base.m_data[this.m_pos + 1];
                this.m_convertBuf[1] = base.m_data[this.m_pos + 2];
                this.m_convertBuf[0] = base.m_data[this.m_pos + 3];
                fixed (byte* numRef = this.m_convertBuf)
                {
                    num = *((uint*)numRef);
                }
            }
            this.m_pos += 4;
            return num;
        }

        public unsafe ushort readUnsignedShort()
        {
            if ((base.m_data == null) || (this.m_pos >= (base.m_size - 1)))
            {
                return 0;
            }
            ushort num = 0;
            if (BitConverter.IsLittleEndian == (this.m_endian == Define.Endian.LITTLE_ENDIAN))
            {
                fixed (byte* numRef = &(base.m_data[this.m_pos]))
                {
                    num = *((ushort*)numRef);
                    //numRef = null;
                }
            }
            else
            {
                this.m_convertBuf[1] = base.m_data[this.m_pos];
                this.m_convertBuf[0] = base.m_data[this.m_pos + 1];
                fixed (byte* numRef = this.m_convertBuf)
                {
                    num = *((ushort*)numRef);
                }
            }
            this.m_pos += 2;
            return num;
        }

        public string readUTF8Bytes(int len)
        {
            if ((base.m_data == null) || (this.m_pos >= ((base.m_size - len) + 1)))
            {
                return "";
            }
            if ((base.m_data.Length - this.m_pos) < len)
            {
                return "";
            }
            string str = Encoding.UTF8.GetString(base.m_data, this.m_pos, len);
            this.m_pos += len;
            return str;
        }

        public bool uncompress()
        {
            try
            {
                bool flag2;
                InflaterInputStream stream = new InflaterInputStream(new MemoryStream(base.m_data));
                DynamicArray<byte> array = new DynamicArray<byte>();
                int count = 0;
                byte[] buffer = new byte[0x800];
                goto Label_0053;
            Label_0028:
                count = stream.Read(buffer, 0, buffer.Length);
                if (count > 0)
                {
                    array.pushBack(buffer, count);
                }
                else
                {
                    goto Label_0058;
                }
            Label_0053:
                flag2 = true;
                goto Label_0028;
            Label_0058:
                stream.Close();
                base.clear();
                base.m_data = array.data;
                base.m_size = array.length;
                base.m_capcity = array.capcity;
            }
            catch (Exception)
            {
                DebugTrace.print("Failed to uncompress ByteArray");
                return false;
            }
            return true;
        }

        public void writeByte(sbyte v)
        {
            if (this.m_pos > base.m_size)
            {
                this.m_pos = base.m_size;
            }
            if (this.m_pos >= base.m_size)
            {
                base.pushBack((byte)v);
                this.m_pos = base.m_size;
            }
            else
            {
                base.m_data[this.m_pos++] = (byte)v;
            }
        }

        public void writeBytes(byte[] data, int len)
        {
            if (this.m_pos > base.m_size)
            {
                this.m_pos = base.m_size;
            }
            while ((this.m_pos + len) > base.m_capcity)
            {
                base.capcity *= 2;
            }
            Array.Copy(data, 0, base.m_data, this.m_pos, len);
            this.m_pos += len;
            if (this.m_pos > base.m_size)
            {
                base.m_size = this.m_pos;
            }
        }

        public void writeBytes(char[] data, int len)
        {
            if (this.m_pos > base.m_size)
            {
                this.m_pos = base.m_size;
            }
            while ((this.m_pos + len) > base.m_capcity)
            {
                base.capcity *= 2;
            }
            Array.Copy(data, 0, base.m_data, this.m_pos, len);
            this.m_pos += len;
            if (this.m_pos > base.m_size)
            {
                base.m_size = this.m_pos;
            }
        }

        public void writeBytes(ByteArray o, int offset, int len)
        {
            if (this.m_pos > base.m_size)
            {
                this.m_pos = base.m_size;
            }
            while ((this.m_pos + len) > base.m_capcity)
            {
                base.capcity *= 2;
            }
            Array.Copy(o.m_data, offset, base.m_data, this.m_pos, len);
            this.m_pos += len;
            if (this.m_pos > base.m_size)
            {
                base.m_size = this.m_pos;
            }
        }

        public void writeDouble(double v)
        {
            if (this.m_pos > base.m_size)
            {
                this.m_pos = base.m_size;
            }
            byte[] bytes = BitConverter.GetBytes(v);
            while ((this.m_pos + bytes.Length) > base.m_capcity)
            {
                base.capcity *= 2;
            }
            if (BitConverter.IsLittleEndian != (this.m_endian == Define.Endian.LITTLE_ENDIAN))
            {
                Array.Reverse(bytes);
            }
            bytes.CopyTo(base.m_data, this.m_pos);
            this.m_pos += bytes.Length;
            if (this.m_pos > base.m_size)
            {
                base.m_size = this.m_pos;
            }
        }

        public void writeFloat(float v)
        {
            if (this.m_pos > base.m_size)
            {
                this.m_pos = base.m_size;
            }
            byte[] bytes = BitConverter.GetBytes(v);
            while ((this.m_pos + bytes.Length) > base.m_capcity)
            {
                base.capcity *= 2;
            }
            if (BitConverter.IsLittleEndian != (this.m_endian == Define.Endian.LITTLE_ENDIAN))
            {
                Array.Reverse(bytes);
            }
            bytes.CopyTo(base.m_data, this.m_pos);
            this.m_pos += bytes.Length;
            if (this.m_pos > base.m_size)
            {
                base.m_size = this.m_pos;
            }
        }

        public void writeInt(int v)
        {
            if (this.m_pos > base.m_size)
            {
                this.m_pos = base.m_size;
            }
            byte[] bytes = BitConverter.GetBytes(v);
            while ((this.m_pos + bytes.Length) > base.m_capcity)
            {
                base.capcity *= 2;
            }
            if (BitConverter.IsLittleEndian != (this.m_endian == Define.Endian.LITTLE_ENDIAN))
            {
                Array.Reverse(bytes);
            }
            bytes.CopyTo(base.m_data, this.m_pos);
            this.m_pos += bytes.Length;
            if (this.m_pos > base.m_size)
            {
                base.m_size = this.m_pos;
            }
        }

        public void writeShort(short v)
        {
            if (this.m_pos > base.m_size)
            {
                this.m_pos = base.m_size;
            }
            byte[] bytes = BitConverter.GetBytes(v);
            while ((this.m_pos + bytes.Length) > base.m_capcity)
            {
                base.capcity *= 2;
            }
            if (BitConverter.IsLittleEndian != (this.m_endian == Define.Endian.LITTLE_ENDIAN))
            {
                Array.Reverse(bytes);
            }
            bytes.CopyTo(base.m_data, this.m_pos);
            this.m_pos += bytes.Length;
            if (this.m_pos > base.m_size)
            {
                base.m_size = this.m_pos;
            }
        }

        public void writeUnsignedByte(byte v)
        {
            if (this.m_pos > base.m_size)
            {
                this.m_pos = base.m_size;
            }
            if (this.m_pos >= base.m_size)
            {
                base.pushBack(v);
                this.m_pos = base.m_size;
            }
            else
            {
                base.m_data[this.m_pos++] = v;
            }
        }

        public void writeUnsignedInt(uint v)
        {
            if (this.m_pos > base.m_size)
            {
                this.m_pos = base.m_size;
            }
            byte[] bytes = BitConverter.GetBytes(v);
            while ((this.m_pos + bytes.Length) > base.m_capcity)
            {
                base.capcity *= 2;
            }
            if (BitConverter.IsLittleEndian != (this.m_endian == Define.Endian.LITTLE_ENDIAN))
            {
                Array.Reverse(bytes);
            }
            bytes.CopyTo(base.m_data, this.m_pos);
            this.m_pos += bytes.Length;
            if (this.m_pos > base.m_size)
            {
                base.m_size = this.m_pos;
            }
        }

        public void writeUnsignedShort(ushort v)
        {
            if (this.m_pos > base.m_size)
            {
                this.m_pos = base.m_size;
            }
            byte[] bytes = BitConverter.GetBytes(v);
            while ((this.m_pos + bytes.Length) > base.m_capcity)
            {
                base.capcity *= 2;
            }
            if (BitConverter.IsLittleEndian != (this.m_endian == Define.Endian.LITTLE_ENDIAN))
            {
                Array.Reverse(bytes);
            }
            bytes.CopyTo(base.m_data, this.m_pos);
            this.m_pos += bytes.Length;
            if (this.m_pos > base.m_size)
            {
                base.m_size = this.m_pos;
            }
        }

        public void writeUTF8Bytes(string str)
        {
            if (this.m_pos > base.m_size)
            {
                this.m_pos = base.m_size;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            while ((this.m_pos + bytes.Length) > base.m_capcity)
            {
                base.capcity *= 2;
            }
            bytes.CopyTo(base.m_data, this.m_pos);
            this.m_pos += bytes.Length;
            if (this.m_pos > base.m_size)
            {
                base.m_size = this.m_pos;
            }
        }

        // Properties
        public int bytesAvailable
        {
            get
            {
                return Math.Max(0, base.m_size - this.m_pos);
            }
        }

        public override int length
        {
            get
            {
                return base.m_size;
            }
            set
            {
                if (base.capcity < value)
                {
                    base.capcity = value;
                }
                base.m_size = value;
                if (this.m_pos > base.m_size)
                {
                    this.m_pos = base.m_size;
                }
            }
        }

        public int position
        {
            get
            {
                return this.m_pos;
            }
            set
            {
                this.m_pos = Math.Max(0, value);
            }
        }
    }


}
