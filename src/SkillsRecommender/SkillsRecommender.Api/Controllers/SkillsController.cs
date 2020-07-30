using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using SkillsRecommender.Library;

namespace SkillsRecommender.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly ILogger<SkillsController> _logger;

        public SkillsController(ILogger<SkillsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Skill> Get()
        {
            MLContext mlContext = new MLContext();
            var skillListPath = System.IO.Path.Combine(Environment.CurrentDirectory, Constants.Data_directory, Constants.Skill_file);

            var returnValue = DataAccess.LoadSkillData(mlContext, skillListPath).ToArray();

            return returnValue;
        }
    }
}
