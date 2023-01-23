using System;

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
}
