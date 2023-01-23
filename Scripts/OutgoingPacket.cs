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
}
