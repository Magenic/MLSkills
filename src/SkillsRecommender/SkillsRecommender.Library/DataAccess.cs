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
            IDataView dataView = mlContext.Data.LoadFromTextFile<EmployeeSkill>(dataPath, hasHeader: true, separatorChar: ',', allowSparse: true);

            var employeeSkills = mlContext.Data.CreateEnumerable<EmployeeSkill>(dataView, reuseRowObject: false).ToList();

            var returnList = new List<EmployeeSkillTrainer>();


            if (string.IsNullOrEmpty(personnelNumber))
            {
                //var employeeGroups = employeeSkills.GroupBy(_ => _.PersonnelNumber)
                //    .ToList();
                //foreach (var employeeGroup in employeeGroups)
                //{
                //    var employeeId = employeeGroup.Key;
                //    foreach (var skill in skillData)
                //    {
                //        // There may be duplicates from Saba
                //        var employeeSkill = employeeSkills.FirstOrDefault(_ =>

                //            _.PersonnelNumber == employeeId &&
                //            _.SkillId == skill.SkillId);
                //        if (employeeSkill != null)
                //        {
                //            returnList.Add(new EmployeeSkillTrainer
                //            {
                //                PersonnelNumber = employeeId,
                //                JobAreaId = employeeSkill.JobAreaId,
                //                SkillId = skill.SkillId,
                //                SkillOfferingId = skill.SkillOfferingId,
                //                HasSkill = true
                //            });
                //        }
                //        else
                //        {
                //            returnList.Add(new EmployeeSkillTrainer
                //            {
                //                PersonnelNumber = employeeId,
                //                JobAreaId = employeeSkills.First(_ => _.PersonnelNumber == employeeId).JobAreaId,
                //                SkillId = skill.SkillId,
                //                SkillOfferingId = skill.SkillOfferingId,
                //                HasSkill = false
                //            });
                //        }
                //    }

                //}
                foreach (var employeeSkill in employeeSkills)
                {
                    returnList.Add(new EmployeeSkillTrainer
                    {
                        PersonnelNumber = employeeSkill.PersonnelNumber,
                        JobAreaId = employeeSkill.JobAreaId,
                        SkillId = employeeSkill.SkillId,
                        SkillOfferingId = skillData.Single(_ => _.SkillId == employeeSkill.SkillId).SkillOfferingId,
                        HasSkill = true
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
                        //SkillLevel = employeeSkill.SkillLevel
                        HasSkill = true
                    });
                }
            }

            return mlContext.Data.LoadFromEnumerable(returnList);
        }

        public static List<Skill> LoadSkillData(MLContext mlContext, string dataPath)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<Skill>(dataPath, hasHeader: true, separatorChar: ',');

            var returnValue = mlContext.Data.CreateEnumerable<Skill>(dataView, reuseRowObject: false).ToList();

            return returnValue;
        }
    }
}
