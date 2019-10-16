using System;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class CalcUtil
    {
        public static double GetCurrentUtilityForElement(ElementModel element)
        {
            return Math.Pow(element.ImportanceNorm, 2) + (double)element.CurrentLevelOfDetail / element.MaxCognitiveLoadNorm * element.ImportanceNorm;
        }
        public static float GetUtilityForElement(ElementModel element, int lod)
        {
            return  (float)Math.Pow(element.ImportanceNorm, 2.0f) + (float)lod / element.MaxLevelOfDetail * element.ImportanceNorm;
        }

        public static float GetCurrentCognitiveLoadForElement(ElementModel element)
        {
            var load = (float)element.CurrentLevelOfDetail / element.MaxLevelOfDetail * element.MaxCognitiveLoadNorm;
            load /= 3f;
            return load;
        }

        public static double GetObjectiveForElement(ElementModel element, int lod, OptimizationModel model)
        {
            var importance = Math.Pow(element.ImportanceNorm, 2);
            var utility = GetUtilityForElement(element, lod);// (double)lod / element.MaxLevelOfDetail * importance;
            var visibilityReward = 1.0 + model.VisibilityReward * element.Visibility;

            return (importance * model.WeightImportance + utility * model.WeightUtility) * visibilityReward;
        }

        public static double GetTimeDependentCognitiveLoadOnsetPenalty(ElementModel element, OptimizationModel model)
        {
            var cogLoadOnsetPenalty = model.CognLoadOnsetPenalty *
                                      Math.Exp(-element.TimeVisible / model.CognLoadOnsetPenaltyDecay);
            return cogLoadOnsetPenalty;
        }

        public static List<double> GetAllCognitiveLoadsForElement(ElementModel element)
        {
            var allLoads = new List<double>();

            for (var i = 1; i <= element.MaxLevelOfDetail; i++)
            {
                var load = (double) i / element.MaxLevelOfDetail * element.MaxCognitiveLoadNorm;
                allLoads.Add(load / 3f);
            }

            return allLoads;
        }

        public static bool AreEqual(float value1, float value2)
        {
            return Mathf.Abs(value1 - value2) < Mathf.Epsilon;
        }

        public static bool AreEqual(int value1, int value2)
        {
            return value1.Equals(value2);
        }
    }
}