using Microsoft.ML.Data;

namespace SkillsRecommender.Library
{
    public class EmployeeSkill
    {
        [LoadColumn(0)]
        public string PersonnelNumber { get; set; }
        [LoadColumn(2)]
        public float SkillId { get; set; }
        [LoadColumn(4)]
        public float SkillLevel { get; set; }
        [LoadColumn(6)]
        public float JobAreaId { get; set; }
    }
}