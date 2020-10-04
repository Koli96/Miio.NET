using System;
using System.Net.Sockets;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using Polly;
using Miio.Utilities;
using NLog;

namespace Miio
{
    public class MiioEngine
    {
        private IMiioLogger _logger;
        private const int _endpointPort = 54321;
        private readonly UdpClient _miioUdpClient;
        private uint _stamp;
        private readonly IPAddress _ip;
        private readonly byte[] _deviceTokenAsByteArray;
        private uint _deviceId;

        public string Ip => _ip.ToString();
        public string DeviceToken { get; }

        public MiioEngine(string endpointIpAddress, string deviceToken)
        {
            _ip = IPAddress.Parse(endpointIpAddress);
            _miioUdpClient = new UdpClient();
            DeviceToken = deviceToken;
            _deviceTokenAsByteArray = ByteUtilities.ConvertToBytes(DeviceToken);


            _logger = new MiioLogger();
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

        public Task<string> DecodeMessage(Packet packet)
        {
            return this.Decrypt(packet.PayloadAsByteArray);
        }

        public async Task<int> Handshake()
        {
            Packet packet = new Packet()
            {
                IsHandshake = true
            };
            Log(LogLevel.Info, "Trying to make handshake");

            return await SendMessage(packet);
        }

        public async Task<int> SendPayload(string payload)
        {
            Log(LogLevel.Info, "Encrypting payload", payload);
            var encryptedPayload = await this.Encrypt(payload);

            Packet packet = new Packet()
            {
                IsHandshake = false,
                Stamp = _stamp,
                DeviceId = _deviceId,
                Token = DeviceToken,
                PayloadAsByteArray = encryptedPayload
            };

            var md5 = CalcuateMD5(packet.WholePacket);
            packet.TokenAsByteArray = md5;

            return await SendMessage(packet);
        }

        private async Task<byte[]> Encrypt(string payload)
        {
            byte[] encrypted;

            using(Aes aes = Aes.Create())
            {
                var (key, iv) = this.GetCryptoVectors();

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
                            await swEncrypt.WriteAsync(payload);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        private async Task<string> Decrypt(byte[] encrypted)
        {
            string plaintext = null;

            using(Aes aes = Aes.Create())
            {
                var (key, iv) = this.GetCryptoVectors();

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
                            plaintext = await srDecrypt.ReadToEndAsync();
                        }
                    }
                }
            }

            Log(LogLevel.Info, "Decrypted response", plaintext);
            return plaintext;
        }

        private (byte[] key, byte[] iv) GetCryptoVectors()
        {
            var key = CalcuateMD5(_deviceTokenAsByteArray);
            var ivInside = key.Concat(_deviceTokenAsByteArray).ToArray();
            var iv = CalcuateMD5(ivInside);

            return (key, iv);
        }

        private Packet ReceiveMessage()
        {
            var endpoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                Log(LogLevel.Info, "Trying to get response packet");
                if(_miioUdpClient.Client == null || _miioUdpClient.Available <= 0)
                {
                    Log(LogLevel.Warn, "There is no data received");
                    return null;
                }
                byte[] receivedStream = _miioUdpClient.Receive(ref endpoint);
                var received = new Packet(receivedStream);
                Log(LogLevel.Info, "Got response packet", inPacket: received);
                _stamp = received.Stamp + 1;
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

        private Task<int> SendMessage(Packet packet)
        {
            var endpoint = new IPEndPoint(_ip, _endpointPort);
            Log(LogLevel.Info, "Sending packet", outPacket: packet);
            return _miioUdpClient.SendAsync(packet.WholePacket, packet.WholePacket.Length, endpoint);
        }

        private void Log(LogLevel logLevel, string message, string command = null, Packet outPacket = null, Packet inPacket = null)
        {
            var entry = new LogEntry()
            {
                Ip = Ip,
                DeviceToken = DeviceToken,
                Command = command,
                OutPacket = outPacket,
                InPacket = inPacket,
                Message = message
            };

            _logger.Log(logLevel, entry);
        }
    }
}
