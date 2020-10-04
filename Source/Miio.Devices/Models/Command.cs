namespace Miio.Devices.Models
{
    public class Command
    {
        public int Id { get; set; }
        public string Method { get; set; }
        public object[] Params { get; set; }
    }
}
