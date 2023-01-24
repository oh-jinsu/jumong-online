using Godot;
using System;

public abstract class OutgoingPacket
{
    public abstract byte[] Serialize();

    public class HelloFromTcp : OutgoingPacket
    {
        public static readonly byte[] serial = new byte[] { 1, 0 };

        public string token;

        public override byte[] Serialize()
        {
            var body = System.Text.Encoding.UTF8.GetBytes(token);

            var buffer = new byte[2 + body.Length];

            Buffer.BlockCopy(serial, 0, buffer, 0, 2);

            Buffer.BlockCopy(body, 0, buffer, 2, body.Length);

            return buffer;
        }
    }

    public class HelloFromUdp : OutgoingPacket
    {
        public static readonly byte[] serial = new byte[] { 2, 0 };

        public string token;

        public override byte[] Serialize()
        {
            var body = System.Text.Encoding.UTF8.GetBytes(token);

            var buffer = new byte[2 + body.Length];

            Buffer.BlockCopy(serial, 0, buffer, 0, 2);

            Buffer.BlockCopy(body, 0, buffer, 2, body.Length);

            return buffer;
        }
    }

    public class UpdateOrigin : OutgoingPacket
    {
        public static readonly byte[] serial = new byte[] { 3, 0 };

        public Vector3 origin;

        public override byte[] Serialize()
        {
            var buffer = new byte[14];

            Buffer.BlockCopy(serial, 0, buffer, 0, 2);

            Buffer.BlockCopy(BitConverter.GetBytes(origin.x), 0, buffer, 2, 4);

            Buffer.BlockCopy(BitConverter.GetBytes(origin.y), 0, buffer, 6, 4);

            Buffer.BlockCopy(BitConverter.GetBytes(origin.z), 0, buffer, 10, 4);

            return buffer;
        }
    }

    public class UpdateRotation : OutgoingPacket
    {
        public static readonly byte[] serial = new byte[] { 4, 0 };

        public float y;

        public override byte[] Serialize()
        {
            var buffer = new byte[6];

            Buffer.BlockCopy(serial, 0, buffer, 0, 2);

            Buffer.BlockCopy(BitConverter.GetBytes(y), 0, buffer, 2, 4);

            return buffer;
        }
    }
}
