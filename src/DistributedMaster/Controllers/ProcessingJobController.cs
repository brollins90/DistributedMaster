namespace DistributedMaster.Controllers
{
    using ViewModels;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Framework.Logging;
    using Models;
    using Services;
    using System;
    using System.Linq;
    using System.Net;

    [Route("api/processingjob")]
    public class ProcessingJobController : Controller
    {
        private ILogger<ProcessingJobController> _logger;
        private ProcessingJobService _service;

        public ProcessingJobController(
            ILogger<ProcessingJobController> logger,
            ProcessingJobService service)
        {
            _logger = logger;
            _service = service;
        }

        // GET: /api/processingjob
        [HttpGet]
        public JsonResult Get()
        {
            _logger.LogInformation("Getting jobs");
            var jobs = _service.GetAll();
            var retVm = jobs.Select(job => new ProcessingJobViewModel
            {
                Id = job.Id,
                Name = job.Name,
                Ref = $"/api/processingjob/{job.Id}",
                Result = job.Result,
                Status = job.Status,
                WorkUrl = $"/api/processingjob/{job.Id}/work/"

            });
            return Json(retVm);
        }

        // GET: /api/processingjob/{id}
        [HttpGet("{id}")]
        public JsonResult Get(Guid id)
        {
            _logger.LogInformation("Getting specific job");
            var job = _service.GetById(id);
            var retVm = new ProcessingJobViewModel
            {
                Id = job.Id,
                Name = job.Name,
                Ref = $"/api/processingjob/{job.Id}",
                Status = job.Status,
                WorkUrl = $"/api/processingjob/{job.Id}/work/"
            };
            return Json(retVm);
        }

        // POST: /api/processingjob
        [HttpPost]
        public JsonResult Post([FromBody]ProcessingJobSubmitViewModel vm)
        {
            _logger.LogInformation("Creating job");
            if (vm != null)
            {

                try
                {
                    if (ModelState.IsValid)
                    {
                        var newJob = new ProcessingJob
                        {
                            Id = Guid.NewGuid(),
                            Name = vm.Name,
                            Status = "NotDone"
                        };

                        var work = vm.Work.Select(x => new ProcessingJobWork
                        {
                            ClientId = Guid.Empty,
                            EndIndex = x.EndIndex,
                            Goal = x.Goal,
                            JobId = newJob.Id,
                            List1 = x.List1,
                            List2 = x.List2,
                            StartIndex = x.StartIndex,
                            Status = "NotStarted",
                            WorkId = x.WorkId
                        });

                        if (_service.AddJob(newJob))
                        {
                            if (_service.AddJobWork(work))
                            {
                                var retVm = new ProcessingJobViewModel
                                {
                                    Id = newJob.Id,
                                    Name = newJob.Name,
                                    Ref = $"/api/processingjob/{newJob.Id}",
                                    Status = newJob.Status,
                                    WorkUrl = $"/api/processingjob/{newJob.Id}/work/"
                                };
                                Response.StatusCode = (int)HttpStatusCode.Created;
                                return Json(retVm);
                            }
                            else
                            {
                                //_service.RemoveJob(newJob);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { Message = ex.Message });
                }
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { Message = "Failed", ModelState = ModelState });
        }
    }
}
