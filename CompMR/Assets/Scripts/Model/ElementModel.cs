using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Models;
using Assets.Scripts.Util;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Assets.Scripts.Util;
using Action = Assets.Models.Action;

namespace Assets.Scripts.Model
{
    public class ElementModel : MonoBehaviour
    {
        public List<GameObject> LODElements;
        public List<GameObject> LODElementsSwapped;

        private List<float> _currentAlphas;

        public int Identifier;
        public GameObject Headset;

        public GameObject RightHand;
        public GameObject LeftHand;

        public bool DebugMode;
        public bool RotateTowardsUser = true;

        public GameObject LodControllerGameObject;

        private List<Action> _actionQueue;

        public int MaxLevelOfDetail
        {
            get
            {
                if (LODElements != null)
                    return LODElements.Count;

                return 0;
            }
        }

        public int CurrentLevelOfDetail = 1;

        public float Visibility;

        [Range(1, Constants.MAX_IMPORTANCE)]
        public int Importance;

        [Range(1, Constants.MAX_COGNITIVE_LOAD)]
        public int MaxCognitiveLoad;

        public float FadeFactor = 1.0f;

        public bool FixateElement;
        public bool OmitElement { get; set; }

        public int LastIndex { get; set; }
        public int TimeVisible { get; set; }

        public float VisibilityAngleDegree = 30;
        public bool ShouldPlaceInView
        {
            //get => CurrentLevelOfDetail == 1;
            get => !ElementIsInView;
        }

        public int MaxViewLevelOfDetail = 2;
        public int MinViewLevelOfDetail = 2;

        public float ImportanceNorm => (float)Importance / Constants.MAX_IMPORTANCE;
        public float MaxCognitiveLoadNorm => (float)MaxCognitiveLoad / Constants.MAX_COGNITIVE_LOAD;

        [Util.ReadOnly] public bool ElementIsInView;
        [Util.ReadOnly] public float CurrentUtility;
        [Util.ReadOnly] public float CurrentCognitiveLoad;
        [Util.ReadOnly] public float CurrentObjective;

        private Vector3 _positionInSpace;
        private Quaternion _rotationInSpace;
        private Transform _parentInSpace;
        private Vector3 _scaleInSpace;
        private BoxCollider[] _disabledColliders;

        [Util.ReadOnly] public bool PlacedViewAnchored;
        private PlacementSlotModel _currentPlacementSlot;
        private bool _started;

        private bool _hovering;

        private DateTime _timeLastDoubleGrip;
        private DateTime _lastInteraction;

        private DateTime _delayStart;

        private bool _tempFixate;
        private DateTime _tempFixateStartTime;


        public ElementModel()
        {
            LastIndex = -1;
            TimeVisible = 0;
            CurrentLevelOfDetail = 1;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_started)
            {
                if (CurrentLevelOfDetail < 1)
                    CurrentLevelOfDetail = 1;

                _actionQueue = new List<Action>();

                _started = true;
                SaveSpaceTransform();
                _timeLastDoubleGrip = DateTime.MinValue;

                _currentAlphas = new List<float>();

                foreach (var element in LODElements)
                {
                    UnityUtil.SetAlphaRecursive(element, 0.0f);
                    _currentAlphas.Add(Visibility);
                }

                UnityUtil.SetAlphaRecursive(LODElements[CurrentLevelOfDetail - 1], 1.0f);
            }

            //ElementIsInView = CheckIsVisibileWithRenderer();
            ElementIsInView = CheckIsVisible();
            CheckShowLodController();

            PerformActionInQueue();

            if (_tempFixate && (DateTime.Now - _tempFixateStartTime).TotalSeconds >= Constants.TEMP_FIXATE_SEC)
                FixateElement = false;
        }

        public void SwapElements()
        {
            for (int i = 0; i < LODElementsSwapped.Count; i++)
            {
                UnityUtil.SetAlphaRecursive(LODElements[i], 0.0f);
                LODElements[i] = LODElementsSwapped[i];

                if (i != CurrentLevelOfDetail - 1)
                {
                    UnityUtil.SetAlphaRecursive(LODElements[i], 0.0f);
                }
            }

            UnityUtil.SetAlphaRecursive(LODElements[CurrentLevelOfDetail - 1], 1.0f);
            LODElements[CurrentLevelOfDetail - 1].SetActive(true);
        }

