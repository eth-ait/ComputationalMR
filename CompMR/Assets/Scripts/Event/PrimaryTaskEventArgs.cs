using System;

namespace Assets.Scripts.Event
{
    public class PrimaryTaskEventArgs : EventArgs
    {
        public string TaskName;
        public int TrialCounter;
        public string[] Parameters;

        public PrimaryTaskEventArgs(string taskName, int trialCounter, params string[] parameters)
        {
            TaskName = taskName;
            TrialCounter = trialCounter;

            Parameters = parameters;
        }
    }
}