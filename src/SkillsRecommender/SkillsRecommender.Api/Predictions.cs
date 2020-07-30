﻿using System;
using System.Collections.Generic;
using SkillsRecommender.Library;

namespace SkillsRecommender.Api
{
    public class Predictions
    {
        public IEnumerable<EmployeeSkillPrediction> RecomendedSkills { get; set; }

        public IEnumerable<Skill> CurrentSkills { get; set; }
    }
}