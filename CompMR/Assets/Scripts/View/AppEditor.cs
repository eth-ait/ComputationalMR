using Assets.Scripts.Controller;
using Assets.Scripts.Model;
using Assets.Scripts.Util;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.View
{
    [CustomEditor(typeof(AppController))]
    public class AppEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AppController appController = (AppController)target;

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                margin = { left = (int)(EditorGUIUtility.labelWidth + 15) },
            };


            if (GUILayout.Button("Optimize", buttonStyle, GUILayout.Width(100)))
            {
                appController.StartOptimization();
            }

            if (GUILayout.Button("Reset", buttonStyle, GUILayout.Width(100)))
            {
                appController.ResetSolution();
            }

            if (GUILayout.Button("Init Fake Model", buttonStyle, GUILayout.Width(100)))
            {
                appController.InitFakeViewModel();
            }

            if (GUILayout.Button("Eval opti", buttonStyle, GUILayout.Width(100)))
            {
                appController.EvalOptimization();
            }

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                contentOffset = new Vector2(0, 18)
            };
            GUILayout.Label("Selected Solution", labelStyle);


            GUIStyle sliderStyle = new GUIStyle(GUI.skin.horizontalSlider)
            {
                margin = { left = (int)(EditorGUIUtility.labelWidth + 15)},
                alignment = TextAnchor.MiddleRight,
            };

            var newSelectedSolution = Mathf.RoundToInt(GUILayout.HorizontalSlider(appController.AppModel.SelectedSolution, 0, appController.AppModel.CurrentNumSolutions - 1, sliderStyle, GUI.skin.horizontalSliderThumb));

            if(!CalcUtil.AreEqual(appController.AppModel.SelectedSolution, newSelectedSolution))
            {
                appController.SelectSolution(newSelectedSolution);
            }
        }
    }
}