using Eduplos.Domain.CoreModule;

namespace Eduplos.Domain.HostelModule
{
    public class HostelAllocations
    {
        public long AllocationId { get; set; }
        public int SessionId { get; set; }
        public string StudentId { get; set; }
        public string Student { get; set; }
        public int? BedSpaceId { get; set; }
        public string Status { get; set; }
        public string Session { get; set; }
        public virtual BedSpace BedSpace{get;set;}
    }
}
