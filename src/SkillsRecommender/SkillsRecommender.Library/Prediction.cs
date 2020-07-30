using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;

namespace SkillsRecommender.Library
{
    public class Prediction
    {
        public static (List<Skill> currentSkills, List<EmployeeSkillPrediction> recommendedSkills) GetRecommendedSkills(string modelFileLocation, string employeeSkillFileLocation, string skillFileLocation, string personnelNumber)
        {

            MLContext mlContext = new MLContext();
            List<Skill> currentSkills = new List<Skill>();

            (var model, var schema) = LoadModel(mlContext, modelFileLocation);

            var returnValue = new List<EmployeeSkillPrediction>();
            var predictionEngine = mlContext.Model.CreatePredictionEngine<EmployeeSkillTrainer, EmployeeSkillPrediction>(model);

            var skills = DataAccess.LoadSkillData(mlContext, skillFileLocation);

            var allEmployeeSkills = DataAccess.LoadEmployeeSkillData(mlContext, employeeSkillFileLocation, skills, personnelNumber);
            var employeeSkills = mlContext.Data
                .CreateEnumerable<EmployeeSkillTrainer>(allEmployeeSkills, reuseRowObject: false)
                .Where(_ => _.PersonnelNumber == personnelNumber).ToList();


            foreach (var skill in skills)
            {
                if (!employeeSkills.Any(_ => _.SkillId == skill.SkillId))
                {
                    var testInput = new EmployeeSkillTrainer { PersonnelNumber = personnelNumber, SkillId = skill.SkillId, JobAreaId = employeeSkills.First().JobAreaId, SkillOfferingId = skill.SkillOfferingId };
                    var prediction = predictionEngine.Predict(testInput);
                    prediction.SkillDescription = skill.Description;
                    returnValue.Add(prediction);
                }
                else
                {
                    currentSkills.Add(skill);
                }
            }
            returnValue = returnValue.Where(_ => _.PredictedLabel).OrderByDescending(_ => _.Score).Take(20).ToList();

            return (currentSkills, returnValue);
        }

        private static (ITransformer model, DataViewSchema schema) LoadModel(MLContext mlContext, string modelPath)
        {
            DataViewSchema modelSchema;
            var model = mlContext.Model.Load(modelPath, out modelSchema);
            return (model, modelSchema);
        }
    }
}