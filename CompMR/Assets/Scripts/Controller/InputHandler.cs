using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Controller
{
    public class InputHandler : MonoBehaviour
    {
        public GameObject CameraGameObject;
        public List<Hand> Hands;

        public GameObject Menu;

        void Start()
        {
            foreach (var hand in Hands)
            {
                SteamVR_Input._default.inActions.CompMRMenu.AddOnChangeListener(OnChange, hand.handType);
            }
        }

        void Update()
        {
            if (Menu.activeSelf)
            {
            }
        }
        public void HandleButton()
        {
            Debug.Log("Button enabled");
        }


        private void OnChange(SteamVR_Action_In source)
        {
            foreach (var hand in Hands)
            {
                if (source.GetChanged(hand.handType))
                {
                    ToggleMenu(hand.transform);
                }
            }
        }

        private void ToggleMenu(Transform targetTransform)
        {
            if (!Menu.activeSelf)
            {
                Debug.Log("showing menu");
                Menu.SetActive(true);
                Menu.transform.position = targetTransform.position;

                RotateTowardsHead();
            }
            else if (Menu.activeSelf)
            {
                Debug.Log("hiding menu");
                Menu.SetActive(false);
            }
        }


        private void RotateTowardsHead()
        {
            var direction = (CameraGameObject.transform.position - Menu.transform.position).normalized;
            Menu.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}