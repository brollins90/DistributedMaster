namespace DistributedMaster.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ViewModels;

    public class ClientWorker
    {
        Guid _clientId;
        IJobProcessingClient _jobProcessingClient;

        public ClientWorker(IJobProcessingClient jobProcessingClient)
        {
            _clientId = Guid.NewGuid();
            _jobProcessingClient = jobProcessingClient;
        }

        public async Task Start()
        {
            var work = await _jobProcessingClient.GetWork(_clientId);
            if (work == null)
            {
                await SubmitJob();
            }
            work = await _jobProcessingClient.GetWork(_clientId);

            while (work != null)
            {
                await BeginWork(work);
                work = await _jobProcessingClient.GetWork(_clientId);
            }
        }

        async Task BeginWork(ProcessingJobWorkViewModel workItem)
        {
            Guesser g = new Guesser(workItem.List1, workItem.List2, workItem.StartIndex, workItem.EndIndex);

            string foundString;
            bool suceeded = g.Go(workItem.Goal, out foundString);

            if (suceeded) Console.WriteLine();

            var putResult = await _jobProcessingClient.SendResult(workItem, new ProcessingJobWorkResultViewModel
            {
                ClientId = _clientId,
                JobId = workItem.JobId,
                WorkId = workItem.WorkId,
                Result = foundString,
                Success = suceeded
            },
            _clientId);
            //if (putResult == 200 || putResult == 201 || putResult == 204)
            //{
            //    //BeginWork(workItem);
            //}
        }

        public async Task<ProcessingJobViewModel> SubmitJob()
        {
            var words4 = Words.words.Where(x => x.word.Length == 4).Select(x => x.word).OrderBy(w => w).ToArray();
            var words5 = Words.words.Where(x => x.word.Length == 5).Select(x => x.word).OrderBy(w => w).ToArray();
            return await _jobProcessingClient.SubmitJob(new ProcessingJobSubmitViewModel
            {
                //Id = jobId,
                Name = "Job",
                Work = new List<ProcessingJobWorkViewModel>
                {
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 0,
                        EndIndex = 5,
                        List1 = words4,
                        List2 = words5
                    },
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 6,
                        EndIndex = 10,
                        List1 = words4,
                        List2 = words5
                    },
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 11,
                        EndIndex = 16,
                        List1 = words4,
                        List2 = words5
                    }
                    /*,
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 30,
                        EndIndex = 39,
                        List1 = words4,
                        List2 = words5
                    },
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 40,
                        EndIndex = 49,
                        List1 = words4,
                        List2 = words5
                    },
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 50,
                        EndIndex = 59,
                        List1 = words4,
                        List2 = words5
                    },
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 60,
                        EndIndex = 69,
                        List1 = words4,
                        List2 = words5
                    },
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 70,
                        EndIndex = 74,
                        List1 = words4,
                        List2 = words5
                    },
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 80,
                        EndIndex = 89,
                        List1 = words4,
                        List2 = words5
                    },
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 90,
                        EndIndex = 99,
                        List1 = words4,
                        List2 = words5
                    },
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 100,
                        EndIndex = 109,
                        List1 = words4,
                        List2 = words5
                    },
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 110,
                        EndIndex = 119,
                        List1 = words4,
                        List2 = words5
                    },
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 120,
                        EndIndex = 129,
                        List1 = words4,
                        List2 = words5
                    },
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 130,
                        EndIndex = 139,
                        List1 = words4,
                        List2 = words5
                    },
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 140,
                        EndIndex = 149,
                        List1 = words4,
                        List2 = words5
                    },
                    new ProcessingJobWorkViewModel
                    {
                        Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                        WorkId = Guid.NewGuid(),
                        StartIndex = 150,
                        EndIndex = 160,
                        List1 = words4,
                        List2 = words5
                    }*/
                }
            },
            _clientId);
        }
    }

}