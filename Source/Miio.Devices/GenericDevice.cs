using Miio.Devices.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Miio.Devices.Logic;
using Miio.Devices.Exceptions;
using Polly;
using System;

namespace Miio.Devices
{
    public class GenericDevice : ISmartDevice
    {
        protected MiioEngine _miioEngine;
        protected readonly ISerializer _serializer;
        protected readonly IIdProvider _idProvider;

        public virtual bool IsHandshakeMade { get; protected set; }
        public string Ip => _miioEngine?.Ip ?? throw new DeviceNotInitiatedException();
        public string DeviceToken => _miioEngine?.DeviceToken ?? throw new DeviceNotInitiatedException();

        public GenericDevice(string deviceIp, string deviceToken)
        {
            _miioEngine = new MiioEngine(deviceIp, deviceToken);
            _serializer = new Logic.JsonSerializer();
            _idProvider = new IdProvider();
        }

        public GenericDevice(ISerializer serializer, IIdProvider idProvider)
        {
            _serializer = serializer;
            _idProvider = idProvider;
        }

        public void InitiateDevice(string deviceIp, string deviceToken)
        {
            if(_miioEngine == null)
            {
                _miioEngine = new MiioEngine(deviceIp, deviceToken);
            }
            else
            {
                throw new DeviceAlreadyInitiatedException();
            }
        }

        public virtual async Task<bool> MakeHandshake()
        {
            await _miioEngine.Handshake();
            var respone = _miioEngine.ReceiveMessage();
            var success = respone != null;
            IsHandshakeMade = success;
            return success;
        }

        public virtual Task<bool> RefreshDevice()
        {
            return MakeHandshake();
        }

        public virtual async Task<Response> SendRawCommand(Command command)
        {
            if(EnsureDeviceIsInitited())
            {
                if(command.Id == default)
                {
                    command.Id = _idProvider.Get();
                }
                var rawCommand = this.GetRawPayload(command);
                return await this.SendRawPayload(rawCommand);
            }
            else
            {
                throw new DeviceNotInitiatedException();
            }
        }

        public virtual async Task<Response> SwitchState()
        {
            var getStateCommand = new Command()
            {
                Method = BasicMethodsAndParams.GET_PROPERTIES,
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

        public virtual Task<Response> TurnOff()
        {
            var turnOffCmd = new Command()
            {
                Method = BasicMethodsAndParams.SET_POWER,
                Params = new object[] { BasicMethodsAndParams.OFF }
            };

            return SendRawCommand(turnOffCmd);
        }

        public virtual Task<Response> TurnOn()
        {
            var turnOnCmd = new Command()
            {
                Method = BasicMethodsAndParams.SET_POWER,
                Params = new object[] { BasicMethodsAndParams.ON }
            };

            return SendRawCommand(turnOnCmd);
        }

        public async virtual Task<object[]> GetProperties(params string[] parameters)
        {
            var cmd = new Command()
            {
                Method = BasicMethodsAndParams.GET_PROPERTIES,
                Params = parameters
            };

            var response = await SendRawCommand(cmd);
            return response.Result;
        }

        protected async Task<Response> SendRawPayload(string payload, int responseTimeout = 1, int tryCounter = 3)
        {
            var received = await Policy
                .HandleResult<Packet>(p => p == null)
                .WaitAndRetryAsync(tryCounter, _ => TimeSpan.FromMilliseconds(250))
                .ExecuteAsync(async () =>
                {
                    await _miioEngine.SendPayload(payload);
                    return _miioEngine.ReceiveMessage(responseTimeout);

                });

            if(received == null)
            {
                throw new DeviceCommunicationException(this.Ip);
            }
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

        protected bool EnsureDeviceIsInitited()
        {
            return _miioEngine != null && IsHandshakeMade;
        }
    }
}
