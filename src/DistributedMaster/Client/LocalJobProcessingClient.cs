namespace DistributedMaster.Client
{
    using System;
    using System.Threading.Tasks;
    using ViewModels;

    public class LocalJobProcessingClient : IJobProcessingClient
    {
        public Task<ProcessingJobWorkViewModel> GetWork(Guid clientId)
        {
            throw new NotImplementedException();
        }

        public Task<int> SendResult(ProcessingJobWorkViewModel work, ProcessingJobWorkResultViewModel result, Guid clientId)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessingJobViewModel> SubmitJob(ProcessingJobSubmitViewModel job, Guid clientId)
        {
            throw new NotImplementedException();
        }
    }
}