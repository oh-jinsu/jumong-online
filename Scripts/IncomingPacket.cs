using Godot;
using System;
using System.Collections.Generic;

public abstract class IncomingPacket
{
    public static IncomingPacket Deserialize(byte[] buffer)
    {
        ushort serial = BitConverter.ToUInt16(buffer, 0);

        switch (serial)
        {
            case 1:
                return new HelloFromTcp(buffer, 2);
            case 2:
                return new HelloFromUdp(buffer, 2);
            case 3:
                return new Welcome(buffer, 2);
            case 4:
                return new GoodBye(buffer, 2);
            case 5:
                return new Introduce(buffer, 2);
            case 6:
                return new UpdateOrigin(buffer, 2);
            case 7:
                return new UpdateRotation(buffer, 2);
            default:
                throw new Exception("unexpected packet");
        }
    }

    public class HelloFromTcp : IncomingPacket
    {
        public string id;

        public HelloFromTcp(byte[] buffer, int index)
        {
            id = System.Text.Encoding.UTF8.GetString(buffer, index, 32);
        }
    }

    public class HelloFromUdp : IncomingPacket
    {
        public string id;

        public HelloFromUdp(byte[] buffer, int index)
        {
            id = System.Text.Encoding.UTF8.GetString(buffer, index, 32);
        }
    }

    public class Welcome : IncomingPacket
    {
        public string id;

        public Welcome(byte[] buffer, int index)
        {
            id = System.Text.Encoding.UTF8.GetString(buffer, index, 32);
        }
    }

    public class GoodBye : IncomingPacket
    {
        public string id;

        public GoodBye(byte[] buffer, int index)
        {
            id = System.Text.Encoding.UTF8.GetString(buffer, index, 32);
        }
    }

    public class Introduce : IncomingPacket
    {
        public List<string> ids;

        public Introduce(byte[] buffer, int index)
        {
            ids = new List<string>();

            for (int i = index; i < buffer.Length; i += 32)
            {
                var id = System.Text.Encoding.UTF8.GetString(buffer, i, 32);

                ids.Add(id);
            }
        }
    }

    public class UpdateOrigin : IncomingPacket
    {
        public string id;

        public Vector3 origin;

        public UpdateOrigin(byte[] buffer, int index)
        {
            id = System.Text.Encoding.UTF8.GetString(buffer, index, 32);

            var x = BitConverter.ToSingle(buffer, index + 32);

            var y = BitConverter.ToSingle(buffer, index + 36);

            var z = BitConverter.ToSingle(buffer, index + 40);

            origin = new Vector3(x, y, z);
        }
    }

    public class UpdateRotation : IncomingPacket
    {
        public string id;

        public float y;

        public UpdateRotation(byte[] buffer, int index)
        {
            id = System.Text.Encoding.UTF8.GetString(buffer, index, 32);

            y = BitConverter.ToSingle(buffer, index + 32);
        }
    }
}
