using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Util;

namespace Assets.Scripts.Model
{
    public class OptimizationModel : MonoBehaviour
    {
        public List<ElementModel> Elements { get; set; }

        public List<SolutionModel> Solutions { get; }
        
        public List<PlacementSlotModel> PlacementSlotsView { get; set; }

        public UserModel UserModel;

        public bool AutoOptimizeEnabled = false;
        public float MinChangeCapacity = Constants.INITIAL_MIN_CHANGE_COGN_CAPACITY;

        public int NumMinPlacementSlots = Constants.MIN_PLACEMENT_SLOTS;
        public int NumMaxPlacementSlots = Constants.TOTAL_NUM_VIEW_SLOTS;

        [Util.ReadOnly] public bool IsFeasible;
       
        public float VisibilityReward = Constants.INITIAL_VISIBILITY_REWARD;
        public float WeightImportance = Constants.INITIAL_WEIGHT_IMPORTANCE;
        public float WeightUtility = 1.0f - Constants.INITIAL_WEIGHT_IMPORTANCE;
        public float CognLoadOnsetPenalty = Constants.INITIAL_COGNITIVE_LOAD_ONSET_PENALTY;
        public float CognLoadOnsetPenaltyDecay = Constants.INITIAL_COGNITIVE_LOAD_ONSET_PENALTY_DECAY_TIMESTEPS;


        public float LastRunCognCapacity { get; set; }

        public OptimizationModel()
        {
            Elements = new List<ElementModel>();
            Solutions = new List<SolutionModel>();
        }

        public void Reset()
        {
            LastRunCognCapacity = -1.0f;

            Elements.Clear();
            Solutions.Clear();
            
            //NumMinPlacementSlots = Constants.MIN_PLACEMENT_SLOTS;
            //NumMaxPlacementSlots = Constants.TOTAL_NUM_VIEW_SLOTS;

            //VisibilityReward = Constants.INITIAL_VISIBILITY_REWARD;
            //WeightImportance = Constants.INITIAL_WEIGHT_IMPORTANCE;
            //WeightUtility = 1.0f - Constants.INITIAL_WEIGHT_IMPORTANCE;
            //CognLoadOnsetPenalty = Constants.INITIAL_COGNITIVE_LOAD_ONSET_PENALTY;
            //CognLoadOnsetPenaltyDecay = Constants.INITIAL_COGNITIVE_LOAD_ONSET_PENALTY_DECAY_TIMESTEPS;
        }
    }
}