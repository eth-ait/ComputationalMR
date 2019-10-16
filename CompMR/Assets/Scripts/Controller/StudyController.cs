using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Event;
using Assets.Scripts.Model;
using Assets.Scripts.Util;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Controller
{
    public class StudyController : MonoBehaviour
    {
        public AppController AppController;
        public StudyModel Model;

        public ConditionFactory ConditionFactory;

        public GameObject BreakObject;

        public event EventHandler<EventArgs> StudyConditionStarted;
        public event EventHandler<EventArgs> StudyConditionStopped;

        public event EventHandler<EventArgs> StudyEventQuestionAsked;

        public event EventHandler<StudyGeneralEventArgs> StudyMiscEvent;

        private TTSUtil _ttsUtil;
        private bool _started;

        private DateTime _conditionStartTime;
        
        private Dictionary<IndexList, List<List<int>>> _questionsPerTask;

        private DateTime _lastQuestionTime;
        private int _nextQuestionDuration;

        public int ConditionDelaySeconds = 5;

        private bool _conditionStartDelayTriggered;
        private DateTime _conditionStartDelayTriggeredTime;

        private List<ConditionModelTemplate> _conditionModelTemplates;

        private bool _overrideStartNextCondition;

        void Start()
        {
            _ttsUtil = TTSUtil.GetInstance();
            _conditionModelTemplates = ConditionBuilderUtil.GetConditionsForParticipant(Model.ParticipantID);
            SetupQuestionIndices();
        }

        public void StartStudy()
        {
            Model.EnableStudy = true;
        }

        public void HandleStartNextConditionButton()
        {
            BreakObject.SetActive(false);
            BreakObject.transform.parent = null;

            StartNextCondition();
        }

        public void OverrideStartNextCondition()
        {
            _overrideStartNextCondition = true;
        }

        void Update()
        {
            if (!Model.EnableStudy)
                return;

            if (!_started)
            {
                ConditionFactory.PrimaryTaskHigh.SetActive(false);
                ConditionFactory.PrimaryTaskMid.SetActive(false);
                ConditionFactory.PrimaryTaskLow.SetActive(false);
                ConditionFactory.PrimaryTaskTraining.SetActive(false);

                Model.CurrentConditionIndex = 4;
                Model.CurrentQuestionIndex = -1;

                _started = true;
                SetupQuestionIndices();

                //foreach (var condition in Model.Conditions)
                //{
                //    condition.StopCondition();
                //}

                Model.CurrentCondition.StopCondition();
                NextCondition();
            }
            else
            {
                if (_conditionStartDelayTriggered)
                {
                    var duration = DateTime.Now - _conditionStartDelayTriggeredTime;

                    if (duration.TotalSeconds >= ConditionDelaySeconds)
                    {
                        StudyConditionStopped?.Invoke(this, null);
                        _conditionStartDelayTriggered = false;

                        BreakObject.transform.parent = Model.CurrentCondition.PrimaryTaskContainer.transform;
                        BreakObject.transform.localPosition = Vector3.zero;
                        BreakObject.transform.localRotation = Quaternion.identity;

                        BreakObject.SetActive(true);

                        Model.EnableStudy = false;

                        _ttsUtil.Speak("The next condition is " + Model.CurrentCondition.ConditionLocation + ".");
                    }
                }
                else
                {
                    if (_overrideStartNextCondition)
                    {
                        _overrideStartNextCondition = false;
                        NextCondition();

                        return;
                    }

                    var conditionDuration = DateTime.Now - _conditionStartTime;

                    if (!(conditionDuration.TotalMinutes > Model.ConditionTimeMinutes))
                    {
                        var lastQuestionDuration = DateTime.Now - _lastQuestionTime;

                        if (lastQuestionDuration.TotalSeconds > _nextQuestionDuration)
                        {
                            NextQuestion();
                        }
                    }
                    else
                    {
                        NextCondition();
                    }
                }
            }
        }

        private List<int> GetCurrentQuestionIndices()
        {
            if (Model.CurrentConditionIndex < 3)
            {
                return _questionsPerTask[Model.CurrentCondition.AllQuestionIndices][0];
            }
            else
            {
                return _questionsPerTask[Model.CurrentCondition.AllQuestionIndices][1];
            }
        }

        private void SetupQuestionIndices()
        {
            _questionsPerTask = new Dictionary<IndexList, List<List<int>>>();
            AddQuestionIndicesForTask(ConditionFactory.QuestionsEmailAll);
            AddQuestionIndicesForTask(ConditionFactory.QuestionsBreakAll);
            AddQuestionIndicesForTask(ConditionFactory.QuestionsIdeationAll);
        }

        private void AddQuestionIndicesForTask(IndexList task)
        {
            var parts = 2;

            var lists = new List<List<int>>();
            for (int i = 0; i < 2; i++)
            {
                lists.Add(new List<int>());
            }

            _questionsPerTask.Add(task, lists);

            var allQuestionIndices = new List<int>(task.Indices);
            for (int partIndex = 0; partIndex < 2; partIndex++)
            {
                for (int i = 0; i < Model.NumQuestionsPerCondition; i++)
                {
                    var randomIndex = UnityEngine.Random.Range(0, allQuestionIndices.Count);

                    var questionIndex = allQuestionIndices[randomIndex];
                    allQuestionIndices.RemoveAt(randomIndex);
                    _questionsPerTask[task][partIndex].Add(questionIndex);
                }
            }
        }

        private void NextQuestion()
        {
            Model.CurrentQuestionIndex++;

            if (Model.CurrentQuestionIndex < Model.NumQuestionsPerCondition)
            {
                var conditionQuestionIndex = Model.CurrentConditionQuestionIndices[Model.CurrentQuestionIndex];
                var question = StudyQuestionnaire.Questions[conditionQuestionIndex];

                Console.Write(Model.CurrentQuestionIndex + " / " +question.QuestionString);

                var fullQuestion = question.TargetApplication + ": " + question.QuestionString;
                _ttsUtil.Speak(fullQuestion);

                StudyEventQuestionAsked?.Invoke(this, null);
            }

            _lastQuestionTime = DateTime.Now;
            _nextQuestionDuration = UnityEngine.Random.Range(Model.QuestionDurationSecondsMin, Model.QuestionDurationSecondsMax);
        }

        private void NextCondition()
        {
            Model.CurrentCondition.StopCondition();

            Model.CurrentConditionIndex++;
            Model.CurrentQuestionIndex = -1;

            if (Model.CurrentConditionIndex >= _conditionModelTemplates.Count)
            {
                //end study
                Model.EnableStudy = false;
                Model.CurrentConditionIndex = -1;
            }
            else
            {
                ConditionFactory.BuildCondition(_conditionModelTemplates[Model.CurrentConditionIndex], Model.CurrentCondition, Model);
                Model.CurrentConditionQuestionIndices = GetCurrentQuestionIndices();

                AppController.AppModel.OptimizationModel.AutoOptimizeEnabled = false;
                AppController.AppModel.EnableAutoPlacement = false;
                AppController.ResetSolution();
                AppController.UpdateModelForTask(Model.CurrentCondition.SecondayTask);

                if (!Model.EnableBreak)
                {
                    StartNextCondition();
                }
                else
                {
                    StartNextConditionDelayed();
                }
            }
        }

        private void StartNextConditionDelayed()
        {
            _conditionStartDelayTriggered = true;
            _conditionStartDelayTriggeredTime = DateTime.Now;
        }

        private void StartNextCondition()
        {
            if (Model.CurrentConditionIndex == 3)
            {
                foreach (var appModelElement in AppController.AppModel.Elements)
                {
                    appModelElement.SwapElements();
                }
            }

            StudyConditionStarted?.Invoke(this, null);

            Model.CurrentCondition.StartCondition(Model.NumQuestionsPerCondition);

            AppController.AppModel.OptimizationModel.AutoOptimizeEnabled = Model.CurrentCondition.Method == MethodType.Automatic;
            AppController.AppModel.EnableAutoPlacement = Model.CurrentCondition.Method == MethodType.Manual;

            if (AppController.AppModel.OptimizationModel.AutoOptimizeEnabled)
                AppController.StartOptimization();

            _conditionStartTime = DateTime.Now;

            _lastQuestionTime = DateTime.Now;
            _nextQuestionDuration = UnityEngine.Random.Range(Model.QuestionDurationSecondsMin, Model.QuestionDurationSecondsMax);

            Model.EnableStudy = true;
        }
    }
}
