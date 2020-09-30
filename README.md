# Miio.NET

First of all - big thanks to [OpenMiHome](https://github.com/OpenMiHome/) for describing [protocol](https://github.com/OpenMiHome/mihome-binary-protocol/blob/master/doc/PROTOCOL.md).

## Basic usage

To use this library you need to obtain your Xiaomi smart device token (you can use modified application (e.g. https://www.kapiba.ru/2017/11/mi-home.html).
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
Now you can start sendig commands. You can find them using modified app mentioned above.

For example if you want turn on Yeelight device command looks like:
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
