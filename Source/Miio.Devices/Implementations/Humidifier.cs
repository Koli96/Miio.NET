using System;
using System.Collections.Generic;
using System.Text;

namespace Miio.Devices.Implementations
{
    public class Humidifier : GenericDevice
    {
        public Humidifier(string deviceIp, string deviceToken) : base(deviceIp, deviceToken)
        {
        }
    }
}
