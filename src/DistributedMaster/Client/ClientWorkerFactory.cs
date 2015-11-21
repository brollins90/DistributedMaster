namespace DistributedMaster.Client
{
    using System;
    using System.Collections.Generic;

    public class ClientWorkerFactory
    {
        string endpointHost = "http://localhost.fiddler:5000/";
        string endpointPath = "/api/processingjob";
        string proxyHost = "localhost";
        int proxyPort = 8888;

        public ClientWorker Create()
        {
            return new ClientWorker(endpointHost, endpointPath, proxyHost, proxyPort);
        }
    }
}