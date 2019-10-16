using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public class SoundTimer : MonoBehaviour
    {
        public int TimerSeconds = 60;
        public AudioSource AudioSource;

        private DateTime _lastBeep;

        // Start is called before the first frame update
        void Start()
        {
            _lastBeep = DateTime.Now;
        }

        // Update is called once per frame
        void Update()
        {
            if (AudioSource.enabled == false)
                AudioSource.enabled = true;

            var now = DateTime.Now;

            var timeElapsed = now - _lastBeep;
            if (timeElapsed.TotalSeconds > TimerSeconds)
            {

                AudioSource.Play(0);
                _lastBeep = now;
            }
        }
    }
}
