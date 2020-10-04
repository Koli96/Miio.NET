using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Miio.Devices.Exceptions
{
    public class DeviceCommunicationException : ApplicationException
    {
        public string Ip { get; protected set; }
        public DeviceCommunicationException(string ip) : this(ip, "Can not communicate with device. Ensure it's connected to same network and provided token is correct.")
        {
            
        }

        public DeviceCommunicationException(string ip, string message) : base($"{message}{Environment.NewLine}Device IP: {ip}")
        {
            Ip = ip;
        }

        public DeviceCommunicationException(string ip, string message, Exception innerException) : base($"{message}{Environment.NewLine}Device IP: {ip}", innerException)
        {
            Ip = ip;
        }        
    }
}
