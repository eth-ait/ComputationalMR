using System;

namespace Assets.Scripts.Event
{
    public interface ITaskController
    {
        event EventHandler<PrimaryTaskEventArgs> PrimaryTaskTrialCompleted;
        event EventHandler<PrimaryTaskInteractionEventArgs> InteractionHappened;
    }
}