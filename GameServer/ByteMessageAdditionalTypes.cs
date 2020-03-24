using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace GameServer
{
    partial class ByteMessage
    {

        public void WriteDemoCubeData(int id,float r, float g, float b, float a, float rotspeed)
        {
            WriteInt(id);
            WriteFloat(r);
            WriteFloat(g);
            WriteFloat(b);
            WriteFloat(a);
            WriteFloat(rotspeed);
        }

        public void ReadDemoCubeData(out int id, out float r, out float g, out float b,out float a, out float rotspeed)
        {
            id = ReadInt();
            r = ReadFloat();
            g = ReadFloat();
            b = ReadFloat();
            a = ReadFloat();
            rotspeed = ReadFloat();
            string jsonString;
            jsonString = JsonSerializer.Serialize(rotspeed);
        }

        public void WritePosRot(float x, float y, float z, float rx, float ry, float rz)
        {
            WriteFloat(x);
            WriteFloat(y);
            WriteFloat(z);
            WriteFloat(rx);
            WriteFloat(ry);
            WriteFloat(rz);
        }

        public void ReadPosRot(out float x, out float y, out float z, out float rx, out float ry, out float rz)
        {
            x = ReadFloat();
            y = ReadFloat();
            z = ReadFloat();
            rx = ReadFloat();
            ry = ReadFloat();
            rz = ReadFloat();
        }

    }
}
