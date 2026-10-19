namespace DeviceAPI.Models
{
    public class DeviceItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string? Secret {get; set; }
    }
}