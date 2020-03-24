using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{

    // Basic byte message
    // Holds reading and writing methods, you can implement your own
    // Warning: the String read/write methods actually embark an additional int read/write to quantify the size of the data. This limits the string to approximatively 4Gb (not GB!).
    // Other Warning: BitConverter methods only allows int as offsets, therefore if you'd be giving more than 4Gb (not GB!) of data, everything would gracefully explode.

    public partial class ByteMessage
    {

        byte[] bytes;
        int cursor;

        public ByteMessage(byte [] bytes)
        {
            this.bytes = bytes;
            cursor = 0;
        }

        public ByteMessage()
        {
            bytes = new byte[0];
            cursor = 0;
        }

        public byte[] GetMessage()
        {
            byte [] lenght = new byte[4];
            lenght = BitConverter.GetBytes(bytes.Length);

            return lenght.Concat(bytes).ToArray();
        }

        // Reads a first int giving the global message's lenght, then reads the message.
        public ByteMessage(System.Net.Security.SslStream stream)
        {
            byte[] b_dataLenght = new byte[4];
            stream.Read(b_dataLenght, 0, 4);
            int dataLenght = BitConverter.ToInt32(b_dataLenght, 0);
            bytes = new byte[dataLenght];

            stream.Read(bytes, 0, dataLenght);
        }

        public ByteMessage(System.Net.Sockets.NetworkStream stream)
        {
            byte[] b_dataLenght = new byte[4];
            stream.Read(b_dataLenght, 0, 4);
            int dataLenght = BitConverter.ToInt32(b_dataLenght, 0);
            bytes = new byte[dataLenght];

            stream.Read(bytes, 0, dataLenght);
        }

        public ByteMessage(string str)
        {
            Load(str);
        }

        public byte ReadByte()
        {
            cursor++;
            if (cursor > bytes.Length)
            {
                throw new Exception("Cursor Out of Bound.");
            }
            return bytes[cursor-1];
        }

        public int ReadInt()
        {
            cursor += 4;
            if (cursor > bytes.Length)
            {
                throw new Exception("Cursor Out of Bound.");
            }
            return BitConverter.ToInt32(bytes, cursor - 4);
        }

        public float ReadFloat()
        {
            cursor += 4;
            if (cursor > bytes.Length)
            {
                throw new Exception("Cursor Out of Bound.");
            }
            return BitConverter.ToSingle(bytes, cursor - 4);
        }

        public double ReadDouble()
        {
            cursor += 8;
            if (cursor > bytes.Length)
            {
                throw new Exception("Cursor Out of Bound.");
            }
            return BitConverter.ToDouble(bytes, cursor - 8);
        }

        public long ReadLong()
        {
            cursor += 8;
            if (cursor > bytes.Length)
            {
                throw new Exception("Cursor Out of Bound.");
            }
            return BitConverter.ToInt64(bytes, cursor - 8);
        }

        public bool ReadBool()
        {
            cursor += 1;
            if (cursor > bytes.Length)
            {
                throw new Exception("Cursor Out of Bound.");
            }
            return BitConverter.ToBoolean(bytes, cursor - 1);
        }

        public string ReadString()
        {
            int size = ReadInt();
            cursor += size;
            if (cursor > bytes.Length)
            {
                throw new Exception("Cursor Out of Bound.");
            }

            return Encoding.ASCII.GetString(bytes, cursor-size, size);
        }

        public string ReadTag()
        {
            cursor += 3;
            if (cursor > bytes.Length)
            {
                throw new Exception("Cursor Out of Bound.");
            }
            return Encoding.ASCII.GetString(bytes, cursor - 3, 3);
        }

        public void WriteTag(string tag)
        {
            if (tag.Length > 3) tag = tag.Substring(0, 3);
            bytes = bytes.Concat(Encoding.ASCII.GetBytes(tag)).ToArray();
        }
        public void WriteString(string message)
        {
            WriteInt(message.Length);
            bytes = bytes.Concat(Encoding.ASCII.GetBytes(message)).ToArray();
        }
        public void WriteInt(int val)
        {
            bytes = bytes.Concat(BitConverter.GetBytes(val)).ToArray();
        }
        public void WriteLong(long val)
        {
            bytes = bytes.Concat(BitConverter.GetBytes(val)).ToArray();
        }
        public void WriteFloat(float val)
        {
            bytes = bytes.Concat(BitConverter.GetBytes(val)).ToArray();
        }
        public void WriteDouble(double val)
        {
            bytes = bytes.Concat(BitConverter.GetBytes(val)).ToArray();
        }
        public void WriteBool(bool val)
        {
            bytes = bytes.Concat(BitConverter.GetBytes(val)).ToArray();
        }

        public void Save(string path)
        {
            File.WriteAllBytes(path, bytes.ToArray());
        }

        public void Load(string path)
        {
            bytes = File.ReadAllBytes(path);
        }


    }
}
