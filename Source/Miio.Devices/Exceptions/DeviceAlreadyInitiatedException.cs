using System;

namespace Miio.Devices.Exceptions
{
    public class DeviceAlreadyInitiatedException : InvalidOperationException
    {
        public DeviceAlreadyInitiatedException() : this("Device is already initiated. Make a handshake if you haven't yet and start sending commands.")
        {
        }

        public DeviceAlreadyInitiatedException(string message) : base(message)
        {
        }

        public DeviceAlreadyInitiatedException(string message, Exception innerException) : base(message, innerException)
        {
        }      
    }
}
