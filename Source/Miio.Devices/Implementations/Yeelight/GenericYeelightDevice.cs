using Miio.Devices.Exceptions;
using Miio.Devices.Models;
using Miio.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miio.Devices.Implementations.Yeelight
{
    public class GenericYeelightDevice : GenericDevice, IYeelightDevice
    {
        public GenericYeelightDevice(string deviceIp, string deviceToken) : base(deviceIp, deviceToken)
        {
        }

        public override Task<Response> SwitchState()
        {
            var toggleCmd = new Command()
            {
                Method = YeelightCommandsAndParams.TOGGLE
            };

            return SendRawCommand(toggleCmd);
        }

        public override Task<Response> TurnOn()
        {
            var turnOnCmd = new Command()
            {
                Method = BasicMethodsAndParams.SET_POWER,
                Params = new object[] { BasicMethodsAndParams.ON, YeelightCommandsAndParams.SUDDEN }
            };

            return SendRawCommand(turnOnCmd);
        }

        public override Task<Response> TurnOff()
        {
            var turnOnCmd = new Command()
            {
                Method = BasicMethodsAndParams.SET_POWER,
                Params = new object[] { BasicMethodsAndParams.OFF, YeelightCommandsAndParams.SUDDEN }
            };

            return SendRawCommand(turnOnCmd);
        }

        public Task<Response> TurnOnSmoothly(uint duration)
        {
            return SwitchMode(Mode.Default, duration);
        }

        public Task<Response> TurnOffSmoothly(uint duration)
        {
            var turnOnCmd = new Command()
            {
                Method = BasicMethodsAndParams.SET_POWER,
                Params = new object[] { BasicMethodsAndParams.OFF, YeelightCommandsAndParams.SMOOTH, duration }
            };

            return SendRawCommand(turnOnCmd);
        }

        public Task<Response> SwitchMode(Mode mode)
        {
            var cmd = new Command()
            {
                Method = BasicMethodsAndParams.SET_POWER,
                Params = new object[] { BasicMethodsAndParams.ON, YeelightCommandsAndParams.SUDDEN, 0, (int)mode }
            };

            return SendRawCommand(cmd);
        }

        public Task<Response> SwitchMode(Mode mode, uint duration)
        {
            var cmd = new Command()
            {
                Method = BasicMethodsAndParams.SET_POWER,
                Params = new object[] { BasicMethodsAndParams.ON, YeelightCommandsAndParams.SMOOTH, duration, (int)mode }
            };

            return SendRawCommand(cmd);
        }

        public Task<Response> SetBrightness(ushort value)
        {
            if(value < 1 || value > 100)
            {
                throw new ArgumentException("Brightness must be value from 1-100 range");
            }
            var cmd = new Command()
            {
                Method = YeelightCommandsAndParams.SET_BRIGHTNESS,
                Params = new object[] { value, YeelightCommandsAndParams.SUDDEN, 0 }
            };

            return SendRawCommand(cmd);
        }

        public Task<Response> SetBrightness(ushort value, uint duration)
        {
            if(value < 1 || value > 100)
            {
                throw new ArgumentException("Brightness must be value from 1-100 range");
            }

            var cmd = new Command()
            {
                Method = YeelightCommandsAndParams.SET_BRIGHTNESS,
                Params = new object[] { value, YeelightCommandsAndParams.SMOOTH, duration }
            };

            return SendRawCommand(cmd);
        }

        public Task<Response> SetColorTemperature(uint temperature)
        {
            if(temperature < 1700 || temperature > 6500)
            {
                throw new ArgumentException("Temperature must be value from 1700-6500 range");
            }

            var cmd = new Command()
            {
                Method = YeelightCommandsAndParams.SET_COLOR_TEMPERATURE,
                Params = new object[] { temperature, YeelightCommandsAndParams.SUDDEN}
            };

            return SendRawCommand(cmd);
        }

        public Task<Response> SetColorTemperature(uint temperature, uint duration)
        {
            if(temperature < 1700 || temperature > 6500)
            {
                throw new ArgumentException("Temperature must be value from 1700-6500 range");
            }

            var cmd = new Command()
            {
                Method = YeelightCommandsAndParams.SET_COLOR_TEMPERATURE,
                Params = new object[] { temperature, YeelightCommandsAndParams.SMOOTH, duration }
            };

            return SendRawCommand(cmd);
        }

        public Task<Response> SetRGBColor(ushort red, ushort green, ushort blue)
        {
            if(red > 255 || green > 255 || blue > 255)
            {
                throw new ArgumentException("Each color must be value from 0-255 range");
            }

            var color = red << 16 | green << 8 | blue;          

            var cmd = new Command()
            {
                Method = YeelightCommandsAndParams.SET_RGB_COLOR,
                Params = new object[] { color, YeelightCommandsAndParams.SUDDEN, 0 }
            };

            return SendRawCommand(cmd);
        }

        public Task<Response> SetRGBColor(ushort red, ushort green, ushort blue, uint duration)
        {
            if(red > 255 || green > 255 || blue > 255)
            {
                throw new ArgumentException("Each color must be value from 0-255 range");
            }

            var color = red << 16 | green << 8 | blue;

            var cmd = new Command()
            {
                Method = YeelightCommandsAndParams.SET_RGB_COLOR,
                Params = new object[] { color, YeelightCommandsAndParams.SMOOTH, duration }
            };

            return SendRawCommand(cmd);
        }

        public async Task<int> GetBrightness()
        {
            var props = await this.GetProperties(YeelightCommandsAndParams.BRIGHTNESS);
            if(props.Length == 1 && int.TryParse(props[0].ToString(), out int brightness))
            {
                return brightness;
            }
            else
            {
                throw new DeviceCommunicationException(this.Ip, "Can not read brighness from device");
            }
        }

        public async Task<(ushort red, ushort green, ushort blue)> GetCurrentRGBColor()
        {
            var props = await this.GetProperties(YeelightCommandsAndParams.RGB);
            if(props.Length == 1 && int.TryParse(props[0].ToString(), out int rgb))
            {
                var red = (ushort)(rgb >> 16);
                var green = (ushort)((rgb >> 8) & 255);
                var blue = (ushort)(rgb & 255);
                
                return (red, green, blue);
            }
            else
            {
                throw new DeviceCommunicationException(this.Ip, "Can not read brighness from device");
            }
        }
    }
}
