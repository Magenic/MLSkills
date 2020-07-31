
namespace SkillsRecommender.Library
{
    public class EmployeeSkillTrainer
    {
        public string PersonnelNumber { get; set; }
        public float SkillId { get; set; }
        public float JobAreaId { get; set; }
        public float SkillOfferingId { get; set; }
        public float Label { get; set; }

        public string Features
        {
            get { return $"{SkillId.ToString().PadLeft(5, '0')}|{JobAreaId.ToString().PadLeft(5, '0')}|{SkillOfferingId.ToString().PadLeft(5, '0')}"; }
        }
    }
}
