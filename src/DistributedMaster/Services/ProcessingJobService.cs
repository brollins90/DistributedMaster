namespace DistributedMaster.Services
{
    using Models;
    using System;
    using System.Collections.Generic;

    public class ProcessingJobService
    {
        private ProcessingJobRepository _repository;
        private ProcessingJobWorkRepository _workRepository;

        public ProcessingJobService(
            ProcessingJobRepository repository,
            ProcessingJobWorkRepository workRepository)
        {
            _repository = repository;
            _workRepository = workRepository;
        }

        public bool AddJob(ProcessingJob newJob)
        {
            _repository.AddJob(newJob);
            return _repository.SaveAll();
        }

        public bool AddJobWork(IEnumerable<ProcessingJobWork> work)
        {
            _workRepository.AddWork(work);
            return _repository.SaveAll();
        }

        public IEnumerable<ProcessingJob> GetAll()
        {
            return _repository.GetAll();
        }

        public ProcessingJob GetById(Guid id)
        {
            return _repository.GetById(id);
        }

        public IEnumerable<ProcessingJobWork> GetWorkForJob(Guid jobId)
        {
            var existingWork = _workRepository.GetForJob(jobId);

            return existingWork;
        }

        public ProcessingJobWork GetWorkById(Guid id)
        {
            return _workRepository.GetById(id);
        }

        public void UpdateJob(ProcessingJob job)
        {
            _repository.UpdateJob(job);
        }

        public void UpdateWork(ProcessingJobWork work)
        {
            _workRepository.UpdateWork(work);
        }
    }
}