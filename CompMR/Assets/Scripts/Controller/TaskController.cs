using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class TaskController : MonoBehaviour
    {
        private bool _started;

        public List<TaskModel> Tasks;

        void Start()
        {
        
        }

        void Update()
        {
            if (!_started)
            {
                _started = true;
            }        
        }
    }
}
