using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Event;
using Assets.Scripts.Model;
using Assets.Scripts.Optimization;
using Assets.Scripts.Util;
using Assets.Scripts.View;
using UnityEngine;
using Logger = Assets.Scripts.Util.Logger;

namespace Assets.Scripts.Controller
{
    public class LoggingController : MonoBehaviour
    {
        public GazeHandler GazeHandler;

        public CountingTaskController TaskHighC17;
        public SearchTaskController TaskMidSearch;
        public CountingTaskController TaskLowC2;

        public Logger IPALogger;
        public Logger PrimaryTaskLogger;
        public Logger SecondaryTaskLogger;
        public Logger DebugLogger;
        public Logger OptimizationLogger;
        public Logger InteractionLogger;

        public StudyController StudyController;
        public StudyModel StudyModel;

        public AppController AppController;

        private int _secondaryInteractionCount;

        void Start()
        {
            TaskHighC17.InteractionHappened += OnPrimaryTaskInteractionHappened;
            TaskHighC17.PrimaryTaskTrialCompleted += OnPrimarytaskTrialCompleted;

            TaskMidSearch.InteractionHappened += OnPrimaryTaskInteractionHappened;
            TaskMidSearch.PrimaryTaskTrialCompleted += OnPrimarytaskTrialCompleted;

            TaskLowC2.InteractionHappened += OnPrimaryTaskInteractionHappened;
            TaskLowC2.PrimaryTaskTrialCompleted += OnPrimarytaskTrialCompleted;

            GazeHandler.NewIPAValueAvailable += OnNewIPAValueAvailable;

            StudyController.StudyConditionStarted += OnStudyConditionStarted;
            StudyController.StudyConditionStopped += OnStudyConditionStopped;
            StudyController.StudyEventQuestionAsked += OnStudyQuestionAsked;
            StudyController.StudyMiscEvent += OnStudyMiscEvent;

            AppController.OnOptimizationCompleted += OnOptimizationComplete;

            SetLoggerDefaultPrefixes(IPALogger);
            SetLoggerDefaultPrefixes(PrimaryTaskLogger);
            SetLoggerDefaultPrefixes(SecondaryTaskLogger);
            SetLoggerDefaultPrefixes(DebugLogger);
            SetLoggerDefaultPrefixes(OptimizationLogger);
            SetLoggerDefaultPrefixes(InteractionLogger);

            foreach (var appModelElement in AppController.AppModel.Elements)
            {
                var lodController = appModelElement.LodControllerGameObject.GetComponent<LodController>();

                lodController.LODChangedManual += OnLODChangedManual;
                lodController.ElementFixatedManual += OnFixateManual;
            }

        }

        void Update()
        {

        }

        private void SetLoggerDefaultPrefixes(Logger logger)
        {
            logger.SetPrefix(LogPrefix.PARTICIPANT, StudyModel.ParticipantID);
            logger.SetPrefix(LogPrefix.CONDITION, -1);
            logger.SetPrefix(LogPrefix.METHOD, "none");
            logger.SetPrefix(LogPrefix.TRIAL, -1);
            logger.SetPrefix(LogPrefix.PRIMARY_TASK, "none");
            logger.SetPrefix(LogPrefix.SECONDARY_TASK, "none");
            logger.SetPrefix(LogPrefix.ENABLED, "FALSE");
        }

        private void OnStudyQuestionAsked(object sender, EventArgs e)
        {
            if (StudyModel.CurrentQuestionIndex > 0)
            {
                var lastQuestionsIndex = StudyModel.CurrentConditionQuestionIndices[StudyModel.CurrentQuestionIndex - 1];
                var lastQuestion = StudyQuestionnaire.Questions[lastQuestionsIndex];
                SecondaryTaskLogger.Log(StudyModel.CurrentQuestionIndex.ToString("D"), _secondaryInteractionCount.ToString("D"), lastQuestion.TargetApplication, lastQuestion.QuestionString, lastQuestion.AnswerLOD.ToString("D"));
                DebugLogger.Log(StudyModel.CurrentQuestionIndex.ToString("D"), _secondaryInteractionCount.ToString("D"), lastQuestion.TargetApplication, lastQuestion.QuestionString, lastQuestion.AnswerLOD.ToString("D"));
            }

            var currentQuestionsIndex = StudyModel.CurrentConditionQuestionIndices[StudyModel.CurrentQuestionIndex];
            var currentQuestion = StudyQuestionnaire.Questions[currentQuestionsIndex];
            DebugLogger.Log(StudyModel.CurrentQuestionIndex.ToString("D"), currentQuestion.TargetApplication, currentQuestion.QuestionString, currentQuestion.AnswerLOD.ToString("D"));

            _secondaryInteractionCount = 0;
        }

        private void OnStudyConditionStarted(object sender, EventArgs e)
        {
            SetPrefixForLoggers(LogPrefix.CONDITION, StudyModel.CurrentConditionIndex);
            SetPrefixForLoggers(LogPrefix.METHOD, StudyModel.CurrentCondition.Method == MethodType.Automatic ? "Automatic" : "Manuel");
            SetPrefixForLoggers(LogPrefix.PRIMARY_TASK, StudyModel.CurrentCondition.PrimaryTask.name);
            SetPrefixForLoggers(LogPrefix.SECONDARY_TASK, StudyModel.CurrentCondition.SecondayTask.Name);
            SetPrefixForLoggers(LogPrefix.ENABLED, "TRUE");

            _secondaryInteractionCount = 0;
        }