        private void PerformActionInQueue()
        {
            if (_actionQueue == null || _actionQueue.Count < 1)
                return;

            var action = _actionQueue[0];
            _actionQueue.RemoveAt(0);

            switch (action.Type)
            {
                case ActionType.FadeIn:
                    var fadeAction = (FadeAction)action;
                    fadeAction.Target.SetActive(true);
                    UnityUtil.SetAlphaRecursive(fadeAction.Target, fadeAction.Value);
                    break;
                case ActionType.FadeOut:
                    var fadeActionOut = (FadeAction)action;
                    UnityUtil.SetAlphaRecursive(fadeActionOut.Target, fadeActionOut.Value);

                    if (fadeActionOut.Value < .05)
                        fadeActionOut.Target.SetActive(false);
                    break;
                case ActionType.PlaceInSpace:
                    PlaceInSpace(false);
                    break;
                case ActionType.PlaceInView:
                    var placeAction = (PlacementAction)action;
                    PlaceInView(placeAction.Slot, false);
                    break;
            }
        }

        private void CheckShowLodController()
        {
            //if (!ElementIsInView)
            //    return;

            if (PlacedViewAnchored)
                return;

            if (LodControllerGameObject == null)
                return;

            //if (FixateElement)
            //{
            //    PlaceLodController();
            //    return;
            //}

            var distance = float.PositiveInfinity;

            if (RightHand != null)
            {
                var center = gameObject.transform.position;
                var hmdPosition = RightHand.transform.position;
                distance = (center - hmdPosition).magnitude;
            }

            if (LeftHand != null)
            {
                var center = gameObject.transform.position;
                var hmdPosition = LeftHand.transform.position;
                var leftDistance = (center - hmdPosition).magnitude;

                distance = Mathf.Min(distance, leftDistance);
            }

            if (distance < 0.5f && !LodControllerGameObject.activeSelf)
            {
                PlaceLodController();
            }

            if (distance >= 0.5f && LodControllerGameObject.activeSelf)
            {
                LodControllerGameObject.SetActive(false);
            }
        }

        void OnDrawGizmos()
        {
            if (!DebugMode)
                UpdateLoD();
        }

        //public void RemoveFromView()
        //{
        //    if (_actionQueue != null)
        //    {
        //        var steps = Constants.ANIMATION_STEPS;
        //        var delta = FadeFactor / steps;

        //        for (var j = 0.0f; j <= 1.0f; j += delta)
        //        {
        //            FadeAction action = new FadeAction
        //            {
        //                Type = ActionType.FadeIn,
        //                Target = LODElements[CurrentLevelOfDetail - 1],
        //                Value = j
        //            };

        //            _actionQueue.Add(action);
        //        }
        //    }
        //}

        public void SetCurrentLoD(int newLoD, bool delay = true)
        {
            if (_actionQueue != null && CurrentLevelOfDetail != newLoD)
            {
                var delta = 1.0f;

                if (delay)
                {
                    var steps = Constants.ANIMATION_STEPS;
                    delta = FadeFactor / steps;
                }

                for (var j = 1.0f; j >= -0.01f; j -= delta)
                {
                    FadeAction action = new FadeAction
                    {
                        Type = ActionType.FadeOut,
                        Target = LODElements[CurrentLevelOfDetail - 1],
                        Value = j
                    };

                    _actionQueue.Add(action);
                }

                for (var j = 0.0f; j <= 1.0f; j += delta)
                {
                    FadeAction action = new FadeAction
                    {
                        Type = ActionType.FadeIn,
                        Target = LODElements[newLoD - 1],
                        Value = j
                    };

                    _actionQueue.Add(action);
                }
            }


            CurrentLevelOfDetail = newLoD;
            UpdateLoD();

            if (newLoD > CurrentLevelOfDetail && RotateTowardsUser)
            {
                Rotate(Headset.transform.position);
            }

            if (!PlacedViewAnchored)
                PlaceLodController();

            CurrentUtility = CalcUtil.GetUtilityForElement(this, CurrentLevelOfDetail);
            CurrentCognitiveLoad = CalcUtil.GetCurrentCognitiveLoadForElement(this);
        }

        private void Rotate(Vector3 targetPosition)
        {
            transform.LookAt(targetPosition);
        }

        private void UpdateLoD()
        {
            if (CurrentLevelOfDetail < 1)
                CurrentLevelOfDetail = 1;
            else if (CurrentLevelOfDetail > LODElements.Count)
                CurrentLevelOfDetail = LODElements.Count;

            for (int i = 0; i < LODElements.Count; i++)
            {
                var isVisibile = LODElements[i].activeSelf;
                var shouldBeVisible = Visibility > 0.0 && CurrentLevelOfDetail - 1 == i;

                if (!Application.IsPlaying(this))
                {
                    LODElements[i].SetActive(shouldBeVisible);
                }
            }
        }


