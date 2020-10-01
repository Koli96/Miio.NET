using System;
using System.ComponentModel;
using System.Linq;
using Miio.Utilities;

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
        private string _token;
        private ushort _length;

        public ushort MagicNumber => 0x2131;
        public byte[] MagicNumberAsByteArray => ByteUtilities.ConvertToBytes(MagicNumber);
        public string MagicNumberAsHex => MagicNumber.ToString("X2");

        public bool IsHandshake { get; set; }

        public uint EmptyPart => IsHandshake ? 0xFFFFFFFF : 0;
        public byte[] EmptyPartAsByteArray => ByteUtilities.ConvertToBytes(EmptyPart);
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
        public byte[] DeviceIdAsByteArray => ByteUtilities.ConvertToBytes(DeviceId);
        public string DeviceIdAsHex => DeviceId.ToString("X8");

        public ushort Length
        {
            get => (ushort)(MagicNumberAsByteArray.Length
                + EmptyPartAsByteArray.Length
                + DeviceIdAsByteArray.Length
                + StampAsByteArray.Length
                + TokenAsByteArray.Length
                + PayloadAsByteArray.Length
                + 2); //+2 because of Length itself;
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
        public byte[] LengthAsByteArray => ByteUtilities.ConvertToBytes(Length);
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
        public byte[] StampAsByteArray => ByteUtilities.ConvertToBytes(Stamp);
        public string StampAsHex => EmptyPart.ToString("X2");

        public string Token
        {
            get => IsHandshake ? "ffffffffffffffffffffffffffffffff" : _token;
            set
            {
                if(IsHandshake)
                {
                    throw new InvalidOperationException("You can not set length when making handshake");
                }
                else
                {
                    _token = value;
                }
            }
        }

        public byte[] TokenAsByteArray
        {
            get => ByteUtilities.ConvertToBytes(Token);
            set
            {
                Token = ByteUtilities.ConvertToString(value); ;
            }
        }

        public string Payload { get; set; }

        public byte[] PayloadAsByteArray
        {
            get => ByteUtilities.ConvertToBytes(Payload);
            set
            {
                Payload = ByteUtilities.ConvertToString(value);
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
            return ByteUtilities.ConvertToString(WholePacket);
        }
    }
}
