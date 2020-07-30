using Microsoft.ML.Data;

namespace SkillsRecommender.Library
{
    public class Skill
    {
        [LoadColumn(0)]
        public float SkillId { get; set; }
        [LoadColumn(1)]
        public string Description { get; set; }
        [LoadColumn(2)]
        public float SkillOfferingId { get; set; }
    }
}
