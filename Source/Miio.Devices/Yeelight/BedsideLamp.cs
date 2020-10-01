namespace Miio.Devices.Yeelight
{
    public class BedsideLamp : SmartDevice, IYeelightDevice
    {
        public BedsideLamp(string deviceIp, string deviceToken) : base(deviceIp, deviceToken)
        {
        }
    }
}
