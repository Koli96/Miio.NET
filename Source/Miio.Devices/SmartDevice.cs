using Miio.Devices.Models;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Miio.Devices.Logic;

namespace Miio.Devices
{
    public abstract class SmartDevice : ISmartDevice
    {
        protected readonly MiioEngine _miioEngine;
        protected readonly ISerializer _serializer;

        public SmartDevice(string deviceIp, string deviceToken)
        {
            _miioEngine = new MiioEngine(deviceIp, deviceToken);
            _serializer = new Logic.JsonSerializer();

        }

        public virtual async Task<bool> MakeHandshake()
        {
            await _miioEngine.Handshake();
            var respone = _miioEngine.ReceiveMessage();
            return respone != null;
        }

        public virtual Task<bool> RefreshDevice()
        {
            return MakeHandshake();
        }

        public virtual async Task<Response> SendRawCommand(Command command)
        {
            var rawCommand = this.GetRawPayload(command);
            return await this.SendRawPayload(rawCommand);
        }

        public virtual async Task<Response> SwitchState()
        {
            Random rand = new Random();
            var getStateCommand = new Command()
            {
                Method = BasicMethodsAndParams.GET_PROPERTIES,
                Id = rand.Next(100,1000),
                Params = new object[] { BasicMethodsAndParams.POWER }
            };

            var stateResponse = await SendRawCommand(getStateCommand);
            var state = (string)stateResponse.Result[0] == BasicMethodsAndParams.ON;
            if(state)
            {
                return await TurnOff();
            }
            else
            {
                return await TurnOn();
            }

        }

        public Task<Response> TurnOff()
        {
            Random rand = new Random();
            var turnOffCmd = new Command()
            {
                Method = BasicMethodsAndParams.SET_POWER,
                Id = rand.Next(100, 1000),
                Params = new object[] { BasicMethodsAndParams.OFF }
            };

            return SendRawCommand(turnOffCmd);
        }

        public Task<Response> TurnOn()
        {
            Random rand = new Random();

            var turnOnCmd = new Command()
            {
                Method = BasicMethodsAndParams.SET_POWER,
                Id = rand.Next(100, 1000),
                Params = new object[] { BasicMethodsAndParams.ON }
            };

            return SendRawCommand(turnOnCmd);
        }

        protected async Task<Response> SendRawPayload(string payload, int timeout = 3)
        {
            await _miioEngine.SendPayload(payload);
            var received = _miioEngine.ReceiveMessage(timeout);
            return await GetResponse(received);
        }

        protected string GetRawPayload(Command command)
        {
            return _serializer.Serialize(command);
        }

        protected async Task<Response> GetResponse(Packet packetResponse)
        {
            var decoded = await _miioEngine.DecodeMessage(packetResponse);
            return JsonConvert.DeserializeObject<Response>(decoded);
        }
    }
}
