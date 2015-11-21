namespace DistributedMaster.ViewModels
{
    using ViewModels;
    using System;
    using System.Collections.Generic;

    public class ProcessingJobSubmitViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<ProcessingJobWorkViewModel> Work { get; set; }
    }
}