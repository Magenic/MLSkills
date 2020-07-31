using Microsoft.ML.Data;

namespace SkillsRecommender.Library
{
    public class Skill
    {
        [LoadColumn(Constants.SkillSkillIdColumn)]
        public float SkillId { get; set; }
        [LoadColumn(Constants.SkillDescriptionColumn)]
        public string Description { get; set; }
        [LoadColumn(Constants.SkillSkillOfferingIdColumn)]
        public float SkillOfferingId { get; set; }
    }
}
