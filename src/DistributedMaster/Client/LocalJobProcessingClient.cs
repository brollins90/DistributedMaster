namespace DistributedMaster.Client
{
    using System;
    using System.Threading.Tasks;
    using ViewModels;

    public class LocalJobProcessingClient : IJobProcessingClient
    {
        IJobProcessingClient _client;

        public LocalJobProcessingClient(IJobProcessingClient client)
        {
            _client = client;
        }

        public Task<ProcessingJobWorkViewModel> GetWork(Guid clientId)
        {
            return _client.GetWork(clientId);
        }

        public Task<int> SendResult(ProcessingJobWorkViewModel work, ProcessingJobWorkResultViewModel result, Guid clientId)
        {
            return _client.SendResult(work, result, clientId);
        }

        public Task<ProcessingJobViewModel> SubmitJob(ProcessingJobSubmitViewModel job, Guid clientId)
        {
            return _client.SubmitJob(job, clientId);
        }
    }
}