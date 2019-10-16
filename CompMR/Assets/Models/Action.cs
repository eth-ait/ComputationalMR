using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Models
{
    public enum ActionType
    {
        FadeIn,
        FadeOut,
        PlaceInSpace,
        PlaceInView
    }

    public class Action
    {
        public ActionType Type;
        public GameObject Target;
    }

    public class PlacementAction : Action
    {
        public PlacementSlotModel Slot;
    }

    public class FadeAction : Action
    {
        public float Value;
    }
}