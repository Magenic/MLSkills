using System;
using System.IO;
using SkillsRecommender.Library;

namespace SkillsRecommender.ModelBuilder
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var employeeSkillPath = System.IO.Path.Combine(Environment.CurrentDirectory, Constants.Data_directory, Constants.Employee_skills_file);
            var skillListPath = System.IO.Path.Combine(Environment.CurrentDirectory, Constants.Data_directory, Constants.Skill_file);

            var trainingResult = Training.TrainModel(employeeSkillPath, skillListPath);


            var modelPath = Path.Combine(Environment.CurrentDirectory, Constants.Data_directory, Constants.model_file);

            Training.SaveModel(trainingResult.model, trainingResult.schema, modelPath);

            var (currentSkills, recommendedSkills) = Prediction.GetRecommendedSkills(modelPath, employeeSkillPath, skillListPath, "E3574");
        }
    }

}
