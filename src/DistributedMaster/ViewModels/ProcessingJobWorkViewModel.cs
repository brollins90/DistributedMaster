namespace DistributedMaster.ViewModels
{
    using System;
    using System.Collections.Generic;

    public class ProcessingJobWorkViewModel
    {
        public Guid JobId { get; set; }
        public Guid WorkId { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string Status { get; set; }
        public string Goal { get; set; }
        public IList<string> List1 { get; set; }
        public IList<string> List2 { get; set; }
        public string RequestWorkUrl { get; set; }
        public string ResultUrl { get; set; }
    }
}