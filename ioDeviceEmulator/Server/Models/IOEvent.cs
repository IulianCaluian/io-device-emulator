using ioDeviceEmulator.Shared;

namespace ioDeviceEmulator.Server.Models
{
    public class IOEvent
    {
        public DateTime EventDate { get; set; }
        public ioElementType IOType { get; set; }
        public int Index { get; set; }  
        public int Status { get; set; }
        public string? Summary { get; set; }
    }


}
