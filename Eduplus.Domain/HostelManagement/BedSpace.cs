namespace Eduplos.Domain.HostelModule
{
    public class BedSpace
    {
        public string SpaceId { get; set; }
        public string SpaceTitle { get; set; }
        public int RoomId { get; set; }
        public string Status { get; set; }
        public virtual Room Room { get; set; }

    }
}
