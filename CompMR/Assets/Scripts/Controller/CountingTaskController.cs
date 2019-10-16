using System;
using Assets.Scripts.Event;
using Assets.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;
using Logger = Assets.Scripts.Util.Logger;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Controller
{
    public class CountingTaskController : MonoBehaviour, ITaskController
    {
        public event EventHandler<PrimaryTaskEventArgs> PrimaryTaskTrialCompleted;
        public event EventHandler<PrimaryTaskInteractionEventArgs> InteractionHappened;

        private int _startNumber;
        public int Decrement = 17;

        public TextMesh CurrentNumberText;
        public TextMesh NextNumberText;

        public GameObject IndicatorCorrect;
        public GameObject IndicatorFalse;

        public string TaskName;
        private int _trialCounter;
        private int _numErrorsInTrial;

        private DateTime _trialStartTime;

        // Start is called before the first frame update
        void Start()
        {
            _startNumber = Random.Range(700, 900);
            CurrentNumberText.text = _startNumber.ToString("D");
        }

        // Update is called once per frame
        void Update()
        {
            UnityUtil.FadeObject(IndicatorCorrect);
            UnityUtil.FadeObject(IndicatorFalse);
        }

        void OnDisable()
        {
            InteractionHappened?.Invoke(this, new PrimaryTaskInteractionEventArgs(TaskName, _trialCounter, "end", NextNumberText.text, CurrentNumberText.text));
        }

        void OnEnable()
        {
            _startNumber = Random.Range(700, 900);
            CurrentNumberText.text = _startNumber.ToString("D");

            NextNumberText.text = string.Empty;

            InteractionHappened?.Invoke(this, new PrimaryTaskInteractionEventArgs(TaskName, _trialCounter, "start", NextNumberText.text, CurrentNumberText.text));

            _trialStartTime = DateTime.Now;
        }

        public void HandleNumberInput(GameObject source)
        {
            var text = source.GetComponentInChildren<Text>();
            NextNumberText.text += text.text;
            InteractionHappened?.Invoke(this, new PrimaryTaskInteractionEventArgs(TaskName, _trialCounter, "input", text.text));
        }

        public void HandleConfirm()
        {
            int current;
            if (!int.TryParse(CurrentNumberText.text, out current))
                return;

            int next;
            if (!int.TryParse(NextNumberText.text, out next))
                return;
        
            var offset = current - next;

            if (offset == Decrement)
            {
                CurrentNumberText.text = NextNumberText.text;
                NextNumberText.text = string.Empty;

                UnityUtil.EnableObject(IndicatorCorrect);

                var trialDuration = DateTime.Now - _trialStartTime;
                PrimaryTaskTrialCompleted?.Invoke(this, new PrimaryTaskEventArgs(TaskName, _trialCounter, "TRUE",
                                                                                    _numErrorsInTrial.ToString("D"), trialDuration.TotalMilliseconds.ToString("F1"),
                                                                                    next.ToString("D"), current.ToString("D")));

                _trialCounter++;
                _numErrorsInTrial = 0;
                _trialStartTime = DateTime.Now;
            }
            else
            {
                PrimaryTaskTrialCompleted?.Invoke(this, new PrimaryTaskEventArgs(TaskName, _trialCounter, "FALSE", next.ToString("D"), current.ToString("D")));

                _numErrorsInTrial++;

                UnityUtil.EnableObject(IndicatorFalse);
                NextNumberText.text = string.Empty;
            }
        }

        public void HandleDelete()
        {
            InteractionHappened?.Invoke(this, new PrimaryTaskInteractionEventArgs(TaskName, _trialCounter, "delete", NextNumberText.text, CurrentNumberText.text, _trialCounter.ToString("D"), _numErrorsInTrial.ToString("D")));

            NextNumberText.text = string.Empty;
        }
    }
}
