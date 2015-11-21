namespace DistributedMaster.Client
{
    public class LocalClientWorkerFactory
    {
        public ClientWorker Create()
        {
            return new ClientWorker(new LocalJobProcessingClient());
        }
    }
}