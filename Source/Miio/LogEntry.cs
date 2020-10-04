namespace Miio
{
    public class LogEntry
    {
        public string DeviceToken { get; set; }
        public string Ip { get; set; }
        public string Command { get; set; }
        public string Message { get; set; }
        public Packet OutPacket { get; set; }
        public Packet InPacket { get; set; }
    }
}
