using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class PlacementSlotModel : MonoBehaviour
    {
        private bool _isDirty = true;
        private Queue<float> _qualities;

        public float SideLength;
        public Vector3 Position;

        public float Quality;
        public float QualityCMA;

        public Queue<float> Qualities
        {
            get
            {
                if (_qualities == null)
                    _qualities = new Queue<float>();

                return _qualities;
            }

            set => _qualities = value;
        }

        public int IntegrateQualityOverSteps { get; set; }
        private int _currentIntegrationSteps;

        public bool IsVisible;
        public bool IsOccluded;

        public GameObject Visual;
        public GameObject Container;

        public Transform Occluder;
        public Bounds Bounds { get; set; }

        public ElementModel AppElement;

        public Color Color;

        void Update()
        {
            if (_isDirty)
                UpdateVisual();
        }

        public void ChangeQuality(float newValue, bool integrateQuality = false)
        {
            if (!integrateQuality)
                Quality = newValue;
            else
                AddQualityMeasure(newValue);

            _isDirty = true;
        }

        public void ChangeVisibility(bool newValue)
        {
            IsVisible = newValue;
            Visual.SetActive(IsVisible);
            _isDirty = true;
        }

        private void UpdateVisual()
        {
            if (!IsVisible)
                return;

            Color = new Color(1.0f - Quality, 1.0f, 1.0f - Quality);
            Visual.GetComponent<Renderer>().material.color = Color;
            _isDirty = false;
        }

        private void AddQualityMeasure(float value)
        {
            Qualities.Enqueue(value);

            if (Qualities.Count > IntegrateQualityOverSteps)
            {
                var removedValue = Qualities.Dequeue();
                QualityCMA = QualityCMA + value / _currentIntegrationSteps - removedValue / _currentIntegrationSteps;
            }
            else
            {
                QualityCMA = (value + _currentIntegrationSteps * QualityCMA) / (_currentIntegrationSteps + 1);
                _currentIntegrationSteps++;
            }

            Quality = QualityCMA;
        }
    }
}