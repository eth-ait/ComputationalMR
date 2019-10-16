using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.View
{
    [CustomEditor(typeof(UserModel))]
    public class UserEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UserModel user = (UserModel) target;


            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                contentOffset = new Vector2(0, 18)
            };
            GUILayout.Label("Cognitive Capacity", labelStyle);


            GUIStyle sliderStyle = new GUIStyle(GUI.skin.horizontalSlider)
            {
                margin = { left = (int)(EditorGUIUtility.labelWidth + 15)},
                alignment = TextAnchor.MiddleRight,
            };
            user.CognitiveCapacity = GUILayout.HorizontalSlider(user.CognitiveCapacity, 0.0f, 1.0f, sliderStyle, GUI.skin.horizontalSliderThumb);
        }
    }
}