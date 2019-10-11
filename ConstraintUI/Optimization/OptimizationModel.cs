using System.Collections.Generic;
using System.Runtime.InteropServices;
using ConstraintUI.Model;

namespace ConstraintUI.Optimization
{
    public class OptimizationModel
    {
        public List<ElementModel> Elements { get; set; }

        public List<SolutionModel> Solutions { get; }
        public UserModel UserModel { get; set; }

        public int NumMinPlacementSlots { get; set; }
        public int NumMaxPlacementSlots { get; set; }

        public bool IsFeasible { get; set; }
        public double VisibilityReward { get; set; }
        public double WeightImportance { get; set; }
        public double WeightUtility { get; set; }   
        public double CognitiveLoadOnsetPenalty { get; set; }
        public double CognitiveLoadOnsetPenaltyDecay { get; set; }

        public OptimizationModel()
        {
            Elements = new List<ElementModel>();
            Solutions = new List<SolutionModel>();
        }
    }
}