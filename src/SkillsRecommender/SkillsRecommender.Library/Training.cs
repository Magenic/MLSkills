using System;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using System.Linq;
using Microsoft.ML.Data;
using System.Collections.Generic;

namespace SkillsRecommender.Library
{
    public class Training
    {

        public static (ITransformer model, BinaryClassificationMetrics metrics, DataViewSchema schema) TrainModel(string employeeSkillFileLocation, string skillFileLocation)
        {
            MLContext mlContext = new MLContext();

            var skills = DataAccess.LoadSkillData(mlContext, skillFileLocation);
            var dataView = DataAccess.LoadEmployeeSkillData(mlContext, employeeSkillFileLocation, skills);

            var splitData = SplitData(mlContext, dataView);

            var estimator = TrainModel(mlContext, splitData.trainData);

            //var metrics = EvaluateModel(mlContext, splitData.testData, estimator);

            return (estimator, null, dataView.Schema);
        }

        private static (IDataView testData, IDataView trainData) SplitData(MLContext mlContext, IDataView dataToSplit)
        {

            var split = mlContext.Data.TrainTestSplit(dataToSplit, testFraction: 0.1);

            var trainSet = mlContext.Data
                .CreateEnumerable<EmployeeSkillTrainer>(split.TrainSet, reuseRowObject: false);

            var testSet = mlContext.Data
                .CreateEnumerable<EmployeeSkillTrainer>(split.TestSet, reuseRowObject: false);

            var trainLoader = mlContext.Data.LoadFromEnumerable(trainSet);

            var testLoader = mlContext.Data.LoadFromEnumerable(testSet);

            return (trainLoader, testLoader);
        }

        private static ITransformer TrainModel(MLContext mlContext, IDataView trainingData)
        {
            //IEstimator<ITransformer> estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "PersonnelNumberEncoded", inputColumnName: "PersonnelNumber")
            //                        .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "FeaturesEncoded", inputColumnName: "Features"))
            //                        .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "SkillIdEncoded", inputColumnName: "SkillId"));

            //var options = new MatrixFactorizationTrainer.Options
            //{
            //    MatrixColumnIndexColumnName = "PersonnelNumberEncoded",
            //    MatrixRowIndexColumnName = "FeaturesEncoded",
            //    LabelColumnName = "SkillLevel",
            //    NumberOfIterations = 300,
            //    ApproximationRank = 3
            //};

            //var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));
            //return trainerEstimator.Fit(trainingData);


            var options = new FieldAwareFactorizationMachineTrainer.Options
            {
                FeatureColumnName = "Features",
                LabelColumnName = "HasSkill",
                NumberOfIterations = 20,
            };
            var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "PersonnelNumberFeaturized", inputColumnName: nameof(EmployeeSkillTrainer.PersonnelNumber))
                                          .Append(mlContext.Transforms.Concatenate("Features", "PersonnelNumberFeaturized", "JobAreaId", "SkillOfferingId", "SkillId"));

            var trainingPipeLine = dataProcessPipeline.Append(mlContext.BinaryClassification.Trainers.FieldAwareFactorizationMachine(options));
            return trainingPipeLine.Fit(trainingData);
        }

        private static BinaryClassificationMetrics EvaluateModel(MLContext mlContext, IDataView testData, ITransformer model)
        {
            var prediction = model.Transform(testData);

            Console.WriteLine(prediction);
            Console.WriteLine(prediction.Schema[0]);

            var metrics = mlContext.BinaryClassification.Evaluate(prediction, labelColumnName: "HasSkill", scoreColumnName: "Score", predictedLabelColumnName: "PredictedLabel");

            Console.WriteLine("Accuracy : " + metrics.Accuracy.ToString());
            Console.WriteLine("Loss: " + metrics.LogLoss.ToString()); ;

            return metrics;
        }

        public static void SaveModel(ITransformer model, DataViewSchema schema, string modelPath)
        {
            MLContext mlContext = new MLContext();

            mlContext.Model.Save(model, schema, modelPath);
        }
    }
}
