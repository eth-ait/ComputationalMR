using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ConstraintUI.Annotations;
using ConstraintUI.Model;
using ConstraintUI.Util;

namespace ConstraintUI.View
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly AppModel _appModel;
        private ObservableCollection<ElementModel> _elementCandidates;
        private ObservableCollection<ElementModel> _viewElements;

        private bool _modelIsFeasible;

        private int _selectedSolution;
        private int _currentNumSolutions;
        private bool _autoOptimizeEnabled;
        private double _currentObjective;

        public double UserCognitiveLoad
        {
            get => _appModel.User.CognitiveLoad;
            set
            {
                _appModel.User.CognitiveLoad = value;
                OnPropertyChanged();
            }
        }

        public double UserCognitiveCapacity
        {
            get => _appModel.User.CognitiveCapacity;
            set
            {
                _appModel.User.CognitiveCapacity = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ElementModel> ElementCandidates
        {
            get => _elementCandidates;
            set
            {
                _elementCandidates = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ElementModel> ViewElements
        {
            get => _viewElements;
            set
            {
                _viewElements = value;
                OnPropertyChanged();
            }
        }

        public double CurrentUtility
        {
            get
            {
                if (_viewElements == null || _viewElements.Count <= 0)
                    return 0.0;

                var utility = _viewElements.Sum(element => element != null ? CalcUtil.GetCurrentUtilityForElement(element) * element.Visibility : 0.0);
                return utility;
            }
        }

        public double CurrentImportance
        {
            get
            {
                if (_viewElements == null || _viewElements.Count <= 0)
                    return 0.0;

                var importance = _viewElements.Sum(element => element != null ? element.Importance * element.Visibility : 0.0);
                return importance;
            }
        }

        public double CurrentObjective
        {
            get => _currentObjective;
            set
            {
                if (value.Equals(_currentObjective))
                    return;

                _currentObjective = value;
                OnPropertyChanged();
            }
        }

        public bool ModelIsFeasible
        {
            get => _modelIsFeasible;
            set
            {
                if (value == _modelIsFeasible)
                    return;

                _modelIsFeasible = value;

                OnPropertyChanged();
            }
        }

        public int CurrentNumSolutions
        {
            get => _currentNumSolutions - 1;
            set
            {
                if (value == _currentNumSolutions) return;
                _currentNumSolutions = value;

                OnPropertyChanged();
            }
        }

        public int SelectedSolution
        {
            get => _selectedSolution;
            set
            {
                if (value == _selectedSolution)
                    return;

                _selectedSolution = value;

                OnPropertyChanged();
            }
        }

        public int NumMinPlacementSlots
        {
            get => _appModel.OptimizationModel.NumMinPlacementSlots;
            set
            {
                _appModel.OptimizationModel.NumMinPlacementSlots = value;

                OnPropertyChanged();
            }
        }
        public int NumMaxPlacementSlots
        {
            get => _appModel.OptimizationModel.NumMaxPlacementSlots;
            set
            {
                _appModel.OptimizationModel.NumMaxPlacementSlots = value;

                OnPropertyChanged();
            }
        }

        public double VisibilityReward
        {
            get => _appModel.OptimizationModel.VisibilityReward;
            set
            {
                if (value.Equals(_appModel.OptimizationModel.VisibilityReward))
                    return;

                _appModel.OptimizationModel.VisibilityReward = value;

                OnPropertyChanged();
            }
        }

        public double WeightImportance
        {
            get => _appModel.OptimizationModel.WeightImportance;
            set
            {
                if (value.Equals(_appModel.OptimizationModel.WeightImportance))
                    return;

                _appModel.OptimizationModel.WeightImportance = value;
                WeightUtility = 1.0 - value;

                OnPropertyChanged();
            }
        }
        public double WeightUtility
        {
            get => _appModel.OptimizationModel.WeightUtility;
            set
            {
                if (value.Equals(_appModel.OptimizationModel.WeightUtility))
                    return;

                _appModel.OptimizationModel.WeightUtility = value;

                OnPropertyChanged();
            }
        }

        public double CognitiveLoadOnsetPenalty
        {
            get => _appModel.OptimizationModel.CognitiveLoadOnsetPenalty;
            set
            {
                if (value.Equals(_appModel.OptimizationModel.CognitiveLoadOnsetPenalty))
                    return;

                _appModel.OptimizationModel.CognitiveLoadOnsetPenalty = value;

                OnPropertyChanged();
            }
        }
        public double CognitiveLoadOnsetPenaltyDecay
        {
            get => _appModel.OptimizationModel.CognitiveLoadOnsetPenaltyDecay;
            set
            {
                if (value.Equals(_appModel.OptimizationModel.CognitiveLoadOnsetPenaltyDecay))
                    return;

                _appModel.OptimizationModel.CognitiveLoadOnsetPenaltyDecay = value;

                OnPropertyChanged();
            }
        }

        public bool AutoOptimizeEnabled
        {
            get => _autoOptimizeEnabled;
            set
            {
                if (value == _autoOptimizeEnabled) return;
                _autoOptimizeEnabled = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel(AppModel appModel)
        {
            _appModel = appModel;
            ElementCandidates = new ObservableCollection<ElementModel>();
            ViewElements = new ObservableCollection<ElementModel>();

            for (int i = 0; i < Constants.TOTAL_NUM_VIEW_SLOTS; i++)
            {
                ViewElements.Add(null);
            }

            ModelIsFeasible = true;
            AutoOptimizeEnabled = false;
            WeightImportance = Constants.INITIAL_WEIGHT_IMPORTANCE;

            ViewElements.CollectionChanged += delegate
            {
                OnPropertyChanged(nameof(CurrentImportance));
                OnPropertyChanged(nameof(CurrentUtility));
            };
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}