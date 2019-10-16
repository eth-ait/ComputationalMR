using System;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class StateController : MonoBehaviour
    {
        private Dictionary<ElementModel, bool> State;

        public AppModel AppModel;
        public AppController AppController;

        private bool _started;

        public float DelaySeconds = 3.0f;

        private bool _shouldTriggerStateChange;
        private DateTime StateChangeTriggered;

        void Update()
        {
            if (!_started)
            {
                _started = true;
                AppController.OnOptimizationCompleted += OnOptimizationCompleted;

                State = new Dictionary<ElementModel, bool>();
            }

            if (!AppModel.OptimizationModel.AutoOptimizeEnabled)
                return;

            CheckForStateChange();

            if (_shouldTriggerStateChange)
            {
                var duration = (DateTime.Now - StateChangeTriggered).TotalSeconds;

                if (duration > DelaySeconds)
                {
                    Debug.Log("StateManagerTriggered");
                    AppController.StartOptimization();
                    _shouldTriggerStateChange = false;
                }
            }
            else
            {
                StateChangeTriggered = DateTime.Now;
            }
        }

        private void OnOptimizationCompleted(object sender, EventArgs e)
        {
            PreserveState();
        }

        private void PreserveState()
        {
            State.Clear();

            foreach (var element in AppModel.ViewElements)
            {
                if (element != null)
                    State.Add(element, element.ElementIsInView);
            }
        }

        private void CheckForStateChange()
        {
            var stateHasChanged = false;

            foreach (var element in AppModel.ViewElements)
            {
                if (element && State.ContainsKey(element) && element.ElementIsInView != State[element])
                    stateHasChanged = true;
            }


            if (stateHasChanged)
            {
                if (!_shouldTriggerStateChange)
                {
                    _shouldTriggerStateChange = true;
                    StateChangeTriggered = DateTime.Now;
                }
            }
            else
            {
                _shouldTriggerStateChange = false;
            }
        }
    }
}