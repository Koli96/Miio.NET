using Miio.Devices.Logic;

namespace Miio.Devices.Models.Enums
{
    public class BasicCommands : IDeviceCommandContainer
    {
        private static BasicCommands _setPower;
        private static BasicCommands _getProps;

        protected BasicCommands(string actualCommandName)
        {
            ActualCommandName = actualCommandName;
        }

        public string ActualCommandName { get; }

        public static BasicCommands SET_POWER
        {
            get
            {
                return _setPower ?? (_setPower = new BasicCommands("set_power"));
            }
        }
        
        public static BasicCommands GET_PROPERTIES
        {
            get
            {
                return _getProps ?? (_getProps = new BasicCommands("get_prop"));
            }
        }

        public override string ToString()
        {
            return ActualCommandName;
        }
    }
}
