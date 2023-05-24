namespace Eduplus.Domain.HostelModule
{
    public class Room
    {
        public string RoomId { get; set; }
        public string Title { get; set; }
        public int Capacity { get; set; }//total beds
        public int HostelId { get; set; }
        public string Status { get; set; }
        public virtual Hostel Hostel { get; set; }
    }
}
