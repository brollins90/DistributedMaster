namespace DistributedMaster.Client
{
    using DistributedMaster.Models;
    using DistributedMaster.ViewModels;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class ClientWorker
    {
        //IGetWork _workGetter;
        Guid clientId;
        string _endpointHost;
        string _endpointPath;
        string _proxyHost;
        int _proxyPort;

        //public ClientWorker(IGetWork workGetter)
        //{
        //    _workGetter = workGetter;
        //}

        public ClientWorker(string endpointHost, string endpointPath, string proxyHost, int proxyPort)
        {
            clientId = Guid.NewGuid();
            _endpointHost = endpointHost;
            _endpointPath = endpointPath;
            _proxyHost = proxyHost;
            _proxyPort = proxyPort;
        }

        public async Task Start()
        {
            var jobs = await GetT<ProcessingJobViewModel[]>(_endpointPath);
            if (jobs != null && jobs.Length == 0)
            {
                var submittedJob = await SubmitJob();
            }

            var work = await TryGetWork();
            while (work != null)
            {
                await TryStartWork(work);
                work = await TryGetWork();
            }
        }

        public async Task<ProcessingJobWorkViewModel> TryGetWork()
        {
            var jobs = await GetT<ProcessingJobViewModel[]>(_endpointPath);
            if (jobs != null)
            {
                foreach (var job in jobs)
                {
                    if (!job.Status.Equals("Done"))
                    {
                        var works = await GetT<ProcessingJobWorkViewModel[]>(job.WorkUrl);

                        foreach (var work in works)
                        {
                            if (work.Status.Equals("NotStarted"))
                            {
                                return work;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public async Task TryStartWork(ProcessingJobWorkViewModel workItem)
        {
            var putResult = await PutObj(workItem.RequestWorkUrl, new ProcessingJobWorkRequestViewModel
            {
                ClientId = this.clientId,
                JobId = workItem.JobId,
                WorkId = workItem.WorkId
            });
            if (putResult == 200 || putResult == 201 || putResult == 204)
            {
                BeginWork(workItem);
            }
        }

        private async Task BeginWork(ProcessingJobWorkViewModel workItem)
        {
            Guesser g = new Guesser(workItem.List1, workItem.List2, workItem.StartIndex, workItem.EndIndex);

            string foundString;
            bool suceeded = g.Go(workItem.Goal, out foundString);

            if (suceeded) Console.WriteLine();

            var putResult = await PutObj(workItem.ResultUrl, new ProcessingJobWorkResultViewModel
            {
                ClientId = this.clientId,
                JobId = workItem.JobId,
                WorkId = workItem.WorkId,
                Result = foundString,
                Success = suceeded
            });
            if (putResult == 200 || putResult == 201 || putResult == 204)
            {
                //BeginWork(workItem);
            }
        }

        public async Task<T> GetT<T>(string path)
        {
            using (var client = new HttpClient(new HttpClientHandler() { Proxy = new WebProxy(_proxyHost, _proxyPort) }))
            {
                client.BaseAddress = new Uri(_endpointHost);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-DistributedClientId", clientId.ToString());

                try
                {
                    var response = await client.GetAsync(path);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Get Succeeded");
                        string resp = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(resp);
                        return JsonConvert.DeserializeObject<T>(resp);
                    }
                    else
                    {
                        Console.WriteLine("Get failed with: " + response.StatusCode);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Get failed with exception: " + e.Message);
                }
            }
            return default(T);
        }

        public async Task<T> PostT<T>(string path, object content)
        {
            using (var client = new HttpClient(new HttpClientHandler() { Proxy = new WebProxy(_proxyHost, _proxyPort) }))
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.BaseAddress = new Uri(_endpointHost);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-DistributedClientId", clientId.ToString());

                try
                {
                    var asJson = JsonConvert.SerializeObject(content);

                    var response = await client.PostAsJsonAsync(path, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string resp = await response.Content.ReadAsStringAsync();
                        T retval = JsonConvert.DeserializeObject<T>(resp);
                        Console.WriteLine(resp);
                        return retval;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Get failed with exception: " + e.Message);
                }
            }
            return default(T);
        }

        public async Task<int> PutObj(string path, object content)
        {
            using (var client = new HttpClient(new HttpClientHandler() { Proxy = new WebProxy(_proxyHost, _proxyPort) }))
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.BaseAddress = new Uri(_endpointHost);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-DistributedClientId", clientId.ToString());

                try
                {
                    var asJson = JsonConvert.SerializeObject(content);

                    var response = await client.PutAsJsonAsync(path, content);
                    return (int)response.StatusCode;
                    //if (response.IsSuccessStatusCode)
                    //{
                    //    string resp = await response.Content.ReadAsStringAsync();
                    //    T retval = JsonConvert.DeserializeObject<T>(resp);
                    //    Console.WriteLine(resp);
                    //    return retval;
                    //}
                }
                catch (Exception e)
                {
                    Console.WriteLine("Get failed with exception: " + e.Message);
                }
            }
            return 500;
        }

        public async Task<ProcessingJobViewModel> SubmitJob()
        {
            var words4 = Words.words.Where(x => x.word.Length == 4).Select(x => x.word).OrderBy(w => w).ToArray();
            var words5 = Words.words.Where(x => x.word.Length == 5).Select(x => x.word).OrderBy(w => w).ToArray();

            using (var client = new HttpClient(new HttpClientHandler() { Proxy = new WebProxy(_proxyHost, _proxyPort) }))
            {
                client.BaseAddress = new Uri(_endpointHost);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("DistributedClientId", clientId.ToString());

                //Guid jobId = Guid.NewGuid();
                var content = new
                {
                    //Id = jobId,
                    Name = "Job",
                    Work = new[]
                    {
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 0,
                            EndIndex = 5,
                            List1 = words4,
                            List2 = words5
                        },
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 6,
                            EndIndex = 10,
                            List1 = words4,
                            List2 = words5
                        },
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 11,
                            EndIndex = 16,
                            List1 = words4,
                            List2 = words5
                        }
                        /*,
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 30,
                            EndIndex = 39,
                            List1 = words4,
                            List2 = words5
                        },
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 40,
                            EndIndex = 49,
                            List1 = words4,
                            List2 = words5
                        },
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 50,
                            EndIndex = 59,
                            List1 = words4,
                            List2 = words5
                        },
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 60,
                            EndIndex = 69,
                            List1 = words4,
                            List2 = words5
                        },
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 70,
                            EndIndex = 74,
                            List1 = words4,
                            List2 = words5
                        },
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 80,
                            EndIndex = 89,
                            List1 = words4,
                            List2 = words5
                        },
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 90,
                            EndIndex = 99,
                            List1 = words4,
                            List2 = words5
                        },
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 100,
                            EndIndex = 109,
                            List1 = words4,
                            List2 = words5
                        },
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 110,
                            EndIndex = 119,
                            List1 = words4,
                            List2 = words5
                        },
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 120,
                            EndIndex = 129,
                            List1 = words4,
                            List2 = words5
                        },
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 130,
                            EndIndex = 139,
                            List1 = words4,
                            List2 = words5
                        },
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 140,
                            EndIndex = 149,
                            List1 = words4,
                            List2 = words5
                        },
                        new
                        {
                            Goal = "FA-B5-B1-53-78-07-FF-40-FE-49-B1-7D-BB-47-61-36-AC-06-BB-A8-84-C0-70-69-26-56-3A-10-F4-3B-80-AE",
                            workId = Guid.NewGuid(),
                            StartIndex = 150,
                            EndIndex = 160,
                            List1 = words4,
                            List2 = words5
                        }*/
                    }
                };

                return await PostT<ProcessingJobViewModel>(_endpointPath, content);
            }
        }
    }
}