using System;
using System.Collections.Generic;
using System.Security.Permissions;
using ConstraintUI.Optimization;

namespace ConstraintUI.Model
{
    public class AppModel
    {
        public UserModel User { get; }

        public List<ElementModel> Elements { get; }

        //public int NumMaxPlacementSlots { get; set; }
        //public int NumMinPlacementSlots { get; set; }

        //public double VisibilityReward { get; set; }
        //public double WeightImportance { get; set; }
        //public double WeightUtility { get; set; }

        public OptimizationModel OptimizationModel { get; set; }

        public AppModel(OptimizationModel optimizationModel)
        {
            User = new UserModel();
            Elements = new List<ElementModel>();
            OptimizationModel = optimizationModel;
        }
    }
}