﻿namespace DistributedMaster.ViewModels
{
    using System;
    using System.Collections.Generic;
	
    public class ProcessingJobWorkConfirmViewModel
    {
        public Guid JobId { get; set; }
        public Guid WorkId { get; set; }
        public Guid ClientId { get; set; }
    }
}