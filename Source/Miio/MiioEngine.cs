using System;
using System.Net.Sockets;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using Polly;

namespace Miio
{
    public class MiioEngine
    {
        private const int _endpointPort = 54321;
        private readonly UdpClient _miioUdpClient;
        private uint stamp;
        private readonly IPAddress _ip;
        private readonly string _deviceToken;
        private uint _deviceId;

        public MiioEngine(string endpointIpAddress, string deviceToken)
        {
            _ip = IPAddress.Parse(endpointIpAddress);
            _miioUdpClient = new UdpClient();
            _deviceToken = deviceToken;
        }

        public Packet ReceiveMessage(int timeout = 3)
        {
            return Policy
                .Handle<Exception>()
                .OrResult<Packet>(r => r == null)
                .WaitAndRetry(
                    retryCount: timeout * 2,
                    sleepDurationProvider: _ => TimeSpan.FromMilliseconds(500)
                ).Execute(ReceiveMessage);
        }

        public string DecodeMessage(Packet packet)
        {
            return this.Decrypt(packet.PayloadAsByteArray);
        }

        public async Task<int> Handshake()
        {
            Packet packet = new Packet()
            {
                IsHandshake = true,
                Token = "ffffffffffffffffffffffffffffffff"
            };

            return await SendMessage(packet.WholePacket);
        }

        public async Task<int> SendPayload(string payload)
        {
            var encryptedPayload = this.Encrypt(payload);
            Packet packet = new Packet()
            {
                IsHandshake = false,
                Stamp = stamp,
                DeviceId = _deviceId,
                Token = _deviceToken,
                PayloadAsByteArray = encryptedPayload
            };

            var md5 = CalcuateMD5(packet.WholePacket);
            packet.TokenAsByteArray = md5;

            return await SendMessage(packet.WholePacket);
        }
        private byte[] Encrypt(string payload)
        {
            var tokenAsByteArray = ConvertToBytes(_deviceToken);
            var key = CalcuateMD5(tokenAsByteArray);
            var ivInside = key.Concat(tokenAsByteArray).ToArray();
            var iv = CalcuateMD5(ivInside);

            byte[] encrypted;

            using(Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;
                aes.IV = iv;


                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using(MemoryStream msEncrypt = new MemoryStream())
                {
                    using(CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using(StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(payload);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        private string Decrypt(byte[] encrypted)
        {
            var tokenAsByteArray = ConvertToBytes(_deviceToken);
            var key = CalcuateMD5(tokenAsByteArray);
            var ivInside = key.Concat(tokenAsByteArray).ToArray();
            var iv = CalcuateMD5(ivInside);

            string plaintext = null;

            using(Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using(MemoryStream msDecrypt = new MemoryStream(encrypted))
                {
                    using(CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using(StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        private Packet ReceiveMessage()
        {
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                if(_miioUdpClient.Client == null)
                {
                    return null;
                }
                if(_miioUdpClient.Available <= 0)
                {
                    return null;
                }
                byte[] receivedStream = _miioUdpClient.Receive(ref remoteIpep);
                var received = new Packet(receivedStream);
                stamp = received.Stamp + 1;
                _deviceId = received.DeviceId;
                return received;

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private byte[] CalcuateMD5(byte[] toCalculate)
        {
            using(MD5 md5 = MD5.Create())
            {
                return md5.ComputeHash(toCalculate);
            }
        }
        private Task<int> SendMessage(byte[] message)
        {
            var endpoint = new IPEndPoint(_ip, _endpointPort);
            return _miioUdpClient.SendAsync(message, message.Length, endpoint);
        }

        private byte[] ConvertToBytes(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
        }

        private string BytesToString(byte[] bytes)
        {
            return string.Join(null, bytes.Select(x => x.ToString("X2")));
        }
    }
}
