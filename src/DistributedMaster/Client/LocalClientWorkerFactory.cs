namespace DistributedMaster.Client
{
    public class LocalClientWorkerFactory
    {
        RemoteClientWorkerFactory _remoteFactory = new RemoteClientWorkerFactory();

        public ClientWorker Create()
        {
            return _remoteFactory.Create();
        }
    }
}