using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using SkillsRecommender.Library;

namespace SkillsRecommender.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredictionController
    {
        public PredictionController()
        {
        }

        [HttpGet]
        public Predictions Get(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
            {
                throw new ArgumentNullException($"Parameter employeeId cannot be empty.");
            }
            var returnValue = new Predictions();

            var employeeSkillPath = System.IO.Path.Combine(Environment.CurrentDirectory, Constants.Data_directory, Constants.Employee_skills_file);
            var skillListPath = System.IO.Path.Combine(Environment.CurrentDirectory, Constants.Data_directory, Constants.Skill_file);
            var modelPath = System.IO.Path.Combine(Environment.CurrentDirectory, Constants.Data_directory, Constants.model_file);

            var (currentSkills, recomendedSkills) = Prediction.GetRecommendedSkills(modelPath, employeeSkillPath, skillListPath, employeeId);

            returnValue.CurrentSkills = currentSkills.AsEnumerable();
            returnValue.RecommendedSkills = recomendedSkills;
            return returnValue;
        }
    }
}
