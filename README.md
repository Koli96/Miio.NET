# Miio.NET

First of all - big thanks to [OpenMiHome](https://github.com/OpenMiHome/) for describing [protocol](https://github.com/OpenMiHome/mihome-binary-protocol/blob/master/doc/PROTOCOL.md).

## Basic `miio` engine usage

To use this library you need to obtain your Xiaomi smart device token (you can use modified application (e.g. https://www.kapiba.ru/2017/11/mi-home.html)).
When you know your token it's possible to initiate device:
```
var deviceIp = "127.0.0.1";
var deviceToken = "ffffffffffffffffffffffffffffffff"
MiioEngine device = new MiioEngine(deviceIp, deviceToken);
```
Next step is to make a handshshake:
```
await device.Handshake();
```
and flush receive stream:
```
device.ReceiveMessage();
```
Now you can start sending commands. You can find them using modified app mentioned above.

For example if you want turn on Yeelight device, command looks like:
```
{
    "id": 4545,
    "method": "set_power",
    "params": ["on", "smooth", 500]
}
```
This particular command turn on device with "smooth" brighting for 500 ms.
So only what you have to do is send this string using `SendPayload(string)` method:
```
var cmd = @"{""id"":3312,""method"":""set_power"",""params"":[""on"",""smooth"",500]}";
await device.SendPayload(cmd);
var response = device.ReceiveMessage(10);
var decoded = device.DecodeMessage(response);
```
From `decoded` variable you can read response from device:
```
{"id":3312,"result":["ok"]}
```

## Usage of `Miio.Devices`
If you don't want to use raw commands, you can use this library to manage specicif devices. Every device inherits from `ISmartDevice` interface, which contains following methods:
```
Task<bool> MakeHandshake();
Task<bool> RefreshDevice();
Task<Response> TurnOn();
Task<Response> TurnOff();
Task<Response> SwitchState();
Task<Response> SendRawCommand(Command command);
```
### Example of usage:
Let's assume that we want to switch state of our device (if it's turned off then turn it on and vice versa)
At the begining you have to initiate your device (like `MiioEngine`):
```
var deviceIp = "127.0.0.1";
var deviceToken = "ffffffffffffffffffffffffffffffff"
var bedsideLamp = new BedsideLamp(deviceIp, deviceToken);
```
Next step as at engine is to make a handshake:
```
bool success = await bedsideLamp.MakeHandshake();
```
`MakeHandshake()` method returns if handshake was succesfull. Last step is using `SwitchState()` method.
```
await bedsideLamp.SwitchState();
```

### `Command` and `Response` classes
`ISmartDevice` interface allows you to use `SendRawCommand(Command)` method. It's representation of commands sended to your smart device. For example command to turn on device can look like this:
```
new Command()
{
    Method = "set_power",
    Id = 1234,
    Params = new object[] { "on" }
};
```
`Response` class is serialized response from your device.