        private void OnStudyConditionStopped(object sender, EventArgs e)
        {
            SetPrefixForLoggers(LogPrefix.ENABLED, "FALSE");
        }

        private void OnStudyMiscEvent(object sender, StudyGeneralEventArgs e)
        {
            DebugLogger.Log(e.Parameters);
        }

        private void OnNewIPAValueAvailable(object sender, GazeDataEventArgs e)
        {
            IPALogger.Log(e.RawIPA.ToString("F4"), e.NormalizedIPA.ToString("F4"), e.SmoothIPA.ToString("F4"));

            //Do not use debug logger here because of multi-threading.
            //DebugLogger.Log(e.RawIPA.ToString("F4"), e.NormalizedIPA.ToString("F4"), e.SmoothIPA.ToString("F4"));
        }

        private void OnPrimarytaskTrialCompleted(object sender, PrimaryTaskEventArgs e)
        {
            SetPrefixForLoggers(LogPrefix.TRIAL, e.TrialCounter);
            
            PrimaryTaskLogger.Log(e.Parameters);
            DebugLogger.Log(e.Parameters);
        }

        private void OnPrimaryTaskInteractionHappened(object sender, PrimaryTaskInteractionEventArgs e)
        {
            DebugLogger.SetPrefix(LogPrefix.PRIMARY_TASK, e.TaskName);
            DebugLogger.SetPrefix(LogPrefix.TRIAL, e.TrialCounter);
            
            DebugLogger.Log(e.Parameters);
        }

        private void OnOptimizationComplete(object sender, OptimizationCompletedEventArgs e)
        {
            var logArgumentsOptimizer = new[]{ "optimizationCompleted", 
                                                e.Duration.TotalMilliseconds.ToString("F3"), 
                                                e.Model.Solutions.Count.ToString("D"), 
                                                e.Model.IsFeasible ? "TRUE": "FALSE",
                                                e.Model.UserModel.CognitiveCapacity.ToString("F3"),
                                                e.Model.UserModel.CognitiveLoad.ToString("F3"),
                                                e.Model.NumMinPlacementSlots.ToString("D"),
                                                e.Model.NumMaxPlacementSlots.ToString("D"),
                                                e.Model.VisibilityReward.ToString("F3"), 
                                                e.Model.WeightImportance.ToString("F3"), 
                                                e.Model.WeightUtility.ToString("F3")};

            OptimizationLogger.Log(logArgumentsOptimizer);
            DebugLogger.Log(logArgumentsOptimizer);

            for (var solutionIndex = 0; solutionIndex < e.Model.Solutions.Count; solutionIndex++)
            {
                var solution = e.Model.Solutions[solutionIndex];
                var logArguments = new List<string>
                {
                    solutionIndex.ToString("D"),
                    solution.Objective.ToString("F3")
                };

                for (var solutionElementIndex = 0; solutionElementIndex < solution.SolutionElements.Count; solutionElementIndex++)
                {
                    var solutionSolutionElement = solution.SolutionElements[solutionElementIndex];
                    var appElement = AppController.AppModel.Elements[solutionElementIndex];

                    logArguments.Add(appElement.ElementIsInView ? "InView" : "NotInView");

                    foreach (var key in solutionSolutionElement.LodAndVisiblityDictionary.Keys)
                    {
                        var lod = key;
                        var visibility = solutionSolutionElement.LodAndVisiblityDictionary[key];
                        logArguments.Add(lod.ToString("D"));
                        logArguments.Add(visibility.ToString("F1"));
                    }
                }

                OptimizationLogger.Log(logArguments.ToArray());
                DebugLogger.Log(logArguments.ToArray());
            }
        }

        private void OnFixateManual(object sender, StringParamsEventArgs e)
        {
            DebugLogger.Log(e.Parameters);
            InteractionLogger.Log(e.Parameters);
        }

        private void OnLODChangedManual(object sender, StringParamsEventArgs e)
        {
            DebugLogger.Log(e.Parameters);
            InteractionLogger.Log(e.Parameters);

            _secondaryInteractionCount++;
        }

        private void SetPrefixForLoggers(string key, int value)
        {
            IPALogger.SetPrefix(key, value);
            PrimaryTaskLogger.SetPrefix(key, value);
            SecondaryTaskLogger.SetPrefix(key, value);
            OptimizationLogger.SetPrefix(key, value);
            DebugLogger.SetPrefix(key, value);
            InteractionLogger.SetPrefix(key, value);
        }

        private void SetPrefixForLoggers(string key, string value)
        {
            IPALogger.SetPrefix(key, value);
            PrimaryTaskLogger.SetPrefix(key, value);
            SecondaryTaskLogger.SetPrefix(key, value);
            OptimizationLogger.SetPrefix(key, value);
            DebugLogger.SetPrefix(key, value);
            InteractionLogger.SetPrefix(key, value);
        }
    }
}