using System;

namespace Assets.Scripts.Event
{
    public class GazeDataEventArgs : EventArgs
    {
        public float NormalizedIPA;
        public float RawIPA;
        public float SmoothIPA;

        public GazeDataEventArgs(float normalizedIpa, float rawIpa, float smoothIPA)
        {
            NormalizedIPA = normalizedIpa;
            RawIPA = rawIpa;
            SmoothIPA = smoothIPA;
        }
    }
}