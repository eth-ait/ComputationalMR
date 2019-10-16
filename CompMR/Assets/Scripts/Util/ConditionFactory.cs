using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public class ConditionFactory : MonoBehaviour
    {
        public GameObject PrimaryTaskHigh;
        public GameObject PrimaryTaskMid;
        public GameObject PrimaryTaskLow;
        public GameObject PrimaryTaskTraining;

        public GameObject PrimaryTaskContainerLeft;
        public GameObject PrimaryTaskContainerRight;
        public GameObject PrimaryTaskContainerCenter;

        public TaskModel SecondaryTaskModelEmail;
        public TaskModel SecondaryTaskModelBreak;
        public TaskModel SecondaryTaskModelIdeation;

        public IndexList QuestionsEmailAll;
        public IndexList QuestionsBreakAll;
        public IndexList QuestionsIdeationAll;

        public void BuildCondition(ConditionModelTemplate template, ConditionModel model, StudyModel studyModel)
        {
            model.Method = template.Method;

            switch (template.Position)
            {
                case ConditionPosition.Left:
                    model.PrimaryTaskContainer = PrimaryTaskContainerLeft;
                    break;
                case ConditionPosition.Right:
                    model.PrimaryTaskContainer = PrimaryTaskContainerRight;
                    break;
                case ConditionPosition.Center:
                    model.PrimaryTaskContainer = PrimaryTaskContainerCenter;
                    break;
            }

            switch (template.PrimaryTask)
            {
                case PrimaryTaskType.High:
                    model.PrimaryTask = PrimaryTaskHigh;
                    model.ConditionLocation = "count down with 17.";
                    break;
                case PrimaryTaskType.Mid:
                    model.PrimaryTask = PrimaryTaskMid;
                    model.ConditionLocation = "icon search.";
                    break;
                case PrimaryTaskType.Low:
                    model.PrimaryTask = PrimaryTaskLow;
                    model.ConditionLocation = "count down with 2.";
                    break;
            }

            switch (template.SecondaryTask)
            {
                case SecondaryTaskType.Email:
                    model.SecondayTask = SecondaryTaskModelEmail;
                    model.AllQuestionIndices = QuestionsEmailAll;
                    break;
                case SecondaryTaskType.Break:
                    model.SecondayTask = SecondaryTaskModelBreak;
                    model.AllQuestionIndices = QuestionsBreakAll;
                    break;
                case SecondaryTaskType.Ideation:
                    model.SecondayTask = SecondaryTaskModelIdeation;
                    model.AllQuestionIndices = QuestionsIdeationAll;
                    break;
            }
        }
    }
}