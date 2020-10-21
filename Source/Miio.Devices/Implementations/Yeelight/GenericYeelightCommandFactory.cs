using Miio.Devices.Commands;
using Miio.Devices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miio.Devices.Implementations.Yeelight
{
    public static class GenericYeelightCommandFactory
    {
        public static Command ToggleCommand()
        {
            var suddenArgs = GenerateSuddenArguments();
            return new Command(YeelightCommands.TOGGLE, suddenArgs);
        }

        public static Command ToggleCommand(uint duration)
        {
            var smoothArgs = GenerateSmoothArguments(duration);
            return new Command(YeelightCommands.TOGGLE, smoothArgs);
        }

        public static Command SwitchState(bool turnOn, uint duration)
        {
            string state = turnOn ? BasicParametersNames.ON : BasicParametersNames.OFF;
            var args = new List<object>
            {
                state
            };
            args.Concat(duration == 0 ? GenerateSuddenArguments() : GenerateSmoothArguments(duration));
            return new Command(YeelightCommands.SET_POWER, args);
        }

        public static Command SwitchState(bool turnOn)
        {
            return SwitchState(turnOn, 0);
        }

        public static Command SwitchMode(Mode mode, uint duration)
        {
            var cmd = SwitchState(true, duration);
            var newParams = cmd.Params.ToList();
            newParams.Add((int)mode);
            cmd.Params = newParams;
            return cmd;
        }

        public static Command SwitchMode(Mode mode)
        {
            return SwitchMode(mode, 0);
        }

        public static Command SetBrightnessCommand(uint value, uint duration)
        {
            var args = new List<object>
            {
                value
            };
            args.Concat(duration == 0 ? GenerateSuddenArguments() : GenerateSmoothArguments(duration));
            return new Command(YeelightCommands.SET_BRIGHTNESS, args);
        }

        public static Command SetBrightnessCommand(uint value)
        {
            return SetBrightnessCommand(value, 0);
        }

        public static Command SetColorTemperatureCommand(uint value, uint duration)
        {
            var args = new List<object>
            {
                value
            };
            args.Concat(duration == 0 ? GenerateSuddenArguments() : GenerateSmoothArguments(duration));
            return new Command(YeelightCommands.SET_COLOR_TEMPERATURE, args);
        }

        public static Command SetColorTemperatureCommand(uint value)
        {
            return SetColorTemperatureCommand(value, 0);
        }

        public static Command SetRgbColorCommand(int value, uint duration)
        {
            var args = new List<object>
            {
                value
            };
            args.Concat(duration == 0 ? GenerateSuddenArguments() : GenerateSmoothArguments(duration));
            return new Command(YeelightCommands.SET_RGB_COLOR, args);
        }

        public static Command SetRgbColorCommand(int value)
        {
            return SetRgbColorCommand(value, 0);
        }

        public static Command SetHsvColorCommand(uint hue, uint sat, uint duration)
        {
            var args = new List<object>
            {
                hue, sat
            };
            args.Concat(duration == 0 ? GenerateSuddenArguments() : GenerateSmoothArguments(duration));
            return new Command(YeelightCommands.SET_HSV_COLOR, args);
        }

        public static Command SetHsvColorCommand(uint hue, uint sat)
        {
            return SetHsvColorCommand(hue, sat, 0);
        }

        public static Command GetBrightnessCommand()
        {
            return CommandFactory.GetPropertiesCommand(YeelightParametersNames.BRIGHTNESS);
        }

        public static Command GetRgbColorCommand()
        {
            return CommandFactory.GetPropertiesCommand(YeelightParametersNames.RGB);
        }


        private static List<object> GenerateSmoothArguments(uint duration)
        {
            if(duration < 30)
            {
                throw new ArgumentException("Duration must be at least 30 ms");
            }

            return new List<object>
            {
                YeelightParametersNames.SMOOTH,
                duration
            };
        }

        private static List<object> GenerateSuddenArguments()
        {
            return new List<object>
            {
                YeelightParametersNames.SUDDEN,
                0
            };
        }
    }
}
