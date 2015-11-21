namespace DistributedMaster.Models
{
    using System;

    public class ProcessingJob
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Result { get; set; }
    }
}