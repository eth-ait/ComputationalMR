using System;
using System.Collections.Generic;
using ConstraintUI.Model;
using ConstraintUI.Optimization;

namespace ConstraintUI.Util
{
    public static class CalcUtil
    {
        public static double GetCurrentUtilityForElement(ElementModel element)
        {
            return Math.Pow(element.Importance, 2) + (double)element.CurrentLevelOfDetail / element.MaxLevelOfDetail * element.Importance;
        }
        public static double GetUtilityForElement(ElementModel element, int lod)
        {
            return Math.Pow(element.Importance, 2) + (double)lod / element.MaxLevelOfDetail * element.Importance;
        }
        public static double GetObjectiveForElement(ElementModel element, int lod, OptimizationModel model)
        {
            var importance = Math.Pow(element.Importance, 2);
            var utility = (double)lod / element.MaxLevelOfDetail * importance;
            var visibilityReward = 1.0 + model.VisibilityReward * element.Visibility;

            return (importance * model.WeightImportance + utility * model.WeightUtility) * visibilityReward;
        }

        public static double GetTimeDependentCognitiveLoadOnsetPenalty(ElementModel element, OptimizationModel model)
        {
            var cogLoadOnsetPenalty = model.CognitiveLoadOnsetPenalty *
                          Math.Exp(-element.TimeVisible / model.CognitiveLoadOnsetPenaltyDecay);
            return cogLoadOnsetPenalty;
        }

        public static List<double> GetAllCognitiveLoadsForElement(ElementModel element)
        {
            var allLoads = new List<double>();

            for (var i = 1; i <= element.MaxLevelOfDetail; i++)
            {
                allLoads.Add((double)i / element.MaxLevelOfDetail * element.MaxCognitiveLoad);
            }

            return allLoads;
        }
    }
}