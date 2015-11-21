namespace DistributedMaster.Controllers
{
    using Models;
    using Microsoft.AspNet.Mvc;
    using Services;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ViewModels;
    using System.Net;
    using Microsoft.Framework.Logging;

    [Route("api/processingjob/{jobId}/work")]
    public class ProcessingJobWorkController : Controller
    {
        private ILogger<ProcessingJobWorkController> _logger;
        private ProcessingJobService _service;

        public ProcessingJobWorkController(
            ILogger<ProcessingJobWorkController> logger,
            ProcessingJobService service)
        {
            _logger = logger;
            _service = service;
        }

        // GET: /api/processingjob/{jobId}/work
        [HttpGet]
        public JsonResult Get(Guid jobId)
        {
            _logger.LogInformation("Getting work");
            var works = _service.GetWorkForJob(jobId);
            var results = works.Select(x => new ProcessingJobWorkViewModel
            {
                EndIndex = x.EndIndex,
                Goal = x.Goal,
                JobId = x.JobId,
                List1 = x.List1,
                List2 = x.List2,
                RequestWorkUrl = $"/api/processingjob/{x.JobId}/work/{x.WorkId}",
                ResultUrl = $"/api/processingjob/{x.JobId}/work/{x.WorkId}/result",
                StartIndex = x.StartIndex,
                Status = x.Status,
                WorkId = x.WorkId
            });
            return Json(results);
        }

        // PUT: /api/processingjob/{jobId}/work/{id}
        [HttpPut("{id}")]
        public async Task<JsonResult> Put(Guid jobId, Guid id, [FromBody]ProcessingJobWorkConfirmViewModel vm)
        {
            _logger.LogInformation("Requesting work");
            var work = _service.GetWorkById(id);
            if (work.ClientId == Guid.Empty && !work.Status.Equals("Done"))
            {
                var clientIdString = HttpContext.Request.Headers["X-DistributedClientId"];
                var clientId = Guid.Parse(clientIdString);
                work.ClientId = clientId;
                work.Status = "Processing";
                _service.UpdateWork(work);

                Response.StatusCode = (int)HttpStatusCode.OK;
                vm.ClientId = work.ClientId;
                return Json(vm);
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.Conflict;
                return Json(new { Message = "Conflict", ModelState = ModelState });
            }
        }

        // PUT: /api/processingjob/{jobId}/work/{workId}/result
        [HttpPut("{id}/result")]
        public async Task<JsonResult> PutResult(Guid jobId, Guid id, [FromBody]ProcessingJobWorkResultViewModel vm)
        {
            _logger.LogInformation("Updating work");
            var work = _service.GetWorkById(id);

            var clientIdString = HttpContext.Request.Headers["X-DistributedClientId"];
            var clientId = Guid.Parse(clientIdString);

            if (work.ClientId == clientId && !work.Status.Equals("Done"))
            {
                work.Status = "Done";
                _service.UpdateWork(work);

                if (vm.Success)
                {
                    var job = _service.GetById(jobId);
                    job.Status = "Done";
                    job.Result = vm.Result;
                    _service.UpdateJob(job);
                }

                Response.StatusCode = (int)HttpStatusCode.OK;
                vm.ClientId = work.ClientId;
                return Json(vm);
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.Conflict;
                return Json(new { Message = "Conflict", ModelState = ModelState });
            }
        }
    }
}