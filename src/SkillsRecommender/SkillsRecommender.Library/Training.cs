using System;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using System.Linq;
using Microsoft.ML.Data;

namespace SkillsRecommender.Library
{
    public class Training
    {

        public static (ITransformer model, RegressionMetrics metrics, DataViewSchema schema) TrainModel(string employeeSkillFileLocation, string skillFileLocation)
        {
            MLContext mlContext = new MLContext();

            var skills = DataAccess.LoadSkillData(mlContext, skillFileLocation);
            var dataView = DataAccess.LoadEmployeeSkillData(mlContext, employeeSkillFileLocation, skills);

            var splitData = SplitData(mlContext, dataView);

            var estimator = TrainModel(mlContext, splitData.trainData);

            var metrics = EvaluateModel(mlContext, splitData.testData, estimator);

            return (estimator, metrics, dataView.Schema);
        }

        private static (IDataView testData, IDataView trainData) SplitData(MLContext mlContext, IDataView dataToSplit)
        {
            var split = mlContext.Data.TrainTestSplit(dataToSplit, testFraction: Constants.TestFraction);

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
            IEstimator<ITransformer> estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "PersonnelNumberEncoded", inputColumnName: nameof(EmployeeSkillTrainer.PersonnelNumber))
                                    .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "FeaturesEncoded", inputColumnName: nameof(EmployeeSkillTrainer.Features)));

            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "PersonnelNumberEncoded",
                MatrixRowIndexColumnName = "FeaturesEncoded",
                LabelColumnName = nameof(EmployeeSkillTrainer.Label),
                NumberOfIterations = 1000,
                ApproximationRank = 100
            };

            var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));
            return trainerEstimator.Fit(trainingData);
        }

        private static RegressionMetrics EvaluateModel(MLContext mlContext, IDataView testData, ITransformer model)
        {
            var prediction = model.Transform(testData);

            Console.WriteLine(prediction);
            Console.WriteLine(prediction.Schema[0]);

            var metrics = mlContext.Regression.Evaluate(prediction, labelColumnName: nameof(EmployeeSkillTrainer.Label), scoreColumnName: "Score");

            Console.WriteLine("Root Mean Squared Error : " + metrics.RootMeanSquaredError.ToString());
            Console.WriteLine("RSquared: " + metrics.RSquared.ToString());

            return metrics;
        }

        public static void SaveModel(ITransformer model, DataViewSchema schema, string modelPath)
        {
            MLContext mlContext = new MLContext();

            mlContext.Model.Save(model, schema, modelPath);
        }
    }
}
