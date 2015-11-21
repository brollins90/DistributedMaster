namespace DistributedMaster.ViewModels
{
    using System;
    using System.Collections.Generic;
	
    public class ProcessingJobWorkResultViewModel
    {
        public Guid JobId { get; set; }
        public Guid WorkId { get; set; }
        public Guid ClientId { get; set; }
        public string Result { get; set; }
        public bool Success { get; set; }
    }
}