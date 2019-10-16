using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Placement
{
    public class SpatialPlacementAnalyzer : MonoBehaviour
    {
        public GameObject Room;
        public GameObject Headset;

        public AnimationCurve PositionQuality;
        public AnimationCurve AngleQuality;

        public float OcclusionOffset = Constants.INITIAL_OCCLUSION_OFFSET;
        public float MinPlayerSlotDistance = Constants.INITIAL_MIN_PLACEMENT_DISTANCE;

        public float MinHeight = Constants.INITIAL_SLOTS_HEIGHT_MIN;
        public float MaxHeight = Constants.INITIAL_SLOTS_HEIGHT_MAX;

        public float WeightDistance = Constants.INITIAL_WEIGHT_DISTANCE;
        public float WeightAngle = 1.0f - Constants.INITIAL_WEIGHT_DISTANCE;

        public float SlotSideLength = Constants.SLOT_SIDE_LENGTH;

        public int NumBestSlotsToDisplay = Constants.INITIAL_NUM_BEST_SLOTS_DISPLAY;
        public bool EnableFilterBestSlots = false;

        public bool ShowDebugVisuals = true;
        public bool EnableQualityIntegration = Constants.ENABLE_QUALITY_INTEGRATION;
        public int IntegrateQualityOverSteps = Constants.INITIAL_QUALITY_STEPS_INTEGRATE;

        private bool _started;
        private Bounds _roomDimensions;
        private float _maxDistance;

        private List<PlacementSlotModel> _placementSlots;


        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!_started)
            {
                _started = true;

                _placementSlots = new List<PlacementSlotModel>();

                _roomDimensions = UnityUtil.GetMaxBounds(Room);
                _maxDistance = (_roomDimensions.max - _roomDimensions.min).magnitude / 2.0f;

                Debug.Log(_roomDimensions);

                CreateSlots();

                //RenderDebugRoomBoundingBox(roomDimensions);
            }

            UpdateSlotQuality();

            if (ShowDebugVisuals)
                FilterSlots();
        }

        private void FilterSlots()
        {
            if (EnableFilterBestSlots)
            {
                var bestSlots = _placementSlots.Where(slot => !slot.IsOccluded).OrderByDescending(slot => slot.Quality).Take(NumBestSlotsToDisplay).ToList();

                foreach (var slot in bestSlots)
                {
                    slot.ChangeVisibility(true);
                }

                foreach (var slot in _placementSlots)
                {
                    if (!bestSlots.Contains(slot))
                        slot.ChangeVisibility(false);
                }

            }
            else
            {
                foreach (var slot in _placementSlots)
                {
                    var visibility = slot.Quality > .001f && !slot.IsOccluded;
                    slot.ChangeVisibility(visibility);
                }

            }
        }

        private void UpdateSlotQuality()
        {
            var hmdPosition = Headset.transform.position;
            var hmdDirection = Headset.transform.forward;

            foreach (var placementSlot in _placementSlots)
            {
                var slotPlayerDistance = (hmdPosition - placementSlot.Position).magnitude;

                var invalidPosition = slotPlayerDistance < MinPlayerSlotDistance ||
                                      placementSlot.Position.y < MinHeight ||
                                      placementSlot.Position.y > MaxHeight;

                if (invalidPosition)
                {
                    placementSlot.ChangeQuality(0.0f, EnableQualityIntegration);
                    //placementSlot.ChangeVisibility(false);
                }
                else
                {
                    placementSlot.IsOccluded = IsOccluded(placementSlot, hmdPosition, out var distance);

                    if (placementSlot.IsOccluded)
                    {
                        placementSlot.ChangeQuality(0.0f, EnableQualityIntegration);
                        //placementSlot.ChangeVisibility(false);
                    }
                    else
                    {
                        var angleQuality = GetSlotHmdAngleQuality(placementSlot, hmdPosition, hmdDirection);
                        angleQuality = AngleQuality.Evaluate(angleQuality);

                        distance = PositionQuality.Evaluate(distance);

                        //var combinedWeight = WeightDistance * 1.0f / Mathf.Exp(Mathf.Sqrt(distance)) + WeightAngle * angleQuality;
                        var combinedWeight = WeightDistance * distance + WeightAngle * angleQuality;
                        var quality = angleQuality < .001f ? 0.0f : combinedWeight;
                        placementSlot.ChangeQuality(quality, EnableQualityIntegration);

                        //var visibleFromQuality = quality > 0.00001;
                        //placementSlot.ChangeVisibility(visibleFromQuality && ShowDebugVisuals && !EnableFilterBestSlots);
                    }
                }
            }
        }

        private bool IsOccluded(PlacementSlotModel placementSlot, Vector3 hmdPosition, out float distance)
        {
            var slotPlayerDistance = (hmdPosition - placementSlot.Position).magnitude;

            var containsGeometry = ContainsGeometry(placementSlot, out placementSlot.Occluder);

            if (containsGeometry)
            {
                distance = float.NegativeInfinity;
                return true;
            }

            var slotDistance = Mathf.Max(0, slotPlayerDistance - OcclusionOffset);

            var isHit = Physics.Raycast(hmdPosition, placementSlot.Position - hmdPosition, out var hit, slotDistance);

            if (isHit)
            {
                placementSlot.Occluder = hit.transform;
            }

            if (isHit || slotPlayerDistance < MinPlayerSlotDistance)
            {
                distance = float.NegativeInfinity;
                return true;
            }

            placementSlot.ChangeVisibility(ShowDebugVisuals);

            //var normalizedDistance = 1.0f - Mathf.Pow(slotPlayerDistance / maxDistance, 1f / 2f);
            var normalizedDistance = slotPlayerDistance / _maxDistance;
            distance = Mathf.Clamp(normalizedDistance, 0.0f, 1.0f);

            return false;
        }

        private float GetSlotHmdAngleQuality(PlacementSlotModel placementSlot, Vector3 hmdPosition, Vector3 hmdDirection)
        {
            var slotDirection = placementSlot.Position  - hmdPosition;
            var angle = Vector3.Angle(slotDirection, hmdDirection);

            var angleQuality = angle / 180.0f;
            
            return angleQuality;
        }

        private void CreateSlots()
        {
            var offsetVector = new Vector3(SlotSideLength * .5f, SlotSideLength * .5f, SlotSideLength * .5f);
            var size = new Vector3Int(Mathf.FloorToInt(_roomDimensions.size.x / SlotSideLength),
                                        Mathf.FloorToInt(_roomDimensions.size.y / SlotSideLength),
                                        Mathf.FloorToInt(_roomDimensions.size.z / SlotSideLength));

            var debugCubeSize = .05f;

            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    for (var z = 0; z < size.z; z++)
                    {
                        var currentSlotMinPos = _roomDimensions.min + new Vector3(x * SlotSideLength, y * SlotSideLength, z * SlotSideLength);
                        var currentSlotCenter = currentSlotMinPos + (offsetVector);

                        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.GetComponent<BoxCollider>().enabled = false;
                        cube.transform.position = currentSlotCenter;
                        cube.transform.localScale = new Vector3(debugCubeSize, debugCubeSize, debugCubeSize);
                        cube.transform.parent = transform;

                        //var colorMagnitude = new Vector3(x, y, z).magnitude / size.magnitude;
                        //cube.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f - colorMagnitude, 1.0f - colorMagnitude);

                        var placementSlotModel = cube.AddComponent<PlacementSlotModel>();
                        placementSlotModel.Bounds = new Bounds(currentSlotCenter, new Vector3(SlotSideLength, SlotSideLength, SlotSideLength));
                        placementSlotModel.Visual = cube;
                        placementSlotModel.SideLength = SlotSideLength;
                        placementSlotModel.Position = currentSlotCenter;
                        placementSlotModel.IntegrateQualityOverSteps = IntegrateQualityOverSteps;

                        placementSlotModel.ChangeQuality(0.0f);
                        //placementSlotModel.ChangeVisibility(ShowDebugVisuals);

                        _placementSlots.Add(placementSlotModel);
                    }
                }
            }

            RenderDebugSlotsMinMax(_roomDimensions, debugCubeSize);
        }

        private bool ContainsGeometry(PlacementSlotModel slot, out Transform occluder)
        {
            var allElements = Room.GetComponentsInChildren<Collider>();

            foreach (var collider in allElements)
            {
                if (collider.bounds.Intersects(slot.Bounds))
                {
                    occluder = collider.transform;
                    return true;
                }
            }

            occluder = null;
            return false;
        }

        private void RenderDebugSlotsMinMax(Bounds bounds, float debugCubeSize)
        {
            var cubeMin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeMin.GetComponent<BoxCollider>().enabled = false;
            cubeMin.transform.position = bounds.min;
            cubeMin.transform.localScale = new Vector3(debugCubeSize, debugCubeSize, debugCubeSize);
            cubeMin.GetComponent<Renderer>().material.color = new Color(0f, 1f, 0f);
            cubeMin.transform.parent = transform;

            var cubeMax = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeMax.GetComponent<BoxCollider>().enabled = false;
            cubeMax.transform.position = bounds.max;
            cubeMax.transform.localScale = new Vector3(debugCubeSize, debugCubeSize, debugCubeSize);
            cubeMax.GetComponent<Renderer>().material.color = new Color(0f, 1f, 0f);
            cubeMax.transform.parent = transform;
        }

        private void RenderDebugRoomBoundingBox(Bounds roomDimensions)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<BoxCollider>().enabled = false;
            cube.transform.position = roomDimensions.center;
            cube.transform.localScale = roomDimensions.size;

            cube.transform.parent = transform;
        }
    }
}
