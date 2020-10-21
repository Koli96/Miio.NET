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
            var toggleCmd = GenericYeelightCommandFactory.ToggleCommand();

            return SendRawCommand(toggleCmd);
        }

        public override Task<Response> TurnOn()
        {
            var turnOnCmd = GenericYeelightCommandFactory.SwitchState(true);

            return SendRawCommand(turnOnCmd);
        }

        public override Task<Response> TurnOff()
        {
            var turnOffCmd = GenericYeelightCommandFactory.SwitchState(false);

            return SendRawCommand(turnOffCmd);
        }

        public Task<Response> TurnOnSmoothly(uint duration)
        {
            return SwitchMode(Mode.Default, duration);
        }

        public Task<Response> TurnOffSmoothly(uint duration)
        {
            var turnOffCmd = GenericYeelightCommandFactory.SwitchState(false, duration);

            return SendRawCommand(turnOffCmd);
        }

        public Task<Response> SwitchMode(Mode mode)
        {
            var cmd = GenericYeelightCommandFactory.SwitchMode(mode);

            return SendRawCommand(cmd);
        }

        public Task<Response> SwitchMode(Mode mode, uint duration)
        {
            var cmd = GenericYeelightCommandFactory.SwitchMode(mode, duration);

            return SendRawCommand(cmd);
        }

        public Task<Response> SetBrightness(ushort value)
        {
            return SetBrightness(value, 0);
        }

        public Task<Response> SetBrightness(ushort value, uint duration)
        {
            if(value < 1 || value > 100)
            {
                throw new ArgumentException("Brightness must be value from 1-100 range");
            }

            var cmd = GenericYeelightCommandFactory.SetBrightnessCommand(value, duration);

            return SendRawCommand(cmd);
        }

        public Task<Response> SetColorTemperature(uint temperature)
        {
            return SetColorTemperature(temperature, 0);
        }

        public Task<Response> SetColorTemperature(uint temperature, uint duration)
        {
            if(temperature < 1700 || temperature > 6500)
            {
                throw new ArgumentException("Temperature must be value from 1700-6500 range");
            }

            var cmd = GenericYeelightCommandFactory.SetColorTemperatureCommand(temperature, duration);

            return SendRawCommand(cmd);
        }

        public Task<Response> SetRGBColor(ushort red, ushort green, ushort blue)
        {
            return SetRGBColor(red, green, blue, 0);
        }

        public Task<Response> SetRGBColor(ushort red, ushort green, ushort blue, uint duration)
        {
            if(red > 255 || green > 255 || blue > 255)
            {
                throw new ArgumentException("Each color must be value from 0-255 range");
            }

            var color = red << 16 | green << 8 | blue;

            var cmd = GenericYeelightCommandFactory.SetRgbColorCommand(color, duration);

            return SendRawCommand(cmd);
        }

        public Task<Response> SetHSVColor(ushort hue, ushort saturation)
        {
            return SetHSVColor(hue, saturation, 0);
        }

        public Task<Response> SetHSVColor(ushort hue, ushort saturation, uint duration)
        {
            if(hue > 360 || saturation > 101)
            {
                throw new ArgumentException("Hue must be lower than 360 and saturation lower than 101");
            }

            var cmd = GenericYeelightCommandFactory.SetHsvColorCommand(hue, saturation, duration);

            return SendRawCommand(cmd);
        }

        public async Task<int> GetBrightness()
        {
            var props = await this.GetProperties(YeelightParametersNames.BRIGHTNESS);
            if(props.Length == 1 && int.TryParse(props[0].ToString(), out int brightness))
            {
                return brightness;
            }
            else
            {
                throw new DeviceCommunicationException(this.Ip, "Can not read brighness from device");
            }
        }

        public async Task<int> GetColorTemperature()
        {
            var props = await this.GetProperties(YeelightParametersNames.CT);
            if(props.Length == 1 && int.TryParse(props[0].ToString(), out int ct))
            {
                return ct;
            }
            else
            {
                throw new DeviceCommunicationException(this.Ip, "Can not read color temperature from device");
            }
        }

        public async Task<Mode> GetCurrentMode()
        {
            var props = await this.GetProperties(YeelightParametersNames.MODE);

            if(props.Length == 1 && int.TryParse(props[0].ToString(), out int mode))
            {
                return mode switch
                {
                    1 => Mode.RGB,
                    2 => Mode.ColorTemperature,
                    3 => Mode.HSV,
                    _ => Mode.Default
                };
            }
            else
            {
                throw new DeviceCommunicationException(this.Ip, "Can not read mode from device");
            }
        }

        public async Task<(ushort red, ushort green, ushort blue)> GetCurrentRGBColor()
        {
            var props = await this.GetProperties(YeelightParametersNames.RGB);
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

        public async Task<(ushort hue, ushort saturation)> GetCurrentHSVColor()
        {
            var props = await this.GetProperties(YeelightParametersNames.HUE, YeelightParametersNames.SATURATION);
            if(props.Length == 2 && ushort.TryParse(props[0].ToString(), out ushort hue) && ushort.TryParse(props[1].ToString(), out ushort sat))
            {                
                return (hue, sat);
            }
            else
            {
                throw new DeviceCommunicationException(this.Ip, "Can not read HSV color from device");
            }
        }
    }
}
