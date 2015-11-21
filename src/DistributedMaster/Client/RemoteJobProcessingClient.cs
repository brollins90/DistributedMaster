namespace DistributedMaster.Client
{
    using Models;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using ViewModels;

    public class RemoteJobProcessingClient : IJobProcessingClient
    {
        private string _endpointHost;
        private string _endpointPath;
        private string _proxyHost;
        private int _proxyPort;

        public RemoteJobProcessingClient(string endpointHost, string endpointPath)
        {
            _endpointHost = endpointHost;
            _endpointPath = endpointPath;
        }

        public RemoteJobProcessingClient(string endpointHost, string endpointPath, string proxyHost, int proxyPort)
            : this(endpointHost, endpointPath)
        {
            _proxyHost = proxyHost;
            _proxyPort = proxyPort;
        }

        public async Task<ProcessingJobWorkViewModel> GetWork(Guid clientId)
        {
            var jobs = await GetT<ProcessingJobViewModel[]>(_endpointPath, clientId);
            if (jobs != null)
            {
                foreach (var job in jobs)
                {
                    if (!job.Status.Equals("Done"))
                    {
                        var works = await GetT<ProcessingJobWorkViewModel[]>(job.WorkUrl, clientId);

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

        public Task<int> SendResult(ProcessingJobWorkViewModel work, ProcessingJobWorkResultViewModel result, Guid clientId)
        {
            return PutObj(work.ResultUrl, result, clientId);
        }

        private HttpClient GetHttpClient(Guid clientId)
        {
            HttpClient client;
            if (string.IsNullOrWhiteSpace(_proxyHost))
            {
                client = new HttpClient(new HttpClientHandler() { Proxy = new WebProxy(_proxyHost, _proxyPort) });
            }
            else
            {
                client = new HttpClient();
            }
            client.BaseAddress = new Uri(_endpointHost);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-DistributedClientId", clientId.ToString());

            return client;
        }

        private async Task<T> GetT<T>(string path, Guid clientId)
        {
            using (var client = GetHttpClient(clientId))
            {
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

        private async Task<T> PostT<T>(string path, object content, Guid clientId)
        {
            using (var client = GetHttpClient(clientId))
            {
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

        private async Task<int> PutObj(string path, object content, Guid clientId)
        {
            using (var client = GetHttpClient(clientId))
            {
                try
                {
                    var asJson = JsonConvert.SerializeObject(content);

                    var response = await client.PutAsJsonAsync(path, content);
                    return (int)response.StatusCode;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Get failed with exception: " + e.Message);
                }
            }
            return 500;
        }

        public async Task<ProcessingJobViewModel> SubmitJob(ProcessingJobSubmitViewModel job, Guid clientId)
        {
            var words4 = Words.words.Where(x => x.word.Length == 4).Select(x => x.word).OrderBy(w => w).ToArray();
            var words5 = Words.words.Where(x => x.word.Length == 5).Select(x => x.word).OrderBy(w => w).ToArray();

            using (var client = GetHttpClient(clientId))
            {
                return await PostT<ProcessingJobViewModel>(_endpointPath, job, clientId);
            }
        }
    }
}