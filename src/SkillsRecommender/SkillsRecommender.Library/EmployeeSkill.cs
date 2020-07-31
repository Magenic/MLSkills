using Microsoft.ML.Data;

namespace SkillsRecommender.Library
{
    public class EmployeeSkill
    {
        [LoadColumn(Constants.PersonnelColumn)]
        public string PersonnelNumber { get; set; }
        [LoadColumn(Constants.SkillIdColumn)]
        public float SkillId { get; set; }
        [LoadColumn(Constants.SkillLevelColumn)]
        public int SkillLevel { get; set; }
        [LoadColumn(Constants.JobAreaIdColumn)]
        public float JobAreaId { get; set; }
    }
}