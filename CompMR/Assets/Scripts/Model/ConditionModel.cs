using UnityEngine;

namespace Assets.Scripts.Model
{

    [System.Serializable]
    public class ConditionModel : MonoBehaviour
    {
        public string name;

        public string ConditionLocation;
        public GameObject PrimaryTaskContainer;
        public GameObject PrimaryTask;
        public TaskModel SecondayTask;
        public IndexList AllQuestionIndices;
        //public IndexList SelectionQuestionIndices;
        public MethodType Method;

        private int _currentQuestion;


        public void StopCondition()
        {
            ChangeStatus(false, null);
        }

        public void StartCondition(int numQuestions)
        {
            ChangeStatus(true, PrimaryTaskContainer.transform);
        }

        private void ChangeStatus(bool enabled, Transform parent)
        {
            if (PrimaryTask != null)
            {
                PrimaryTask.transform.parent = parent;
                PrimaryTask.transform.localPosition = Vector3.zero;
                PrimaryTask.transform.localRotation = Quaternion.identity;

                PrimaryTask.SetActive(enabled);
            }
        }
    }
}