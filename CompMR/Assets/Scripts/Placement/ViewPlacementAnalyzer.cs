using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class ViewPlacementAnalyzer : MonoBehaviour
{
    public GameObject Room;
    public GameObject Headset;
    //public GameObject MenuArea;
    public GameObject MenuContainer;
    public GameObject PlacementAnalyzerPrefab;

    public AppModel AppModel;

    public AnimationCurve QualityFunction;

    public float SlotSideLength = Constants.INITIAL_VIEW_SLOT_SIDE_LENGTH;
    public float SlotDistanceToHmd = Constants.INITIAL_SLOTS_DISTANCE_TO_HMD;

    public bool ShowDebugVisuals = true;
    public bool CreateManually = true;

    public bool EnableQualityIntegration = Constants.ENABLE_QUALITY_INTEGRATION;
    public int IntegrateQualityOverSteps = Constants.INITIAL_QUALITY_STEPS_INTEGRATE;

    private List<PlacementSlotModel> PlacementSlots => AppModel.PlacementSlotsView;

    private bool _started;
    private Camera _camera;

    private bool _showDebugVisuals;
    private GameObject _visual;

    public Vector3 Angle = new Vector3(35.0f, 0, 0);
    public float Radius = 1.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if (!_started)
        {
            _started = true;
            _showDebugVisuals = ShowDebugVisuals;

            _camera = Headset.GetComponent<Camera>();

            if(CreateManually)
                CreateSlotsManual();
            else
                CreateSlots();

        }

        if (ShowDebugVisuals != _showDebugVisuals)
            UpdateSlotPositions();
        
        if (!CreateManually)
        {
            foreach (var slot in PlacementSlots)
            {
                UpdateQuality(slot);
            }
        }
    }

    private void UpdateSlotPositions()
    {
        _showDebugVisuals = ShowDebugVisuals;

        foreach (var slot in PlacementSlots)
        {
            slot.ChangeVisibility(ShowDebugVisuals);
            //slot.Visual.GetComponent<Renderer>().enabled = ShowDebugVisuals;
            //slot.Visual.SetActive(ShowDebugVisuals);
        }
    }

    private void CreateSlots()
    {
        var numSlotsHorizontal = Mathf.FloorToInt(30.0f / SlotSideLength);
        var numSlotsVertical = Mathf.FloorToInt(30.0f / SlotSideLength);

        for (int x = 0; x <= numSlotsHorizontal; x++)
        {
            for (int y = 0; y <= numSlotsVertical; y++)
            {
                var relativeX = x - numSlotsHorizontal / 2.0f;
                var relativeY = y - numSlotsVertical / 2.0f;

                var relativeXNormalized = relativeX / numSlotsHorizontal;
                var relativeYNormalized = relativeY / numSlotsVertical;

                var angle = new Vector3(0, relativeX * SlotSideLength, 0.0f);
                Matrix4x4 localRotation = Matrix4x4.Rotate(Quaternion.Euler(angle));
                var localPosition = localRotation.MultiplyVector(new Vector3(0, 0, Radius));

                var container = new GameObject();
                container.transform.parent = MenuContainer.transform;
                container.transform.localPosition = localPosition + new Vector3(0, relativeY * (SlotSideLength / 30.0f), 0) / 2.0f;
                container.transform.localRotation = Quaternion.identity;

                var direction = new Vector3(relativeXNormalized, 0.0f, 1.0f);
                var angleToHeadset = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
                container.transform.localRotation = Quaternion.Euler(new Vector3(0, angleToHeadset, 0));

                //var visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var visual = Instantiate(PlacementAnalyzerPrefab);
                visual.transform.parent = container.transform;
                visual.transform.localRotation = Quaternion.identity;
                visual.transform.localPosition = Vector3.zero;
                visual.transform.localScale = new Vector3(0.05f, 0.05f, 0.01f);
                visual.GetComponent<BoxCollider>().enabled = false;

                var slot = container.AddComponent<PlacementSlotModel>();
                slot.Visual = visual;
                slot.Container = container;
                slot.Position = new Vector3(relativeXNormalized, relativeYNormalized, 0.0f);
                slot.Bounds = new Bounds(new Vector3(.05f, .05f, 0), visual.transform.localScale);
                slot.IntegrateQualityOverSteps = IntegrateQualityOverSteps;

                var quality = UpdateQuality(slot);

                slot.ChangeQuality(quality);
                slot.ChangeVisibility(ShowDebugVisuals);

                PlacementSlots.Add(slot);
            }
        }
    }

    private void CreateSlotsManual()
    {
        var numSlotsHorizontal = Mathf.FloorToInt(30.0f / SlotSideLength);
        var numSlotsVertical = Mathf.FloorToInt(30.0f / SlotSideLength); 

        var relativePositions = new List<Vector2>()
        {
            new Vector2(0.4f, 0.1f),
            new Vector2(0.4f, -0.1f),
            new Vector2(-0.4f, 0.1f),
            new Vector2(-0.4f, -0.1f),
            new Vector2(-.14f, 0.32f),
            new Vector2(.14f, 0.32f),
            new Vector2(-.14f, -0.32f),
            new Vector2(.14f, -0.32f),
        };

        var qualities = new List<float>()
        {
            1.0f,
            1.0f,
            1.0f,
            1.0f,
            1.0f,
            1.0f,
            0.25f,
            0.25f,
        };

        for (int i = 0; i < relativePositions.Count; i++)
        {
            var relativePositionNormalized = relativePositions[i];
            var relativePosition = new Vector2(relativePositionNormalized.x * numSlotsHorizontal, relativePositionNormalized.y * numSlotsVertical);
            
            var angle = new Vector3(0, relativePosition.x * SlotSideLength, 0.0f);
            Matrix4x4 localRotation = Matrix4x4.Rotate(Quaternion.Euler(angle));
            var localPosition = localRotation.MultiplyVector(new Vector3(0, 0, Radius));

            var container = new GameObject();
            container.transform.parent = MenuContainer.transform;
            container.transform.localPosition = localPosition + new Vector3(0, relativePosition.y * (SlotSideLength / 30.0f), 0) / 2.0f;
            container.transform.localRotation = Quaternion.identity;

            var direction = new Vector3(relativePositionNormalized.x, 0.0f, 1.0f);
            var angleToHeadset = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
            container.transform.localRotation = Quaternion.Euler(new Vector3(0, angleToHeadset, 0));

            //var visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var visual = Instantiate(PlacementAnalyzerPrefab);
            visual.transform.parent = container.transform;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            visual.GetComponent<BoxCollider>().enabled = false;

            var slot = container.AddComponent<PlacementSlotModel>();
            slot.Visual = visual;
            slot.Container = container;
            slot.Position = new Vector3(relativePositionNormalized.x, relativePositionNormalized.y, 0.0f);
            slot.Bounds = new Bounds(new Vector3(.05f, .05f, 0), visual.transform.localScale);
            slot.IntegrateQualityOverSteps = IntegrateQualityOverSteps;


            var quality = qualities[i];

            slot.ChangeQuality(quality);
            slot.ChangeVisibility(ShowDebugVisuals);

            PlacementSlots.Add(slot);
        }
    }

    private float UpdateQuality(PlacementSlotModel slot)
    {
        var posInFoV = new Vector2(slot.Position.x, slot.Position.y);
        var center = new Vector2(0, 0);
        var offset = (center - posInFoV);
        var centerDistance = offset.magnitude;
        var horizontalDistance = Mathf.Abs(offset.x);
        //var quality = Mathf.Max(0, QualityFunction.Evaluate(horizontalDistance) - Mathf.Abs(offset.y));
        var quality = QualityFunction.Evaluate(centerDistance);
        slot.ChangeQuality(quality);
        return quality;
    }
}
