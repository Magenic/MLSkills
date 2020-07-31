using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;

namespace SkillsRecommender.Library
{
    public class DataAccess
    {
        public static IDataView LoadEmployeeSkillData(MLContext mlContext, string dataPath, List<Skill> skillData, string personnelNumber = "")
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<EmployeeSkill>(dataPath, hasHeader: true, separatorChar: Constants.SeparatorCharacter, allowSparse: true);

            var employeeSkills = mlContext.Data.CreateEnumerable<EmployeeSkill>(dataView, reuseRowObject: false).ToList();

            var returnList = new List<EmployeeSkillTrainer>();


            if (string.IsNullOrEmpty(personnelNumber))
            {
                foreach (var employeeSkill in employeeSkills)
                {
                    returnList.Add(new EmployeeSkillTrainer
                    {
                        PersonnelNumber = employeeSkill.PersonnelNumber,
                        JobAreaId = employeeSkill.JobAreaId,
                        SkillId = employeeSkill.SkillId,
                        SkillOfferingId = skillData.Single(_ => _.SkillId == employeeSkill.SkillId).SkillOfferingId,
                        Label = CalculateRating(employeeSkills, employeeSkill.SkillId, employeeSkill.JobAreaId)
                    });
                }
            }
            else
            {
                foreach (var employeeSkill in employeeSkills
                    .Where(_ => _.PersonnelNumber == personnelNumber))
                {
                    returnList.Add(new EmployeeSkillTrainer
                    {
                        PersonnelNumber = employeeSkill.PersonnelNumber,
                        JobAreaId = employeeSkill.JobAreaId,
                        SkillId = employeeSkill.SkillId,
                        SkillOfferingId = skillData.Single(_ => _.SkillId == employeeSkill.SkillId).SkillOfferingId,
                        Label = CalculateRating(employeeSkills, employeeSkill.SkillId, employeeSkill.JobAreaId)
                    });
                }
            }

            return mlContext.Data.LoadFromEnumerable(returnList);
        }

        private static float CalculateRating(List<EmployeeSkill> employeeSkills, float skillId, float jobAreaId)
        {
            var countOfSkills = employeeSkills.Count(_ => _.SkillId == skillId && _.JobAreaId == jobAreaId);
            var totalSkillLevel = employeeSkills.Where(_ => _.SkillId == skillId && _.JobAreaId == jobAreaId).Sum(_ => _.SkillLevel);
            var factoredSkillLevel = totalSkillLevel * Constants.SkillWeight;
            return (float)(countOfSkills + factoredSkillLevel);
        }

        public static List<Skill> LoadSkillData(MLContext mlContext, string dataPath)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<Skill>(dataPath, hasHeader: true, separatorChar: Constants.SeparatorCharacter);

            var returnValue = mlContext.Data.CreateEnumerable<Skill>(dataView, reuseRowObject: false).ToList();

            return returnValue;
        }
    }
}
