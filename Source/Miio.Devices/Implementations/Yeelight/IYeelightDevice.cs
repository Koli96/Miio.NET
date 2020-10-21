using Miio.Devices.Models;
using System.Threading.Tasks;

namespace Miio.Devices.Implementations.Yeelight
{
    public interface IYeelightDevice : ISmartDevice
    {
        Task<Response> TurnOnSmoothly(uint duration);
        Task<Response> TurnOffSmoothly(uint duration);
        Task<Response> SwitchMode(Mode mode);
        Task<Response> SwitchMode(Mode mode, uint duration);
        Task<Response> SetBrightness(ushort value);
        Task<Response> SetBrightness(ushort value, uint duration);
        Task<Response> SetColorTemperature(uint temperature);
        Task<Response> SetColorTemperature(uint temperature, uint duration);
        Task<Response> SetHSVColor(ushort hue, ushort saturation);
        Task<Response> SetHSVColor(ushort hue, ushort saturation, uint duration);
        Task<Response> SetRGBColor(ushort red, ushort green, ushort blue, uint duration);
        Task<(ushort red, ushort green, ushort blue)> GetCurrentRGBColor();
        Task<(ushort hue, ushort saturation)> GetCurrentHSVColor();
        Task<int> GetBrightness();
        Task<Mode> GetCurrentMode();
    }
}
