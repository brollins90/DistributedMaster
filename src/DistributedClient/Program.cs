namespace DistributedClient
{
    using DistributedMaster.Client;

    public class Program
    {

        public void Main(string[] args)
        {
            ClientWorker worker = new ClientWorkerFactory().Create();
            worker.Start().Wait();
        }
    }
}