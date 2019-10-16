using System;
using System.Collections.Generic;
using Assets.Scripts.Event;
using Assets.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;
using Logger = Assets.Scripts.Util.Logger;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Controller
{
    public class SearchTaskController : MonoBehaviour, ITaskController
    {
        public event EventHandler<PrimaryTaskEventArgs> PrimaryTaskTrialCompleted;
        public event EventHandler<PrimaryTaskInteractionEventArgs> InteractionHappened;

        private bool _started;
        private List<int> _targetIconIndixes;

        public GameObject IconsContainer;
        public RawImage TargetIcon;

        public string IconsPath;
        private List<int> _distractorIconIndices;

        private bool _currentIconIsInSet;

        public GameObject IndicatorCorrect;
        public GameObject IndicatorFalse;

        public string TaskName;

        private int _trialCounter;
        private DateTime _trialStartTime;

        public bool RandomizeOnEveryTrial = true;


        // Start is called before the first frame update
        void Start()
        {
            if (!_started)
            {
                _started = true;

                BuildIcons();

                _currentIconIsInSet = Random.Range(0.0f, 1.0f) < 0.5f;
                SetTargetIcon();
            }
        }

        // Update is called once per frame
        void Update()
        {
            UnityUtil.FadeObject(IndicatorCorrect);
            UnityUtil.FadeObject(IndicatorFalse);
        }

        void OnEnable()
        {
            InteractionHappened?.Invoke(this, new PrimaryTaskInteractionEventArgs(TaskName, _trialCounter, "start"));
            _trialStartTime = DateTime.Now;
        }
        void OnDisable()
        {
            InteractionHappened?.Invoke(this, new PrimaryTaskInteractionEventArgs(TaskName, _trialCounter, "end"));
        }

        private void SetTargetIcon()
        {
            var newTargetIndex = 0;

            if (_currentIconIsInSet)
            {
                newTargetIndex = _targetIconIndixes[Random.Range(0, _targetIconIndixes.Count)];
            }
            else
            {
                newTargetIndex = _distractorIconIndices[Random.Range(0, _distractorIconIndices.Count)];
            }


            var path = IconsPath + (newTargetIndex + 1);
            TargetIcon.texture = Resources.Load(path) as Texture2D;

            InteractionHappened?.Invoke(this, new PrimaryTaskInteractionEventArgs(TaskName, _trialCounter, "set_target_icon", newTargetIndex.ToString("D"), path));
        }

        private void BuildIcons()
        {
            var allIcons = new List<int>();

            for (int i = 0; i < Constants.NUM_ICONS_IN_LIBRARY; i++)
            {
                allIcons.Add(i);
            }

            var rawImages = IconsContainer.GetComponentsInChildren<RawImage>();

            _targetIconIndixes = new List<int>();
            for (int i = 0; i < rawImages.Length; i++)
            {
                var randomIndex = Random.Range(0, allIcons.Count);
                _targetIconIndixes.Add(allIcons[randomIndex]);
                allIcons.RemoveAt(randomIndex);
            }

            //_targetIconIndixes = ListUtil.ShuffleList(_targetIconIndixes);

            for (int i = 0; i < rawImages.Length; i++)
            {
                var path = IconsPath + (_targetIconIndixes[i] + 1);
                rawImages[i].texture = Resources.Load(path) as Texture2D;
            }

            _distractorIconIndices = new List<int>(allIcons);

            InteractionHappened?.Invoke(this, new PrimaryTaskInteractionEventArgs(TaskName, _trialCounter, "build_icons", string.Join(",", _targetIconIndixes)));

            //for (int i = 0; i < Constants.NUM_ICONS_IN_LIBRARY; i++)
            //{
            //    if(!_targetIconIndixes.Contains(i))
            //        _distractorIconIndices.Add(i);
            //}

        }

        public void HandleConfirmInSet()
        {
            bool solutionIsCorrect = _currentIconIsInSet;
            HandleSolutionConfirmed(solutionIsCorrect);

            if (RandomizeOnEveryTrial)
            {
                BuildIcons();
            }

            _currentIconIsInSet = Random.Range(0.0f, 1.0f) < 0.5f;
            SetTargetIcon();
        }

        public void HandleConfirmNotInSet()
        {
            bool solutionIsCorrect = !_currentIconIsInSet;
            HandleSolutionConfirmed(solutionIsCorrect);

            if (RandomizeOnEveryTrial)
            {
                BuildIcons();
            }

            _currentIconIsInSet = Random.Range(0.0f, 1.0f) < 0.5f;
            SetTargetIcon();
        }

        private void HandleSolutionConfirmed(bool solutionIsCorrect)
        {
            if (solutionIsCorrect)
            {
                UnityUtil.EnableObject(IndicatorCorrect);
            }
            else
            {
                UnityUtil.EnableObject(IndicatorFalse);
            }

            var trialDuration = DateTime.Now - _trialStartTime;
            PrimaryTaskTrialCompleted?.Invoke(this, new PrimaryTaskEventArgs(TaskName, _trialCounter, solutionIsCorrect ? "TRUE" : "FALSE", "-1", trialDuration.TotalMilliseconds.ToString("F3")));


            _trialStartTime = DateTime.Now;
            _trialCounter++;
        }
    }
}
