using Miio.Devices.Models;
using Miio.Devices.Models.Enums;

namespace Miio.Devices.Commands
{
    public static class CommandFactory
    {
        public static Command SwitchStateCommand(bool turnOn)
        {
            string state = turnOn ? BasicParametersNames.ON : BasicParametersNames.OFF;
            return new Command(BasicCommands.SET_POWER, new[] { state });
        }

        public static Command GetPropertiesCommand(params string[] parameters)
        {
            return new Command(BasicCommands.GET_PROPERTIES, parameters);
        }
    }
}
