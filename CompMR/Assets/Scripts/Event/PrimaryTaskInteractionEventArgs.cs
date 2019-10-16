using System;

namespace Assets.Scripts.Event
{
    public class PrimaryTaskInteractionEventArgs : EventArgs
    {
        public string TaskName;
        public int TrialCounter;
        public string[] Parameters;

        public PrimaryTaskInteractionEventArgs(string taskName, int trialCounter, params string[] parameters)
        {
            TaskName = taskName;
            TrialCounter = trialCounter;

            Parameters = parameters;
        }
    }
}