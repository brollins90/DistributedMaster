//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using DistributedMaster.ViewModels;

//namespace DistributedMaster.Client
//{
//    public class RemoteWorkGetter : IGetWork
//    {
//        string _endpointHost;
//        string _endpointPath;
//        string _proxyHost;
//        int _proxyPort;

//        public RemoteWorkGetter(string endpointHost, string endpointPath, string proxyHost, int proxyPort)
//        {
//            _endpointHost = endpointHost;
//            _endpointPath = endpointPath;
//            _proxyHost = proxyHost;
//            _proxyPort = proxyPort;
//        }

//        public async Task<ProcessingJobWorkViewModel> TryGetWork()
//        {
//            var jobs = await GetT<ProcessingJobViewModel[]>(_endpointPath);
//            if (jobs != null)
//            {
//                foreach (var job in jobs)
//                {
//                    if (!job.Status.Equals("Done"))
//                    {
//                        var works = await GetT<ProcessingJobWorkViewModel[]>(job.WorkUrl);

//                        foreach (var work in works)
//                        {
//                            if (work.Status.Equals("NotStarted"))
//                            {
//                                return work;
//                            }
//                        }
//                    }
//                }
//            }
//            return null;
//        }
//    }
//}
