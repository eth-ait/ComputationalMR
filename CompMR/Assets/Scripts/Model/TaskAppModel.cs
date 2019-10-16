using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class TaskAppModel
    {
        public ElementModel Application;

        [Range(1, Constants.MAX_IMPORTANCE)]
        public int Importance = 1;  
    }
}