using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class ConditionBuilderUtil
    {
        public static PrimaryTaskType[][] PrimaryTasks =
        {
            new[] {PrimaryTaskType.High, PrimaryTaskType.Mid, PrimaryTaskType.Low},
            new[] {PrimaryTaskType.Mid, PrimaryTaskType.Low, PrimaryTaskType.High},
            new[] {PrimaryTaskType.Low, PrimaryTaskType.High, PrimaryTaskType.Mid},
            new[] {PrimaryTaskType.Low, PrimaryTaskType.Mid, PrimaryTaskType.High},
            new[] {PrimaryTaskType.High, PrimaryTaskType.Low, PrimaryTaskType.Mid},
            new[] {PrimaryTaskType.Mid, PrimaryTaskType.High, PrimaryTaskType.Low},
        };

        public static SecondaryTaskType[][] SecondaryTasks =
        {
            new[] {SecondaryTaskType.Email, SecondaryTaskType.Break, SecondaryTaskType.Ideation},
            new[] {SecondaryTaskType.Break, SecondaryTaskType.Ideation, SecondaryTaskType.Email},
            new[] {SecondaryTaskType.Ideation, SecondaryTaskType.Email, SecondaryTaskType.Break},
            new[] {SecondaryTaskType.Ideation, SecondaryTaskType.Break, SecondaryTaskType.Email},
            new[] {SecondaryTaskType.Email, SecondaryTaskType.Ideation, SecondaryTaskType.Break},
            new[] {SecondaryTaskType.Break, SecondaryTaskType.Email, SecondaryTaskType.Ideation},
        };

        public static List<ConditionModelTemplate> GetConditionsForParticipant(int participantID)
        {
            List<ConditionModelTemplate> models = new List<ConditionModelTemplate>();

            var primaryTasks = PrimaryTasks[participantID % PrimaryTasks.Length];
            var secondaryTasks = SecondaryTasks[participantID % PrimaryTasks.Length];

            var methodFirst = participantID % 2 == 0 ? MethodType.Manual : MethodType.Automatic;
            var methodSecond = methodFirst == MethodType.Automatic ? MethodType.Manual : MethodType.Automatic;

            var currentPosition = participantID % 2 == 0 ? ConditionPosition.Left : ConditionPosition.Right;
            
            for (int i = 0; i < 3; i++)
            {
                var condition = new ConditionModelTemplate
                {
                    PrimaryTask = primaryTasks[i],
                    SecondaryTask = secondaryTasks[i],
                    Method = methodFirst,
                    Position = currentPosition
                };
                models.Add(condition);

                currentPosition = currentPosition == ConditionPosition.Left
                    ? ConditionPosition.Right
                    : ConditionPosition.Left;
            }
            
            for (int i = 0; i < 3; i++)
            {
                var condition = new ConditionModelTemplate
                {
                    PrimaryTask = primaryTasks[i],
                    SecondaryTask = secondaryTasks[i],
                    Method = methodSecond,
                    Position = currentPosition
                };
                models.Add(condition);

                currentPosition = currentPosition == ConditionPosition.Left
                    ? ConditionPosition.Right
                    : ConditionPosition.Left;
            }

            return models;
        }
    }
}