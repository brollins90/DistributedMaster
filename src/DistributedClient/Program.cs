namespace DistributedClient
{
    using DistributedMaster.Client;

    public class Program
    {

        public void Main(string[] args)
        {
            ClientWorker worker = new RemoteClientWorkerFactory().Create();
            worker.Start().Wait();
        }
    }
}