using Miio.Devices.Logic;
using System.Collections.Generic;

namespace Miio.Devices.Models
{
    public class Command
    {
        public Command()
        {

        }

        public Command(int id, IDeviceCommandContainer command, IEnumerable<object> @params)
        {
            Id = id;
            Method = command.ActualCommandName;
            Params = @params;
        }

        public Command(IDeviceCommandContainer command, IEnumerable<object> @params)
        {
            Method = command.ActualCommandName;
            Params = @params;
        }

        public void SetLiteralMethod(string commandName)
        {
            Method = commandName;
        }

        public int Id { get; set; }
        public string Method { get; private set; }
        public IEnumerable<object> Params { get; set; }
    }
}
