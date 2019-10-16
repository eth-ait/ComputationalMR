using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Util
{
    public class ReadOnlyAttribute : PropertyAttribute
    {

    }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property,
            GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }

    public static class UnityUtil
    {
        public static Bounds GetMaxBounds(GameObject gameObject)
        {
            var boundingBox = new Bounds(gameObject.transform.position, Vector3.zero);

            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            {
                boundingBox.Encapsulate(renderer.bounds);
            }

            return boundingBox;
        }

        public static void EnableObject(GameObject target)
        {
            target.SetActive(true);

            var material = target.GetComponent<MeshRenderer>().material;
            var color = material.color;
            color.a = 1.0f;
            material.color = color;
            target.GetComponent<MeshRenderer>().material = material;
        }

        public static void FadeObject(GameObject target)
        {
            var material = target.GetComponent<MeshRenderer>().material;


            if (material.color.a > 0.01)
            {
                var color = material.color;
                color.a -= Time.deltaTime;
                material.color = color;
                target.GetComponent<MeshRenderer>().material = material;
            }
            else
            {
                target.SetActive(false);
            }
        }

        public static void SetAlphaRecursive(GameObject target, float alpha)
        {
            var renderers = target.GetComponentsInChildren<Renderer>();

            foreach (var renderer in renderers)
            {
                var material = renderer.material;

                if (!material.HasProperty("_Color"))
                    continue;

                var color = material.color;
                color.a = alpha;
                material.color = color;
                renderer.material = new Material(material);
            }

            var images = target.GetComponentsInChildren<RawImage>();

            foreach (var rawImage in images)
            {
                var color = rawImage.color;
                color.a = alpha;
                rawImage.color = color;
            }
        }

        public static void SetColor(GameObject target, float r, float g, float b, bool searchInChildren = false)
        {
            Renderer renderer;
             
            if (!searchInChildren)
                renderer = target.GetComponent<Renderer>();
            else
            {
                renderer = target.GetComponentInChildren<Renderer>();
            }

            if (renderer != null)
            {
                var material = renderer.material;

                if (!material.HasProperty("_Color"))
                    return;

                var color = material.color;
                color.r = r;
                color.g = g;
                color.b = b;
                material.color = color;
                renderer.material = new Material(material);
            }
        }
    }
}