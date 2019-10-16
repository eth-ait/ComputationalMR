using System;
using System.Collections.Generic;
using Assets.Scripts.Event;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.UI;
using ElementModel = Assets.Scripts.Model.ElementModel;

namespace Assets.Scripts.View
{
    public class LodController : MonoBehaviour
    {
        public event EventHandler<StringParamsEventArgs> LODChangedManual;
        public event EventHandler<StringParamsEventArgs> ElementFixatedManual;

        public ElementModel Model;

        public Button DecreaseGameObject;
        public Button IncreaseGameObject;
        public Button FixateGameObject;

        private bool _fixateElementIsEnabled = false;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            IncreaseGameObject.interactable = Model.CurrentLevelOfDetail < Model.MaxLevelOfDetail;
            DecreaseGameObject.interactable = Model.CurrentLevelOfDetail > 1;

            if (Model.FixateElement != _fixateElementIsEnabled)
                SetFixateElement(Model.FixateElement);
        }

        public void IncreaseLOD()
        {
            if (Model != null && Model.CurrentLevelOfDetail < Model.MaxLevelOfDetail)
            {
                Model.SetCurrentLoD(Model.CurrentLevelOfDetail + 1, false);
                Model.TriggerTemporaryFixation();

                LODChangedManual?.Invoke(this, new StringParamsEventArgs("increase", Model.gameObject.name, Model.CurrentLevelOfDetail.ToString("D")));
            }
        }

        public void DecreaseLOD()
        {
            if (Model != null && Model.CurrentLevelOfDetail > 0)
            {
                Model.SetCurrentLoD(Model.CurrentLevelOfDetail - 1, false);
                Model.TriggerTemporaryFixation();

                LODChangedManual?.Invoke(this, new StringParamsEventArgs("decrease", Model.gameObject.name, Model.CurrentLevelOfDetail.ToString("D")));
            }
        }

        public void ToggleFixateElement()
        {
            Model.AbortTemporaryFixation();
            Model.FixateElement = !Model.FixateElement;

            ElementFixatedManual?.Invoke(this, new StringParamsEventArgs("fixate", Model.gameObject.name, Model.CurrentLevelOfDetail.ToString("D"), Model.FixateElement ? "TRUE" : "FALSE"));
        }

        public void SetFixateElement(bool value)
        {
            _fixateElementIsEnabled = value;

            var colors = FixateGameObject.colors;
            var normalColor = colors.normalColor;
            var highlightedColor = colors.highlightedColor;

            colors.normalColor = highlightedColor;
            colors.highlightedColor = normalColor;
            FixateGameObject.colors = colors;
        }
    }
}
