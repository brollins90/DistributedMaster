namespace DistributedMaster.Client
{
    using System;
    using System.Threading.Tasks;
    using ViewModels;

    public interface IJobProcessingClient
    {
        Task<ProcessingJobWorkViewModel> GetWork(Guid clientId);
        Task<int> SendResult(ProcessingJobWorkViewModel work, ProcessingJobWorkResultViewModel result, Guid clientId);
        Task<ProcessingJobViewModel> SubmitJob(ProcessingJobSubmitViewModel job, Guid clientId);
    }
}