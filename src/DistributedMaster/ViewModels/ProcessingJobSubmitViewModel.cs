namespace DistributedMaster.Models
{
    using ViewModels;
    using System;
    using System.Collections.Generic;

    public class ProcessingJobSubmitViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProcessingJobWorkViewModel> Work { get; set; }
    }
}