        private bool CheckIsVisible()
        {
            var center = _positionInSpace;
            var hmdPosition = Headset.transform.position;
            var hmdDirection = Headset.transform.forward;

            var slotDirection = center - hmdPosition;
            var angle = Vector3.Angle(slotDirection, hmdDirection);

            if (angle > VisibilityAngleDegree)
                return false;

            var distance = Mathf.Max(0, (center - hmdPosition).magnitude);

            var isHit = Physics.Raycast(hmdPosition, center - hmdPosition, out var hit, distance);

            if (!isHit)
                return true;

            var occluderIsSelf = hit.transform == transform;
            var occluderIsUIElement = hit.transform.GetComponent<ElementModel>() != null || hit.transform.root.name.Equals("Menu");

            //if(occluderIsUIElement)
            //    Debug.Log(this.transform + " / " + hit.transform.GetComponent<ElementModel>());

            return occluderIsSelf || occluderIsUIElement;
        }


        public void PlaceInView(PlacementSlotModel placementSlotModel, bool delay)
        {
            if (PlacedViewAnchored)
                return;

            if (delay)
            {
                var action = new PlacementAction { Type = ActionType.PlaceInView, Target = gameObject, Slot = placementSlotModel };

                if (_actionQueue.Count > 0)
                {
                    var currentActionType = _actionQueue[0].Type;
                    int insertIndex;

                    for (insertIndex = 0; insertIndex < _actionQueue.Count; insertIndex++)
                    {
                        if (_actionQueue[insertIndex].Type != currentActionType)
                            break;
                    }

                    _actionQueue.Insert(insertIndex, action);
                }
                else
                {
                    var steps = Constants.ANIMATION_STEPS;
                    var delta = FadeFactor / steps;

                    for (var j = 1.0f; j > 0.0f; j -= delta)
                    {
                        FadeAction fadeAction = new FadeAction
                        {
                            Type = ActionType.FadeOut,
                            Target = LODElements[CurrentLevelOfDetail - 1],
                            Value = j
                        };

                        _actionQueue.Add(fadeAction);
                    }

                    _actionQueue.Add(action);

                    for (var j = 0.0f; j <= 1.0f; j += delta)
                    {
                        FadeAction fadeAction = new FadeAction
                        {
                            Type = ActionType.FadeIn,
                            Target = LODElements[CurrentLevelOfDetail - 1],
                            Value = j
                        };

                        _actionQueue.Add(fadeAction);
                    }

                }

                return;
            }

            PlacedViewAnchored = true;

            _currentPlacementSlot = placementSlotModel;
            _currentPlacementSlot.AppElement = this;

            SaveSpaceTransform();

            _disabledColliders = LODElements[CurrentLevelOfDetail - 1].GetComponentsInChildren<BoxCollider>();

            //foreach (var boxCollider in _disabledColliders)
            //{
            //    boxCollider.enabled = false;
            //}

            transform.parent = placementSlotModel.Container.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            transform.localScale = new Vector3(Constants.VIEW_SCALE_FACTOR, Constants.VIEW_SCALE_FACTOR, Constants.VIEW_SCALE_FACTOR);

            if (LodControllerGameObject != null)
                LodControllerGameObject.SetActive(false);
        }

        public void PlaceInSpace(bool delay)
        {
            if (!PlacedViewAnchored)
                return;

            if (delay)
            {
                var action = new PlacementAction { Type = ActionType.PlaceInSpace, Target = gameObject };

                if (_actionQueue.Count > 0)
                {
                    var currentActionType = _actionQueue[0].Type;
                    int insertIndex;

                    for (insertIndex = 0; insertIndex < _actionQueue.Count; insertIndex++)
                    {
                        if (_actionQueue[insertIndex].Type != currentActionType)
                            break;
                    }

                    _actionQueue.Insert(insertIndex, action);
                }
                else
                {
                    var steps = Constants.ANIMATION_STEPS;
                    var delta = FadeFactor / steps;

                    for (var j = 1.0f; j > 0.0f; j -= delta)
                    {
                        FadeAction fadeAction = new FadeAction
                        {
                            Type = ActionType.FadeOut,
                            Target = LODElements[CurrentLevelOfDetail - 1],
                            Value = j
                        };

                        _actionQueue.Add(fadeAction);
                    }

                    _actionQueue.Add(action);

                    for (var j = 0.0f; j <= 1.0f; j += delta)
                    {
                        FadeAction fadeAction = new FadeAction
                        {
                            Type = ActionType.FadeIn,
                            Target = LODElements[CurrentLevelOfDetail - 1],
                            Value = j
                        };

                        _actionQueue.Add(fadeAction);
                    }
                }
            }
            else
            {

                if (PlacedViewAnchored)
                {
                    if (_disabledColliders != null)
                    {
                        //foreach (var boxCollider in _disabledColliders)
                        //{
                        //    boxCollider.enabled = false;
                        //}
                    }

                    RestoreSpaceTransform();
                    PlacedViewAnchored = false;
                    _currentPlacementSlot.AppElement = null;
                    _currentPlacementSlot = null;

                    PlaceLodController();
                }
            }
        }

