using System;
using System.Runtime.CompilerServices;
using Assets.Scripts.Model;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class FakeGazeHandler : MonoBehaviour
    {
        public AppModel AppModel;

        public GameObject DebugVis;
        public TextMesh DebugTextMesh;
        public string IPAStringFormat = "F2";

        public float IPA;

        public float UpdateFrequency = 1.0f;
        public float UpdateStep = .1f;
        public float IPAMin = .2f;
        public float IPAMax = .85f;

        private DateTime _lastUpdateTime;

        private bool _started;
        private float _runner;

        void Update()
        {
            if (!_started)
            {
                _started = true;
                _lastUpdateTime = DateTime.MinValue;
            }

            if (ShouldReceiveNewValue())
            {
                _lastUpdateTime = DateTime.Now;

                SetNewValue();

                if (DebugVis != null)
                {
                    DebugVis.transform.localScale = new Vector3(1, IPA, 1);
                    UnityUtil.SetColor(DebugVis, IPA, 1.0f - IPA, .2f, true);
                }

                if (DebugTextMesh != null)
                    DebugTextMesh.text = IPA.ToString(IPAStringFormat);

                if (AppModel != null)
                    AppModel.User.CognitiveCapacity = 1.0f - IPA;
            }
        }

        private bool ShouldReceiveNewValue()
        {
            var lastUpdateDistance = DateTime.Now - _lastUpdateTime;
            return lastUpdateDistance.TotalSeconds > UpdateFrequency;
        }
        
        private void SetNewValue()
        {
            IPA = Math.Min(Math.Max(IPAMin, Mathf.Abs(Mathf.Sin(_runner))), IPAMax);
            _runner += UpdateStep;
        }
    }
}