namespace DistributedMaster.Client
{
    public class RemoteClientWorkerFactory
    {
        string endpointHost = "http://localhost.fiddler:5000/";
        string endpointPath = "/api/processingjob";
        string proxyHost = "localhost";
        int proxyPort = 8888;

        public ClientWorker Create()
        {
            return new ClientWorker(new RemoteJobProcessingClient(endpointHost, endpointPath, proxyHost, proxyPort));
        }
    }
}