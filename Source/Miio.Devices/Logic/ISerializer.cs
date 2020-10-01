using Newtonsoft.Json;

namespace Miio.Devices.Logic
{
    public interface ISerializer
    {
        string Serialize(object toSerialize);
    }
}
