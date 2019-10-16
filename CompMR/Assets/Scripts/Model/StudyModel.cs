using System;
using System.Collections.Generic;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class StudyModel : MonoBehaviour
    {
        public int ParticipantID = 0;
        public float ConditionTimeMinutes = 4;
        public bool EnableStudy = false;
        public int CurrentConditionIndex = -1;
        public int CurrentQuestionIndex = -1;

        public ConditionModel CurrentCondition;

        public int QuestionDurationSecondsMin = 15;
        public int QuestionDurationSecondsMax = 25;

        public int NumQuestionsPerCondition = 9;

        public bool EnableBreak = true;
        public List<int> CurrentConditionQuestionIndices;

    }
}
