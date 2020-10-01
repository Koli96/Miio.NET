﻿using Miio.Devices.Models;
using System.Threading.Tasks;

namespace Miio.Devices
{
    public interface ISmartDevice
    {
        Task<bool> MakeHandshake();
        Task<bool> RefreshDevice();
        Task<Response> TurnOn();
        Task<Response> TurnOff();
        Task<Response> SwitchState();
        Task<Response> SendRawCommand(Command command);
    }
}
