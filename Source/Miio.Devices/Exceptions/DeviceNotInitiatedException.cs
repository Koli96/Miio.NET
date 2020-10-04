using System;

namespace Miio.Devices.Exceptions
{
    public class DeviceNotInitiatedException : InvalidOperationException
    {
        public DeviceNotInitiatedException() : this("You can't send command before device is fully initiated. Ensure it's IP Address and token is set and handshake is made.")
        {
        }

        public DeviceNotInitiatedException(string message) : base(message)
        {
        }

        public DeviceNotInitiatedException(string message, Exception innerException) : base(message, innerException)
        {
        }      
    }
}
