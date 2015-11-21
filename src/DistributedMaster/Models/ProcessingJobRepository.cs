namespace DistributedMaster.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ProcessingJobRepository
    {
        List<ProcessingJob> _jobs = new List<ProcessingJob>();

        public IEnumerable<ProcessingJob> GetAll()
        {
            return _jobs.Select(x => x).ToList();
        }

        public ProcessingJob GetById(Guid id)
        {
            return _jobs.FirstOrDefault(x => x.Id == id);
        }

        public void AddJob(ProcessingJob newJob)
        {
            _jobs.Add(newJob);
        }

        public void UpdateJob(ProcessingJob job)
        {
            var original = _jobs.First(x => x.Id == job.Id);
            _jobs.Remove(original);
            _jobs.Add(job);
        }

        public bool SaveAll()
        {
            return true;
        }
    }
}