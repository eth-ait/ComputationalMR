using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using ConstraintUI.Annotations;
using ConstraintUI.Util;

namespace ConstraintUI.Model
{
    public class ElementModel : INotifyPropertyChanged
    {
        private int _identifier;
        private int _maxLevelOfDetail;
        private double _visibility;
        private double _importance;
        private double _maxCognitiveLoad;
        private int _currentLevelOfDetail;
        private bool _fixateElement;
        private bool _omitElement;

        public int Identifier
        {
            get => _identifier;
            set
            {
                _identifier = value;
                OnPropertyChanged();
            }
        }

        public int MaxLevelOfDetail
        {
            get => _maxLevelOfDetail;
            set
            {
                _maxLevelOfDetail = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentCognitiveLoad));
                OnPropertyChanged(nameof(CurrentUtility));
            }
        }
        public int CurrentLevelOfDetail
        {
            get => _currentLevelOfDetail;
            set
            {
                _currentLevelOfDetail = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentCognitiveLoad));
                OnPropertyChanged(nameof(CurrentUtility));
            }
        }

        public double Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentCognitiveLoad));
                OnPropertyChanged(nameof(CurrentUtility));
            }
        }

        public double Importance
        {
            get => _importance;
            set
            {
                _importance = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentCognitiveLoad));
                OnPropertyChanged(nameof(CurrentUtility));
            }
        }

        public double MaxCognitiveLoad
        {
            get => _maxCognitiveLoad;
            set
            {
                if (value.Equals(_maxCognitiveLoad)) return;
                _maxCognitiveLoad = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentCognitiveLoad));
                OnPropertyChanged(nameof(CurrentUtility));
            }
        }

        public bool FixateElement
        {
            get => _fixateElement;
            set
            {
                if (value == _fixateElement) return;
                _fixateElement = value;

                OnPropertyChanged();
            }
        }

        public bool OmitElement
        {
            get => _omitElement;
            set
            {
                if (value == _omitElement)
                    return;
                _omitElement = value;

                OnPropertyChanged();
            }
        }

        public int LastIndex { get; set; }
        public int TimeVisible { get; set; }

        public double CurrentUtility => CalcUtil.GetUtilityForElement(this, CurrentLevelOfDetail);


        public double CurrentCognitiveLoad => (double)CurrentLevelOfDetail / MaxLevelOfDetail * MaxCognitiveLoad;

        public event PropertyChangedEventHandler PropertyChanged;

        public ElementModel()
        {
            LastIndex = -1;
            TimeVisible = 0;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}