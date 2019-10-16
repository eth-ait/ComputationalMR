using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts.Event;
using Assets.Scripts.Model;
using Assets.Scripts.Optimization;
using Assets.Scripts.Util;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Controller
{
    public class AppController : MonoBehaviour
    {
        public event EventHandler<OptimizationCompletedEventArgs> OnOptimizationCompleted;

        public AppModel AppModel;

        private Optimizer _optimizer;

        private bool _started;

        void Start()
        {
        }

        void Update()
        {
            if (!_started)
            {
                _started = true;
                UpdateModelForTask(AppModel.CurrentTask);
            }

            //Weird things happen it autooptimization is started too soon.
            if (AppModel.OptimizationModel.AutoOptimizeEnabled)
            {
                var cognCapacityDelta = Mathf.Abs(AppModel.OptimizationModel.LastRunCognCapacity - AppModel.User.CognitiveCapacity);

                if (cognCapacityDelta > AppModel.OptimizationModel.MinChangeCapacity)
                {
                    AppModel.OptimizationModel.LastRunCognCapacity = AppModel.User.CognitiveCapacity;
                    StartOptimization();
                }
            }

            if (AppModel.EnableAutoPlacement)
            {
                //foreach (var placementSlotModel in AppModel.PlacementSlotsView)
                //{
                //    placementSlotModel.AppElement = null;
                //}
                foreach (var elementModel in AppModel.Elements)
                {
                    if (elementModel.CurrentLevelOfDetail > 1)
                    {
                        if (elementModel.PlacedViewAnchored && elementModel.ElementIsInView)
                        {
                            elementModel.PlaceInSpace(false);

                            foreach (var placementSlotModel in AppModel.PlacementSlotsView)
                            {
                                if (placementSlotModel.AppElement == elementModel)
                                    placementSlotModel.AppElement = null;
                            }
                        }
                        else if (!elementModel.PlacedViewAnchored && !elementModel.ElementIsInView && !elementModel.FixateElement)
                        {
                            var availableSlots = AppModel.PlacementSlotsView.Where(slot => slot.AppElement == null).ToList();
                            var bestSlot = availableSlots.OrderByDescending(slot => slot.Quality).FirstOrDefault();

                            if (bestSlot != null)
                            {
                                bestSlot.AppElement = elementModel;
                                elementModel.PlaceInView(bestSlot, false);
                            }
                        }
                    }
                }
            }
        }

        public void SwapElements()
        {
            foreach (var appModelElement in AppModel.Elements)
            {
                appModelElement.SwapElements();
            }
        }

        public void StartOptimization()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            if (_optimizer == null)
                _optimizer = new Optimizer(AppModel.OptimizationModel);

            UpdateOptimizerModel();
            _optimizer.StartOptimization();

            SelectSolution(0);

            watch.Stop();
            Debug.Log("Finished optimization in " + watch.ElapsedMilliseconds + "ms");

            OnOptimizationCompleted?.Invoke(this, new OptimizationCompletedEventArgs(AppModel.OptimizationModel, watch.Elapsed));
        }

        private void UpdateOptimizerModel()
        {
            AppModel.OptimizationModel.Elements.Clear();
            AppModel.OptimizationModel.Elements.AddRange(AppModel.Elements);

            AppModel.OptimizationModel.UserModel = AppModel.User;
        }

        public void InitFakeViewModel()
        {
            var newElements = new List<ElementModel>();

            foreach (var element in AppModel.Elements)
            {
                element.Importance = Random.Range(1, Constants.MAX_IMPORTANCE);
                element.MaxCognitiveLoad = Random.Range(3, Constants.MAX_COGNITIVE_LOAD);
                element.Visibility = 0;

                for (int i = 0; i < 9; i++)
                {
                    var clone = Instantiate(element);
                    clone.Importance = Random.Range(1, Constants.MAX_IMPORTANCE);
                    clone.MaxCognitiveLoad = Random.Range(3, Constants.MAX_COGNITIVE_LOAD);
                    clone.Identifier = AppModel.Elements.Count + newElements.Count;
                    clone.Visibility = 0;
                    newElements.Add(clone);
                }
            }

            AppModel.Elements.AddRange(newElements);

            AppModel.CurrentObjective = 0;
            AppModel.CurrentNumSolutions = 0;
            AppModel.User.CognitiveLoad = 0;

            //for (int i = 0; i < 10; i++)
            //{
            //    var element = new ElementModel();
            //    element.Identifier = i;
            //    element.Importance = Random.Range(0.0f, 1.0f);
            //    element.CurrentLevelOfDetail = Random.Range(1, element.MaxLevelOfDetail + 1);
            //    element.MaxCognitiveLoad = Constants.INITIAL_ELEMENT_MAX_COG_LOAD_OFFSET + (1.0 - Constants.INITIAL_ELEMENT_MAX_COG_LOAD_OFFSET) * Random.Range(0.0f, 1.0f);
            //}

            //AppModel.User.CognitiveCapacity = Constants.INITIAL_COGNITIVE_CAPACITY;

            //AppModel.OptimizationModel.NumMinPlacementSlots = Constants.MIN_PLACEMENT_SLOTS;
            //AppModel.OptimizationModel.NumMaxPlacementSlots = Constants.TOTAL_NUM_VIEW_SLOTS;
            //AppModel.OptimizationModel.VisibilityReward = Constants.INITIAL_VISIBILITY_REWARD;
            //AppModel.OptimizationModel.CognLoadOnsetPenalty = Constants.INITIAL_COGNITIVE_LOAD_ONSET_PENALTY;
            //AppModel.OptimizationModel.CognLoadOnsetPenaltyDecay = Constants.INITIAL_COGNITIVE_LOAD_ONSET_PENALTY_DECAY_TIMESTEPS;
        }
        
        public void EvalOptimization()
        {
            Stopwatch watch = new Stopwatch();
            var timings = new List<long>();

            var numElements = AppModel.Elements.Count;
            var numLod = AppModel.Elements.Sum(e => e.MaxLevelOfDetail);

            for (int j = 0; j < 1000; j++)
            {
                AppModel.User.CognitiveLoad = Random.Range(.2f, .9f);

                watch.Start();

                if (_optimizer == null)
                    _optimizer = new Optimizer(AppModel.OptimizationModel);

                UpdateOptimizerModel();
                _optimizer.StartOptimization();

                SelectSolution(0);

                watch.Stop();
                timings.Add(watch.ElapsedMilliseconds);
                watch.Reset();
            }

            float average = (float) timings.Sum() / timings.Count;
            var squares_query =  from long value in timings select (value - average) * (value - average);
            var sum_of_squares = squares_query.Sum();
            var stdDev = Math.Sqrt((float)sum_of_squares / timings.Count);

            Debug.Log("Finished optimization for " + numElements + "with "+ numLod +"LODs. Average " + average + "ms. StdDev " + stdDev);
            Debug.Log(timings);
        }

        //public void InitModels()
        //{
        //    foreach (var element in AppModel.Elements)
        //    {
        //        AppModel.ElementCandidates.Add(element);
        //    }
        //}

        public void SelectSolution(int selectedSolutionIndex)
        {
            AppModel.ModelIsFeasible = AppModel.OptimizationModel.IsFeasible;
            AppModel.CurrentObjective = 0;

            AppModel.ViewElements.Clear();
            AppModel.ElementCandidates.Clear();

            AppModel.CurrentNumSolutions = AppModel.OptimizationModel.Solutions.Count;

            if (!AppModel.ModelIsFeasible)
                return;

            AppModel.SelectedSolution = selectedSolutionIndex;
            var solution = AppModel.OptimizationModel.Solutions[selectedSolutionIndex];

            AppModel.CurrentObjective = solution.Objective;

            foreach (var appModelElement in AppModel.Elements)
            {
                var isSolutionElement = solution.SolutionElements.Where(e => e.Identifier == appModelElement.Identifier).Sum(e => e.LodAndVisiblityDictionary.Sum(v => v.Value)) > 0;
                appModelElement.Visibility = 0.0f;

                if (!isSolutionElement)
                {
                    appModelElement.SetCurrentLoD(1);
                    appModelElement.PlaceInSpace(true);
                    //this could be used to put an element back to a home position (cf. dock)
                    //if (!appModelElement.ElementIsInView)
                    //{
                    //    appModelElement.PlaceInSpace(true);
                    //}
                }
            }

            foreach (var element in solution.SolutionElements)
            {
                var appElement = AppModel.Elements.Single(e => e.Identifier == element.Identifier);

                foreach (var key in element.LodAndVisiblityDictionary.Keys)
                {
                    var visibility = element.LodAndVisiblityDictionary[key];

                    if (visibility > 0.0)
                    {
                        appElement.Visibility = 1.0f;
                        appElement.SetCurrentLoD(key + 1);
                        appElement.CurrentObjective = (float)CalcUtil.GetObjectiveForElement(appElement, appElement.CurrentLevelOfDetail, AppModel.OptimizationModel);
                    }
                }
            }

            UpdateModelForCurrentSolution();
            PlaceElements();

        }

        private void PlaceElements()
        {
            if (AppModel.Elements.Count < 1)
                return;

            var visibileElements = AppModel.Elements.Where(element => element.Visibility > 0.0).OrderByDescending(CalcUtil.GetCurrentUtilityForElement).ToList();

            if (AppModel.PlacementSlotsView.Count < 1)
                return;

            var slots = AppModel.PlacementSlotsView.OrderByDescending(slot => slot.Quality).ToList();

            var currentSlotRunner = 0;
            foreach (var element in visibileElements)
            {
                if (element != null)
                {
                    //if (element.ShouldPlaceInView && !element.FixateElement)
                    if (element.ShouldPlaceInView && currentSlotRunner < slots.Count)
                    {
                        var nextSlot = slots[currentSlotRunner];

                        while (!(nextSlot.AppElement == null) && currentSlotRunner < slots.Count - 1)
                        {
                            currentSlotRunner++;
                            nextSlot = slots[currentSlotRunner];
                        }

                        element.PlaceInView(nextSlot, true);
                        currentSlotRunner++;
                    }
                    else
                    {
                        element.PlaceInSpace(true);
                    }
                }
            }
        }

        private void UpdateModelForCurrentSolution()
        {
            var visibileElements = AppModel.Elements.Where(element => element.Visibility > 0.0)
                .OrderByDescending(CalcUtil.GetCurrentUtilityForElement).ToList();

            foreach (var element in AppModel.Elements)
            {
                if (!visibileElements.Contains(element))
                {
                    AppModel.ElementCandidates.Add(element);
                    element.LastIndex = -1;
                    element.TimeVisible = 0;
                }
            }

            var tempViewElements = new List<ElementModel>();
            for (var i = 0; i < Constants.TOTAL_NUM_VIEW_SLOTS; i++)
            {
                tempViewElements.Add(null);
            }

            var fixedIndexElements = visibileElements.Where(element => element.LastIndex > -1).ToList();
            var newElements = visibileElements.Where(element => element.LastIndex == -1).ToList();

            foreach (var element in fixedIndexElements)
            {
                element.TimeVisible++;
                tempViewElements[element.LastIndex] = element;
            }

            foreach (var element in newElements)
            {
                for (int j = 0; j < Constants.TOTAL_NUM_VIEW_SLOTS; j++)
                {
                    if (tempViewElements[j] == null)
                    {
                        tempViewElements[j] = element;
                        element.LastIndex = j;
                        break;
                    }
                }
            }

            foreach (var tempViewElement in tempViewElements)
            {
                AppModel.ViewElements.Add(tempViewElement);
            }

            //var currentCognitiveLoad = visibileElements.Sum(element => element.CurrentCognitiveLoad);
            //AppModel.User.CognitiveLoad = currentCognitiveLoad;
        }

        public void ResetSolution()
        {
            AppModel.ViewElements.Clear();
            AppModel.ElementCandidates.Clear();

            AppModel.OptimizationModel.Reset();

            AppModel.CurrentNumSolutions = 0;
            AppModel.CurrentObjective = 0;

            foreach (var element in AppModel.Elements)
            {
                element.Visibility = 0;
                element.SetCurrentLoD(1, false);
                element.PlaceInSpace(false);
                element.LastIndex = -1;
                element.TimeVisible = 0;

                AppModel.ElementCandidates.Add(element);
            }
        }

        public void HandleTaskButton(TaskModel task)
        {
            UpdateModelForTask(task);
        }

        public void UpdateModelForTask(TaskModel task)
        {
            AppModel.CurrentTask = task;

            foreach (var taskAppModel in task.AppImportanceDictionary)
            {
                taskAppModel.Application.Importance = taskAppModel.Importance;
            }
        }
    }
}
