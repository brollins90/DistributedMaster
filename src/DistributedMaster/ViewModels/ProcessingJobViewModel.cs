namespace DistributedMaster.Models
{
    using System;

    public class ProcessingJobViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Ref { get; set; }
        public string WorkUrl { get; set; }
        public string Result { get; set; }
    }
}