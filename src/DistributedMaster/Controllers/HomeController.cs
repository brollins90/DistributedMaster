namespace DistributedMaster.Controllers
{
    using Client;
    using Microsoft.AspNet.Mvc;
    using System.Collections.Generic;

    [Route("[controller]")]
    public class HomeController : Controller
    {
        RemoteClientWorkerFactory factory = new RemoteClientWorkerFactory();
        List<ClientWorker> workers = new List<ClientWorker>();

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Post()
        {
            var worker = factory.Create();
            workers.Add(worker);
            worker.Start();
            return RedirectToAction("index");
        }
    }
}