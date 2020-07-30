using System;
namespace SkillsRecommender.Library
{
    public class EmployeeSkillTrainer
    {
        public string PersonnelNumber { get; set; }
        public float SkillId { get; set; }
        public float JobAreaId { get; set; }
        public float SkillOfferingId { get; set; }
        public bool HasSkill { get; set; }
        //public float SkillLevel { get; set; }

        //public string Features
        //{
        //    get { return $"{SkillId.ToString("D5")}|{JobAreaId.ToString("D5")}|{SkillOfferingId.ToString("D5")}"; }
        //}
    }
}
