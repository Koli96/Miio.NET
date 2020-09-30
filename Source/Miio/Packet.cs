using System;
using System.Linq;

namespace Miio
{
    public class Packet
    {
        public Packet() { }
        public Packet(byte[] bytes)
        {
            _deviceId = BitConverter.ToUInt32(bytes.Skip(8).Take(4).Reverse().ToArray(), 0);
            _stamp = BitConverter.ToUInt32(bytes.Skip(12).Take(4).Reverse().ToArray(), 0);
            _length = BitConverter.ToUInt16(bytes.Skip(2).Take(2).Reverse().ToArray(), 0);
            PayloadAsByteArray = bytes.Skip(32).ToArray();
            TokenAsByteArray = bytes.Skip(16).Take(16).ToArray();
        }

        private uint _deviceId;
        private uint _stamp;
        private ushort _length;

        public ushort MagicNumber => 0x2131;
        public byte[] MagicNumberAsByteArray => BitConverter.GetBytes(MagicNumber).Reverse().ToArray();
        public string MagicNumberAsHex => MagicNumber.ToString("X2");

        public bool IsHandshake { get; set; }

        public uint EmptyPart => IsHandshake ? 0xFFFFFFFF : 0;
        public byte[] EmptyPartAsByteArray => BitConverter.GetBytes(EmptyPart).Reverse().ToArray();
        public string EmptyPartAsHex => EmptyPart.ToString("X2");

        public uint DeviceId
        {
            get => IsHandshake ? 0xFFFFFFFF : _deviceId;
            set
            {
                if(IsHandshake)
                {
                    throw new InvalidOperationException("You can not set device ID when making handshake");
                }
                else
                {
                    _deviceId = value;
                }
            }
        }
        public byte[] DeviceIdAsByteArray => BitConverter.GetBytes(DeviceId).Reverse().ToArray();
        public string DeviceIdAsHex => DeviceId.ToString("X8");

        public ushort Length
        {
            get => (ushort)(MagicNumberAsByteArray.Length
                + EmptyPartAsByteArray.Length
                + DeviceIdAsByteArray.Length
                + StampAsByteArray.Length
                + TokenAsByteArray.Length
                + PayloadAsByteArray.Length
                + 2); //+2 because of Length ;
            set
            {
                if(IsHandshake)
                {
                    throw new InvalidOperationException("You can not set length when making handshake");
                }
                else
                {
                    _length = value;
                }
            }
        }
        public byte[] LengthAsByteArray => BitConverter.GetBytes(Length).Reverse().ToArray();
        public string LengthAsHex => Length.ToString("X4");

        public uint Stamp
        {
            get => IsHandshake ? 0xFFFFFFFF : _stamp;
            set
            {
                if(IsHandshake)
                {
                    throw new InvalidOperationException("You can not set length when making handshake");
                }
                else
                {
                    _stamp = value;
                }
            }
        }
        public byte[] StampAsByteArray => BitConverter.GetBytes(Stamp).Reverse().ToArray();
        public string StampAsHex => EmptyPart.ToString("X2");

        public string Token { get; set; }
        public byte[] TokenAsByteArray
        {
            get => ConvertToBytes(Token);
            set
            {
                Token = string.Join(null, value.Select(x => x.ToString("X2")));
            }
        }

        public string Payload { get; set; }

        public byte[] PayloadAsByteArray
        {
            get => ConvertToBytes(Payload);
            set
            {
                Payload = string.Join(null, value.Select(x => x.ToString("X2")));
            }
        }

        public byte[] WholePacket => MagicNumberAsByteArray
                                        .Concat(LengthAsByteArray)
                                        .Concat(EmptyPartAsByteArray)
                                        .Concat(DeviceIdAsByteArray)
                                        .Concat(StampAsByteArray)
                                        .Concat(TokenAsByteArray)
                                        .Concat(PayloadAsByteArray).ToArray();

        public override string ToString()
        {
            return string.Join(null, WholePacket.Select(x => x.ToString("X2")));
        }

        private byte[] ConvertToBytes(string hex)
        {
            if(string.IsNullOrWhiteSpace(hex))
            {
                return Array.Empty<byte>();
            }
            return Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
        }
    }
}
