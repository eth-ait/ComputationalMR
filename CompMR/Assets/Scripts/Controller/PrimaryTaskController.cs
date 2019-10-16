using System;
using System.Collections.Generic;
using Assets.Scripts.Util;
using UnityEngine;
using Logger = Assets.Scripts.Util.Logger;

namespace Assets.Scripts.Controller
{
    public class PrimaryTaskController : MonoBehaviour
    {
        public List<GameObject> TaskContainers;
        public SoundTimer Sound;

        public int DelaySeconds = 15;
        public int TaskTimeSeconds = 3 * 60;

        public bool LoopTasks = false;
        public Logger Logger;

        private bool _started;
        private bool _tasksStarted = false;
        private int _currentTaskIndex = -1;

        private DateTime _initTime;
        private DateTime _currentTaskStartedTime;


        // Start is called before the first frame update
        void Start()
        {
            _initTime = DateTime.Now;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_tasksStarted)
            {
                var timeOffset = (DateTime.Now - _initTime).TotalSeconds;

                if (timeOffset >= DelaySeconds)
                {
                    Sound.enabled = true;
                    _tasksStarted = true;

                    StartNextTask();
                }
            }
            else
            {
                var timeOffset = (DateTime.Now - _currentTaskStartedTime).TotalSeconds;

                if (timeOffset >= TaskTimeSeconds)
                {
                    StartNextTask();
                }
            }
        }

        private void StartNextTask()
        {
            _currentTaskStartedTime = DateTime.Now;

            if (_currentTaskIndex > -1)
            {
                TaskContainers[_currentTaskIndex].SetActive(false);
            }

            _currentTaskIndex++;

            if (_currentTaskIndex >= TaskContainers.Count)
            {
                if (LoopTasks)
                {
                    _currentTaskIndex = 0;
                }
                else
                {
                    //done with tasks
                    Sound.enabled = false;
                    return;
                }
            }

            TaskContainers[_currentTaskIndex].SetActive(true);
        }
    }
}
