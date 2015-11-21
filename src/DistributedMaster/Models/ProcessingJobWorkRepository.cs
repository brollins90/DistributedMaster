namespace DistributedMaster.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ProcessingJobWorkRepository
    {
        List<ProcessingJobWork> _jobWorks = new List<ProcessingJobWork>();

        public IEnumerable<ProcessingJobWork> GetForJob(Guid jobId)
        {
            return _jobWorks.Where(x => x.JobId == jobId).ToList();
        }

        public ProcessingJobWork GetById(Guid id)
        {
            return _jobWorks.FirstOrDefault(x => x.WorkId == id);
        }

        public void AddWork(IEnumerable<ProcessingJobWork> work)
        {
            _jobWorks.AddRange(work);
        }

        public void UpdateWork(ProcessingJobWork work)
        {
            var original = _jobWorks.First(x => x.WorkId == work.WorkId);
            _jobWorks.Remove(original);
            _jobWorks.Add(work);
        }
    }
}