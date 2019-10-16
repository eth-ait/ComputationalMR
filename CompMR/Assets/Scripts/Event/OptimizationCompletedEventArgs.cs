using System;
using Assets.Scripts.Model;

namespace Assets.Scripts.Event
{
    public class OptimizationCompletedEventArgs : EventArgs
    {
        public OptimizationModel Model;
        public TimeSpan Duration;

        public OptimizationCompletedEventArgs(OptimizationModel model, TimeSpan duration)
        {
            Model = model;
            Duration = duration;
        }
    }
}