        private void PlaceLodController()
        {
            if (LodControllerGameObject == null || CurrentLevelOfDetail <= 0)
                return;

            LodControllerGameObject.SetActive(true);

            var currentLODElement = LODElements[CurrentLevelOfDetail - 1];

            var largestBounds = GetLargestBoundsFromRenderers(currentLODElement);

            LodControllerGameObject.transform.parent = currentLODElement.transform;

            var newPosition = new Vector3(-largestBounds.extents.x, largestBounds.extents.y, 0);
            newPosition.Scale(new Vector3(1.0f / currentLODElement.transform.localScale.x, 1.0f / currentLODElement.transform.localScale.y, 1.0f / currentLODElement.transform.localScale.z));

            LodControllerGameObject.transform.localPosition = newPosition;
        }

        private static Bounds GetLargestBoundsFromRenderers(GameObject target)
        {
            var renderers = target.GetComponentsInChildren<Renderer>();

            var largestBounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
            {
                if (renderers[i].bounds.extents.sqrMagnitude > largestBounds.extents.sqrMagnitude)
                    largestBounds = renderers[i].bounds;
            }

            return largestBounds;
        }

        private void SaveSpaceTransform()
        {
            _positionInSpace = transform.position;
            _rotationInSpace = transform.rotation;
            _parentInSpace = transform.parent;
            _scaleInSpace = transform.localScale;
        }
        private void RestoreSpaceTransform()
        {
            transform.parent = _parentInSpace;
            transform.position = _positionInSpace;
            transform.rotation = _rotationInSpace;
            transform.localScale = _scaleInSpace;
        }

        public void HandleInteraction()
        {
            if (PlacedViewAnchored)
            {
                PlacedViewAnchored = false;

                SetCurrentLoD(MaxViewLevelOfDetail);

                transform.parent = _parentInSpace;
                transform.localScale = _scaleInSpace;

                _currentPlacementSlot.AppElement = null;
                _currentPlacementSlot = null;

                PlaceLodController();
            }

            //FixateElement = true;
        }

        private void HandHoverUpdate(Hand hand)
        {
            if (!_hovering)
            {
                _hovering = true;

                SteamVR_Input._default.inActions.GrabGrip.AddOnChangeListener(OnGrapGrip, SteamVR_Input_Sources.Any);
                SteamVR_Input._default.inActions.GrabGripDouble.AddOnChangeListener(OnGrapGripDouble, SteamVR_Input_Sources.Any);
            }
        }
        private void OnHandHoverEnd(Hand hand)
        {
            if (_hovering)
            {
                _hovering = false;

                SteamVR_Input._default.inActions.GrabGrip.RemoveOnChangeListener(OnGrapGrip, SteamVR_Input_Sources.Any);
                SteamVR_Input._default.inActions.GrabGripDouble.RemoveOnChangeListener(OnGrapGripDouble, SteamVR_Input_Sources.Any);
            }

        }

        private void OnGrapGrip(SteamVR_Action_In action)
        {
            if (CurrentLevelOfDetail < MaxLevelOfDetail)
                SetCurrentLoD(CurrentLevelOfDetail + 1);
        }

        private void OnGrapGripDouble(SteamVR_Action_In action)
        {
            if (DateTime.Now - _timeLastDoubleGrip < TimeSpan.FromSeconds(1))
                return;

            _timeLastDoubleGrip = DateTime.Now;

            Debug.Log(action.direction);
            if (CurrentLevelOfDetail > 1)
                SetCurrentLoD(CurrentLevelOfDetail - 1);
        }

        public void TriggerPlaceInView()
        {

        }

        public void TriggerTemporaryFixation()
        {
            FixateElement = true;
            _tempFixate = true;
            _tempFixateStartTime = DateTime.Now;
        }

        public void AbortTemporaryFixation()
        {
            _tempFixate = false;
        }
    }
}