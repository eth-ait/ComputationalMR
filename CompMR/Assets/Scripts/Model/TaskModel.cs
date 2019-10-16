using System;
using System.Collections.Generic;
using Assets.Scripts.Controller;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class TaskModel : MonoBehaviour
    {
        public AppModel AppModel;
        public string Name;
        public List<TaskAppModel> AppImportanceDictionary;
    }
}
