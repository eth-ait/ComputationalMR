using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Util;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class AppModel : MonoBehaviour
    {
        public UserModel User;

        public List<ElementModel> Elements;

        public OptimizationModel OptimizationModel;

        public TaskModel CurrentTask;

        [Util.ReadOnly] public double CurrentObjective;
        [Util.ReadOnly] public bool ModelIsFeasible;

        [Util.ReadOnly] public int CurrentNumSolutions;
        [Util.ReadOnly] public int SelectedSolution;

        public bool EnableAutoPlacement;

        private List<ElementModel> _elementCandidates;
        private List<ElementModel> _viewElements;
        private List<PlacementSlotModel> _placementSlotsView;

        public List<ElementModel> ElementCandidates
        {
            get
            {
                if (_elementCandidates == null)
                    _elementCandidates = new List<ElementModel>();

                return _elementCandidates;
            }
        }

        public List<ElementModel> ViewElements
        {
            get
            {
                if (_viewElements == null)
                    _viewElements = new List<ElementModel>();

                return _viewElements;
            }

            set => _viewElements = value;
        }


        public List<PlacementSlotModel> PlacementSlotsView
        {
            get
            {
                if (_placementSlotsView == null)
                    _placementSlotsView = new List<PlacementSlotModel>();

                return _placementSlotsView;
            }
        }
        

        public AppModel(OptimizationModel optimizationModel)
        {
            OptimizationModel = optimizationModel;
            OptimizationModel.PlacementSlotsView = PlacementSlotsView;
        }
        

        public double GetCurrentUtility()
        {
            if (ViewElements == null || ViewElements.Count <= 0)
                return 0.0;

            var utility = ViewElements.Sum(element => element != null ? CalcUtil.GetCurrentUtilityForElement(element) * element.Visibility : 0.0);
            return utility;
        }

        public double GetCurrentImportance()
        {
            if (ViewElements == null || ViewElements.Count <= 0)
                return 0.0;

            var importance = ViewElements.Sum(element => element != null ? element.ImportanceNorm * element.Visibility : 0.0);
            return importance;
        }
    }
}