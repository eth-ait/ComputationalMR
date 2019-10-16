using Assets.Scripts.Util;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class UserModel : MonoBehaviour
    {
        [Util.ReadOnly] public float CognitiveCapacity = Constants.INITIAL_COGNITIVE_CAPACITY;
        [Util.ReadOnly] public float CognitiveLoad;
    }
}