using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System.Linq;
using SkillsRecommender.Library;

namespace SkillsRecommender.ModelBuilder
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var employeeSkillPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Data", "Magenic_employee_Skills.csv");
            var skillListPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Data", "Magenic_Skills.csv");

            var trainingResult = Training.TrainModel(employeeSkillPath, skillListPath);


            var modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "SkillRecommenderModel.zip");

            Training.SaveModel(trainingResult.model, trainingResult.schema, modelPath);

            var (currentSkills, recommendedSkills) = Prediction.GetRecommendedSkills(modelPath, employeeSkillPath, skillListPath, "E4012");
        }

    }

}
