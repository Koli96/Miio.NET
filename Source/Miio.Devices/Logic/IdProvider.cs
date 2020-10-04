namespace Miio.Devices.Logic
{
    public class IdProvider : IIdProvider
    {
        private int _id = 1000;
        public int Get()
        {
            return _id++;
        }
    }
}
